using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    /// IAuditLogCleanupActionHandler performs the action on the logs that are being cleaned up. If null, then no action
    /// is performed.
    /// </summary>
    public class AuditLogCleaner : BackgroundService
    {
        private ILogger<AuditLogCleaner> Logger { get; }
        private IServiceScopeFactory ScopeFactory { get; }
        private AuditLogCleanupPolicy AuditLogCleanupPolicy { get; }
        private AuditLogCleanupSettings AuditLogCleanupSettings { get; }
        private IAuditLogCleanupActionHandler AuditLogCleanupActionHandler { get; }

        public AuditLogCleaner(ILogger<AuditLogCleaner> logger,
            IServiceScopeFactory scopeFactory,
            AuditLogCleanupPolicy auditLogCleanupPolicy,
            AuditLogCleanupSettings auditLogCleanupSettings,
            IAuditLogCleanupActionHandler auditLogCleanupActionHandler)
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

            AuditLogCleanupActionHandler = auditLogCleanupActionHandler;
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
        
        private async Task<IQueryable<AuditLog>> ApplyAuditLogCleanupPolicyAsync(IQueryable<AuditLog> auditLogsToDeleteQuery, 
            AuditLogger auditLogger, CancellationToken stoppingToken)
        {
            if (AuditLogCleanupPolicy.TimeWindow != null && AuditLogCleanupPolicy.ThresholdValue == null)
            {
                auditLogsToDeleteQuery =
                    ApplyTimeWindowCleanupPolicy(auditLogsToDeleteQuery, AuditLogCleanupPolicy.TimeWindow.Value);
            }

            else if (AuditLogCleanupPolicy.TimeWindow == null && AuditLogCleanupPolicy.ThresholdValue != null)
            {
                auditLogsToDeleteQuery = await ApplyThresholdValueCleanupPolicyAsync(auditLogsToDeleteQuery,
                    AuditLogCleanupPolicy.ThresholdValue.Value, auditLogger);
            }
            
            else if (AuditLogCleanupPolicy.TimeWindow != null && AuditLogCleanupPolicy.ThresholdValue != null)
            {
                DateTime oldestAllowedAuditLogTime = GetOldestAllowedTimestamp(AuditLogCleanupPolicy.TimeWindow.Value);
                int totalRecords = await auditLogger.OnlyAuditLog.CountAsync(cancellationToken: stoppingToken);
                if (totalRecords > AuditLogCleanupPolicy.ThresholdValue)
                {
                    AuditLog oldestRecordToKeep =
                        await GetOldestRecordToKeepAsync(auditLogger, AuditLogCleanupPolicy.ThresholdValue.Value);
                    
                    auditLogsToDeleteQuery = ApplyTimeWindowAndThresholdValueCleanupPolicy(auditLogsToDeleteQuery,
                        oldestRecordToKeep, oldestAllowedAuditLogTime);
                }
                else
                {
                    auditLogsToDeleteQuery = ApplyTimestampFilter(auditLogsToDeleteQuery, oldestAllowedAuditLogTime);
                }
            }

            auditLogsToDeleteQuery =
                ApplyExcludeTablesFilter(auditLogsToDeleteQuery, AuditLogCleanupPolicy.ExcludeTables);

            return auditLogsToDeleteQuery;
        }

        public static IQueryable<AuditLog> ApplyTimeWindowCleanupPolicy(IQueryable<AuditLog> auditLogsToDeleteQuery, 
            TimeSpan timeWindow)
        {
            DateTime oldestAllowedAuditLogTime = GetOldestAllowedTimestamp(timeWindow);
            return ApplyTimestampFilter(auditLogsToDeleteQuery, timestamp: oldestAllowedAuditLogTime);
        }

        public static async Task<IQueryable<AuditLog>> ApplyThresholdValueCleanupPolicyAsync(
            IQueryable<AuditLog> auditLogsToDeleteQuery, int thresholdValue, AuditLogger auditLogger)
        {
            AuditLog oldestRecordToKeep =
                await GetOldestRecordToKeepAsync(auditLogger, thresholdValue);

            return ApplyTimestampFilter(auditLogsToDeleteQuery, oldestRecordToKeep?.TimeStamp ?? DateTime.MinValue);
        }
        
        public static IQueryable<AuditLog> ApplyTimeWindowAndThresholdValueCleanupPolicy(
            IQueryable<AuditLog> auditLogsToDeleteQuery, AuditLog oldestRecordToKeep, DateTime oldestAllowedAuditLogTime)
        {
            return oldestRecordToKeep != null
                ? auditLogsToDeleteQuery.Where(log => log.TimeStamp < oldestRecordToKeep.TimeStamp || log.TimeStamp < oldestAllowedAuditLogTime)
                : ApplyTimestampFilter(auditLogsToDeleteQuery, oldestAllowedAuditLogTime);
        }
        
        public async Task<int> CleanupAuditLogsAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = ScopeFactory.CreateScope();
            AuditLogger auditLogger = scope.ServiceProvider.GetRequiredService<AuditLogger>();

            IQueryable<AuditLog> auditLogsToDeleteQuery = auditLogger.OnlyAuditLog;

            if (AuditLogCleanupActionHandler != null)
                auditLogsToDeleteQuery = auditLogsToDeleteQuery.Include(a => a.AuditLogChanges);

            auditLogsToDeleteQuery = await ApplyAuditLogCleanupPolicyAsync(auditLogsToDeleteQuery, auditLogger, stoppingToken);
            
            // Apply OrderBy
            auditLogsToDeleteQuery = auditLogsToDeleteQuery
                .OrderBy(a => a.TimeStamp);

            int totalLogsDeleted = await auditLogger.DeleteAuditLogsInBatchesAsync(auditLogsToDeleteQuery,
                (int)AuditLogCleanupSettings.BatchSize, AuditLogCleanupActionHandler, stoppingToken);

            return totalLogsDeleted;
        }

        public static async Task<AuditLog> GetOldestRecordToKeepAsync(AuditLogger auditLogger, int thresholdValue)
        {
            // Fetch the timestamp first of the threshold record
            DateTime oldestRecordToKeepTimestamp = await auditLogger.OnlyAuditLog
                .OrderByDescending(log => log.TimeStamp)
                .Select(log => log.TimeStamp)
                .Skip(thresholdValue - 1)
                .FirstOrDefaultAsync();
            
            // Now retrieve the threshold record
            return await auditLogger.OnlyAuditLog
                .OrderByDescending(log => log.TimeStamp)
                .FirstOrDefaultAsync(log => log.TimeStamp == oldestRecordToKeepTimestamp);
        }


        public static DateTime GetOldestAllowedTimestamp(TimeSpan timeWindow) =>
            DateTime.UtcNow.Subtract(timeWindow);

        public static IQueryable<AuditLog> ApplyTimestampFilter(IQueryable<AuditLog> query, DateTime timestamp) =>
            query.Where(log => log.TimeStamp < timestamp);

        public static IQueryable<AuditLog> ApplyExcludeTablesFilter(IQueryable<AuditLog> query, string[] excludeTables) =>
            excludeTables != null && excludeTables.Any()
                ? query.Where(log => !excludeTables.Contains(log.TableName))
                : query;
    }
}