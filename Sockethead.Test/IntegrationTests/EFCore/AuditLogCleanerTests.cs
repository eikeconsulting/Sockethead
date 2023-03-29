using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sockethead.EFCore.AuditLogging;
using Sockethead.EFCore.Dto;
using Sockethead.EFCore.Entities;
using Sockethead.Test.Common;
using Xunit;

namespace Sockethead.Test.IntegrationTests.EFCore
{
    public class AuditLogCleanerTests : IClassFixture<TestsFixture>
    {
        private TestsFixture Fixture { get; }
        private ILogger<AuditLogCleaner> Logger { get; }
        private IServiceScopeFactory ServiceScopeFactory { get; }
        private AuditLogger AuditLogger { get; }

        public AuditLogCleanerTests(TestsFixture fixture)
        {
            Fixture = fixture;
            Logger = Fixture.GetService<ILogger<AuditLogCleaner>>();
            ServiceScopeFactory = Fixture.GetService<IServiceScopeFactory>();
            AuditLogger = Fixture.GetService<AuditLogger>();
        }

        private static AuditLogCleanupPolicy GetAuditLogCleanupPolicy(TimeSpan? timeWindow = null,
            int? thresholdValue = null, string[] excludeTables = null)
        {
            AuditLogCleanupPolicy policy = new AuditLogCleanupPolicy
            {
                TimeWindow = timeWindow,
                ThresholdValue = thresholdValue,
                ExcludeTables = excludeTables
            };

            if (policy.TimeWindow == null && policy.ThresholdValue == null)
                policy.TimeWindow = TimeSpan.FromDays(30);

            return policy;
        }

        private static AuditLogCleanupSettings GetAuditLogCleanupSettings()
        {
            return new AuditLogCleanupSettings
            {
                BatchSize = 500,
                CleanupInterval = TimeSpan.FromHours(1)
            };
        }

        [Fact]
        public async Task TimeWindowCleanupPolicy_OldAuditLogsShouldBeDeleted()
        {
            AuditLogCleanupPolicy policy = GetAuditLogCleanupPolicy();
            AuditLogCleaner auditLogCleaner =
                new AuditLogCleaner(Logger, ServiceScopeFactory, policy, GetAuditLogCleanupSettings(), null);

            IQueryable<AuditLog> auditLogsQuery =
                AuditLogCleaner.ApplyTimeWindowCleanupPolicy(AuditLogger.OnlyAuditLog, policy.TimeWindow.Value);
        
            if(!await auditLogsQuery.AnyAsync())
                return;

            int totalLogsDeleted = await auditLogCleaner.CleanupAuditLogsAsync(CancellationToken.None);
            totalLogsDeleted.Should().BeGreaterThan(0);
        
            Assert.False(await auditLogsQuery.AnyAsync());
        }
        
        [Fact]
        public async Task ThresholdValueCleanupPolicy_OldAuditLogsShouldBeDeleted()
        {
            AuditLogCleanupPolicy policy = GetAuditLogCleanupPolicy(null, 1000);
            AuditLogCleaner auditLogCleaner =
                new AuditLogCleaner(Logger, ServiceScopeFactory, policy, GetAuditLogCleanupSettings(), null);
            
            IQueryable<AuditLog> auditLogsQuery =
                await AuditLogCleaner.ApplyThresholdValueCleanupPolicyAsync(AuditLogger.OnlyAuditLog,
                    policy.ThresholdValue.Value, AuditLogger);
            
            if(!await auditLogsQuery.AnyAsync())
                return;

            int totalLogsDeleted = await auditLogCleaner.CleanupAuditLogsAsync(CancellationToken.None);
            totalLogsDeleted.Should().BeGreaterThan(0);
        
            Assert.False(await auditLogsQuery.AnyAsync());
        }
        
        [Fact]
        public async Task TimeWindowAndThresholdValueCleanupPolicy_OldAuditLogsShouldBeDeleted()
        {
            AuditLogCleanupPolicy policy = GetAuditLogCleanupPolicy(TimeSpan.FromDays(30),
                1000, new[] { "User" });
            AuditLogCleaner auditLogCleaner =
                new AuditLogCleaner(Logger, ServiceScopeFactory, policy, GetAuditLogCleanupSettings(), null);

            AuditLog oldestRecordToKeep =
                await AuditLogCleaner.GetOldestRecordToKeepAsync(AuditLogger, policy.ThresholdValue.Value);
            DateTime oldestAllowedAuditLogTime = AuditLogCleaner.GetOldestAllowedTimestamp(policy.TimeWindow.Value);

            IQueryable<AuditLog> auditLogsQuery = AuditLogger.OnlyAuditLog
                .Where(log =>
                    log.TimeStamp < oldestRecordToKeep.TimeStamp || log.TimeStamp < oldestAllowedAuditLogTime);
            auditLogsQuery = AuditLogCleaner.ApplyExcludeTablesFilter(auditLogsQuery, policy.ExcludeTables);

            if(!await auditLogsQuery.AnyAsync())
                return;

            int totalLogsDeleted = await auditLogCleaner.CleanupAuditLogsAsync(CancellationToken.None);
            totalLogsDeleted.Should().BeGreaterThan(0);
        
            Assert.False(await auditLogsQuery.AnyAsync());
        }
    }
}