using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sockethead.EFCore.AuditLogging
{
    /// <summary>
    /// A background service that periodically cleans up old audit logs from the AuditLog database. It takes in optional parameters for
    /// the cleanup interval, audit log age, and batch size, defaulting to 1 hour, 30 days, and 500 records, respectively,
    /// if no values are provided. The service deletes logs based on UTC time whenever it is invoked. The batch size parameter
    /// controls how many records are deleted in each batch to avoid overwhelming the database with a large number of deletions
    /// at once.
    /// </summary>
    public class AuditLogCleaner : BackgroundService
    {
        private readonly ILogger<AuditLogCleaner> Logger;
        private readonly IServiceScopeFactory ScopeFactory;
        private readonly TimeSpan CleanupInterval;
        private readonly TimeSpan AuditLogAge;
        private readonly int BatchSize;

        public AuditLogCleaner(ILogger<AuditLogCleaner> logger,
            IServiceScopeFactory scopeFactory,
            TimeSpan cleanupInterval = default,
            TimeSpan auditLogAge = default,
            int batchSize = 500)
        {
            Logger = logger;
            ScopeFactory = scopeFactory;
            CleanupInterval = cleanupInterval == default ? TimeSpan.FromHours(1) : cleanupInterval;
            AuditLogAge = auditLogAge == default ? TimeSpan.FromDays(30) : auditLogAge;
            BatchSize = batchSize;
        }

        public TimeSpan GetAuditLogAge() => AuditLogAge;

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

                await Task.Delay(CleanupInterval, stoppingToken);
            }
        }

        public async Task<int> CleanupAuditLogsAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = ScopeFactory.CreateScope();
            AuditLogger auditLogger = scope.ServiceProvider.GetRequiredService<AuditLogger>();

            DateTime oldestAllowedAuditLogTime = DateTime.UtcNow.Subtract(AuditLogAge);

            int totalLogsDeleted = 0;
            IQueryable<Entities.AuditLog> auditLogsToDeleteQuery =
                auditLogger.OnlyAuditLog
                    .OrderBy(o => o.TimeStamp)
                    .Where(log => log.TimeStamp < oldestAllowedAuditLogTime)
                    .Take(BatchSize);

            do
            {
                auditLogger.AuditLogDbContext.AuditLogs.RemoveRange(auditLogsToDeleteQuery);
                totalLogsDeleted += await auditLogger.AuditLogDbContext.SaveChangesAsync(stoppingToken);
            }
            while (auditLogsToDeleteQuery.Any());

            return totalLogsDeleted;
        }
    }
}