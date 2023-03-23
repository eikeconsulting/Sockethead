using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sockethead.EFCore.Dto;
using Sockethead.EFCore.Entities;

namespace Sockethead.EFCore.AuditLogging
{
    /// <summary>
    /// A background service that periodically cleans up old audit logs from the AuditLog database based on specific
    /// cleanup policies and settings.
    /// AuditLogCleanupPolicy specifies the policy for removing audit logs. If no policy is defined, the default policy
    /// is used to delete records older than 30 days.
    /// AuditLogCleanupSettings specifies settings for the cleanup operation, such as batch size and cleanup interval.
    /// If no settings are defined, the default settings of 500 records per batch and a cleanup interval of 1 hour are used.
    /// </summary>
    public class AuditLogCleaner : BackgroundService
    {
        private readonly ILogger<AuditLogCleaner> Logger;
        private readonly IServiceScopeFactory ScopeFactory;
        private AuditLogCleanupPolicy AuditLogCleanupPolicy { get; }
        private AuditLogCleanupSettings AuditLogCleanupSettings { get; }

        public AuditLogCleaner(ILogger<AuditLogCleaner> logger,
            IServiceScopeFactory scopeFactory,
            AuditLogCleanupPolicy auditLogCleanupPolicy,
            AuditLogCleanupSettings auditLogCleanupSettings)
        {
            Logger = logger;
            ScopeFactory = scopeFactory;

            AuditLogCleanupPolicy = auditLogCleanupPolicy ?? new AuditLogCleanupPolicy();
            if (AuditLogCleanupPolicy.TimeWindow == null && AuditLogCleanupPolicy.ThresholdValue == null)
                AuditLogCleanupPolicy.TimeWindow = TimeSpan.FromDays(30);

            AuditLogCleanupSettings = auditLogCleanupSettings ?? new AuditLogCleanupSettings();
            if (AuditLogCleanupSettings.BatchSize == 0)
                AuditLogCleanupSettings.BatchSize = 500;
            if (AuditLogCleanupSettings.CleanupInterval == default)
                AuditLogCleanupSettings.CleanupInterval = TimeSpan.FromHours(1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Logger.LogInformation("Audit log cleanup task running at: {time}", DateTime.UtcNow);

                    int totalLogsDeleted = await CleanupAuditLogsAsync(stoppingToken);

                    Logger.LogInformation("{count} audit logs deleted", totalLogsDeleted);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "An error occurred while cleaning up audit logs.");
                }

                await Task.Delay(AuditLogCleanupSettings.CleanupInterval, stoppingToken);
            }
        }

        public async Task<int> CleanupAuditLogsAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = ScopeFactory.CreateScope();
            AuditLogger auditLogger = scope.ServiceProvider.GetRequiredService<AuditLogger>();

            DateTime oldestAllowedAuditLogTime;
            AuditLog oldestRecordToKeep;
            
            int totalLogsDeleted = 0;
            IQueryable<AuditLog> auditLogsToDeleteQuery =
                auditLogger.OnlyAuditLog
                    .OrderBy(o => o.TimeStamp);

            if (AuditLogCleanupPolicy.TimeWindow != null && AuditLogCleanupPolicy.ThresholdValue == null)
            {
                oldestAllowedAuditLogTime = GetOldestAllowedTimestamp(AuditLogCleanupPolicy.TimeWindow.Value);
                auditLogsToDeleteQuery = ApplyTimestampFilter(auditLogsToDeleteQuery, oldestAllowedAuditLogTime);
            }

            else if (AuditLogCleanupPolicy.TimeWindow == null && AuditLogCleanupPolicy.ThresholdValue != null)
            {
                oldestRecordToKeep =
                    await GetOldestRecordToKeepAsync(auditLogger, AuditLogCleanupPolicy.ThresholdValue.Value);

                if (oldestRecordToKeep != null)
                    auditLogsToDeleteQuery = ApplyTimestampFilter(auditLogsToDeleteQuery, oldestRecordToKeep.TimeStamp);
            }
            
            else if (AuditLogCleanupPolicy.TimeWindow != null && AuditLogCleanupPolicy.ThresholdValue != null)
            {
                oldestAllowedAuditLogTime = GetOldestAllowedTimestamp(AuditLogCleanupPolicy.TimeWindow.Value);
                int totalRecords = await auditLogger.OnlyAuditLog.CountAsync(cancellationToken: stoppingToken);
                if (totalRecords > AuditLogCleanupPolicy.ThresholdValue)
                {
                    oldestRecordToKeep =
                        await GetOldestRecordToKeepAsync(auditLogger, AuditLogCleanupPolicy.ThresholdValue.Value);
                    
                    auditLogsToDeleteQuery = oldestRecordToKeep != null
                        ? auditLogsToDeleteQuery.Where(log => log.TimeStamp < oldestRecordToKeep.TimeStamp || log.TimeStamp < oldestAllowedAuditLogTime)
                        : ApplyTimestampFilter(auditLogsToDeleteQuery, oldestAllowedAuditLogTime);
                }
                else
                {
                    auditLogsToDeleteQuery = ApplyTimestampFilter(auditLogsToDeleteQuery, oldestAllowedAuditLogTime);
                }
            }

            if (AuditLogCleanupPolicy.ExcludeTables != null && AuditLogCleanupPolicy.ExcludeTables.Any())
                auditLogsToDeleteQuery =
                    auditLogsToDeleteQuery.Where(log => !AuditLogCleanupPolicy.ExcludeTables.Contains(log.TableName));
            
            // Apply Batch Size
            auditLogsToDeleteQuery = auditLogsToDeleteQuery
                .Take((int)AuditLogCleanupSettings.BatchSize)
                .AsNoTracking();
            do
            {
                var auditLogsToDeleteList = await auditLogsToDeleteQuery.ToListAsync(cancellationToken: stoppingToken);
                // ToDo: Pass this list of audit logs to be deleted to a method and archives them by saving the logs.
                
                await auditLogger.AuditLogDbContext.BulkDeleteAsync(auditLogsToDeleteList, cancellationToken: stoppingToken);
                totalLogsDeleted += auditLogsToDeleteList.Count;
            }
            while (auditLogsToDeleteQuery.Any());

            return totalLogsDeleted;
        }

        public static async Task<AuditLog> GetOldestRecordToKeepAsync(AuditLogger auditLogger, int thresholdValue) =>
            await auditLogger.OnlyAuditLog
                .OrderByDescending(log => log.TimeStamp)
                .Skip(thresholdValue - 1)
                .FirstOrDefaultAsync();

        public static DateTime GetOldestAllowedTimestamp(TimeSpan timeWindow) =>
            DateTime.UtcNow.Subtract(timeWindow);

        public static IQueryable<AuditLog> ApplyTimestampFilter(IQueryable<AuditLog> query, DateTime timestamp) =>
            query.Where(log => log.TimeStamp < timestamp);

        public static IQueryable<AuditLog> ApplyExcludeTablesFilter(IQueryable<AuditLog> query, string[] excludeTables) =>
            query.Where(log => !excludeTables.Contains(log.TableName));
    }
}