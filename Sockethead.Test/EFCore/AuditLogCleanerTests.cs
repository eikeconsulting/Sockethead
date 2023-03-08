using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sockethead.EFCore.AuditLogging;
using Sockethead.Test.Common;
using Xunit;

namespace Sockethead.Test.EFCore
{
    public class AuditLogCleanerTests : IClassFixture<TestsFixture>
    {
        private TestsFixture Fixture { get; }

        public AuditLogCleanerTests(TestsFixture fixture)
        {
            Fixture = fixture;
        }

        [Fact]
        public async Task OldAuditLogsShouldBeDeleted()
        {
            AuditLogger auditLogger = Fixture.GetService<AuditLogger>();
            AuditLogCleaner auditLogCleaner = Fixture.GetService<AuditLogCleaner>();
        
            DateTime oldestAllowedAuditLogTime = DateTime.UtcNow.Subtract(auditLogCleaner.GetAuditLogAge());
            IQueryable<Sockethead.EFCore.Entities.AuditLog> auditLogsQuery = auditLogger.OnlyAuditLog
                .Where(log => log.TimeStamp < oldestAllowedAuditLogTime);
        
            if(!await auditLogsQuery.AnyAsync())
                return;

            int totalLogsDeleted = await auditLogCleaner.CleanupAuditLogsAsync(CancellationToken.None);
            totalLogsDeleted.Should().BeGreaterThan(0);
        
            Assert.False(await auditLogsQuery.AnyAsync());
        }
    }
}