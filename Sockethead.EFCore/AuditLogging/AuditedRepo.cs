using Microsoft.EntityFrameworkCore;
using Sockethead.EFCore.Entities;
using System.Linq;
using System.Threading.Tasks;
using Sockethead.EFCore.Dto;

namespace Sockethead.EFCore.AuditLogging
{
    public class AuditedRepo<TDbContext> where TDbContext : DbContext
    {
        public AuditedRepo(TDbContext db, AuditLogger auditLogger)
        {
            Db = db;
            AuditLogger = auditLogger;
        }

        public TDbContext Db { get; }
        public AuditLogger AuditLogger { get; }

        /// <summary>
        /// Will do the following:
        /// 1. Handle soft delete
        /// 2. Touch all Modified entities in the ChangeTracker so the Modified Timestamp is updated
        /// 3. Record the Audit Data in the AuditLogger Context
        /// 4. Commit (Save) the changes in the DbContext
        /// </summary>
        /// <param name="auditMetaData"></param>
        /// <param name="auditLogInsertionPolicy"></param>
        /// <returns></returns>
        public async Task CommitAsync(IAuditMetaData auditMetaData, AuditLogInsertionPolicy auditLogInsertionPolicy = null)
        {
            Db.ChangeTracker.DetectChanges();

            var entries = Db.ChangeTracker.Entries();

            // handle soft delete
            foreach (var item in entries.Where(x => x.State == EntityState.Deleted))
            {
                if (item.Entity is ISoftDeleteEntity entity)
                {
                    // Set the entity to unchanged (if we mark the whole entity as Modified, every field gets sent to Db as an update)
                    item.State = EntityState.Unchanged;

                    // Only update the IsDeleted flag - only this will get sent to the Db
                    entity.IsDeleted = true;
                }
            }

            // touch entities that are modified
            foreach (BaseEntity entity in entries
                .Where(dbEntry => dbEntry.State == EntityState.Modified)
                .Select(dbEntry => dbEntry.Entity)
                .OfType<BaseEntity>())
                entity.Touch();

            await AuditLogger.SaveAndAuditAsync(Db, auditMetaData, auditLogInsertionPolicy);
        }
    }
}
