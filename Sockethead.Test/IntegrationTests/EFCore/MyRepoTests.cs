using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sockethead.EFCore.AuditLogging;
using Sockethead.EFCore.Dto;
using Sockethead.EFCore.Entities;
using Sockethead.Test.Common;
using Sockethead.Web.Data;
using Xunit;

namespace Sockethead.Test.IntegrationTests.EFCore
{
    public class AuditedRepoTests : IClassFixture<TestsFixture>
    {
        private TestsFixture Fixture { get; }
        private MyRepo Repo { get; }
        private AuditLogger AuditLogger { get; }

        public AuditedRepoTests(TestsFixture fixture)
        {
            Fixture = fixture;
            Repo = Fixture.GetService<MyRepo>();
            AuditLogger = Fixture.GetService<AuditLogger>();
        }

        private static string Email => "unittest_01@sockethead.com";

        private static AuditLogInsertionPolicy GetAuditLogInsertionPolicy(string[] includeTables = null,
            string[] sensitiveProperties = null) => new()
        {
            IncludeTables = includeTables,
            SensitiveProperties = sensitiveProperties
        };

        private async Task<AuditLog> GetAuditLogAsync(string tableName, string recordId) =>
            await AuditLogger.EntityLogs(tableName, recordId).FirstOrDefaultAsync();

        private async Task<IdentityUser> AddSampleIdentityUserAsync()
        {
            IdentityUser user = new()
            {
                UserName = Email,
                Email = Email
            };
            
            await Repo.Db.Users.AddAsync(user);
            return user;
        }

        private async Task DeleteAddedUserAndAuditLogsAsync(IdentityUser user, AuditLogInsertionPolicy policy = null)
        {
            // Delete added user
            Repo.Db.Users.Remove(user);
            await Repo.CommitAsync(auditMetaData: null, policy);
            
            // Delete Audit log
            AuditLogger.AuditLogDbContext.RemoveRange(AuditLogger.EntityLogs(nameof(IdentityUser), user.Id));
            await AuditLogger.AuditLogDbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task AddUserRecord_DefaultAuditLogInsertionPolicy_ShouldAddUserAndAuditLogRecords()
        {
            IdentityUser user = await AddSampleIdentityUserAsync();
            
            await Repo.CommitAsync(auditMetaData: null); // Not provided AuditLogInsertionPolicy

            AuditLog auditLog = await GetAuditLogAsync(nameof(IdentityUser), user.Id);
            
            // Audit log must not be null
            Assert.NotNull(auditLog);

            await DeleteAddedUserAndAuditLogsAsync(user);
        }
        
        [Fact]
        public async Task AddUserRecord_EmptyAuditLogInsertionPolicy_ShouldNotAddAuditLogRecords()
        {
            IdentityUser user = await AddSampleIdentityUserAsync();

            // Insertion Policy with empty IncludeTables Array
            await Repo.CommitAsync(auditMetaData: null,
                GetAuditLogInsertionPolicy(includeTables: Array.Empty<string>()));

            AuditLog auditLog = await GetAuditLogAsync(nameof(IdentityUser), user.Id);
            
            // Audit log must not be added
            Assert.Null(auditLog);

            await DeleteAddedUserAndAuditLogsAsync(user);
        }
        
        [Fact]
        public async Task AddUserRecord_CustomAuditLogInsertionPolicy_ShouldNotLogSensitiveFields()
        {
            IdentityUser user = await AddSampleIdentityUserAsync();

            // Insertion Policy which includes sensitiveProperties
            await Repo.CommitAsync(auditMetaData: null,
                GetAuditLogInsertionPolicy(sensitiveProperties: new[]
                    { $"{nameof(IdentityUser)}.{nameof(IdentityUser.Email)}" }));

            AuditLog auditLog = await GetAuditLogAsync(nameof(IdentityUser), user.Id);
            
            // Audit log must be added
            Assert.NotNull(auditLog);

            // Email should not be recorded
            Assert.Null(auditLog.AuditLogChanges.FirstOrDefault(ac => ac.Property == nameof(IdentityUser.Email)));

            await DeleteAddedUserAndAuditLogsAsync(user);
        }
        
        [Fact]
        public async Task AddMultipleRecords_CustomAuditLogInsertionPolicy_ShouldBeFunctional()
        {
            // The AuditLogInsertionPolicy should solely record the User's data while excluding their email information.
            AuditLogInsertionPolicy policy = GetAuditLogInsertionPolicy(includeTables: new[] { nameof(IdentityUser) },
                sensitiveProperties: new[] { $"{nameof(IdentityUser)}.{nameof(IdentityUser.Email)}",
                    $"{nameof(IdentityUser)}.{nameof(IdentityUser.EmailConfirmed)}" });
            
            IdentityUser user = await AddSampleIdentityUserAsync();
            IdentityRole role = new IdentityRole("TestRole");
            await Repo.Db.Roles.AddAsync(role);
            
            await Repo.CommitAsync(auditMetaData: null, policy);

            AuditLog userAuditLog = await GetAuditLogAsync(nameof(IdentityUser), user.Id);
            // User Audit log must be added
            Assert.NotNull(userAuditLog);

            // Email should not be recorded
            Assert.Null(userAuditLog.AuditLogChanges.FirstOrDefault(ac => ac.Property == nameof(IdentityUser.Email)));

            AuditLog roleAuditLog = await GetAuditLogAsync(nameof(IdentityRole), role.Id);
            // Role Audit log must not be added
            Assert.Null(roleAuditLog);
            
            Repo.Db.Roles.Remove(role);
            await DeleteAddedUserAndAuditLogsAsync(user, policy);
        }
    }
}