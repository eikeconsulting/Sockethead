using Microsoft.EntityFrameworkCore;
using Sockethead.EFCore.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Sockethead.EFCore.AuditLogging
{
    public class AuditedRepo<TDbContext> where TDbContext : DbContext
    {
        public AuditedRepo(TDbContext db, AuditLogger auditLogger)
        {
            Db = db;
            AuditLogger = auditLogger;
        }

        private TDbContext Db { get; }
        private AuditLogger AuditLogger { get; }

        /// <summary>
        /// Will do the following:
        /// 1. Touch all Modified entities in the ChangeTracker so the Modified Timestamp is updated
        /// 2. Record the Audit Data in the AuditLogger Context
        /// 3. Commit (Save) the changes in the DbContext
        /// </summary>
        /// <param name="auditMetaData"></param>
        /// <returns></returns>
        public async Task CommitAsync(IAuditMetaData auditMetaData)
        {
            foreach (BaseEntity entity in Db.ChangeTracker.Entries()
                .Where(dbEntry => dbEntry.State == EntityState.Modified)
                .Select(dbEntry => dbEntry.Entity)
                .OfType<BaseEntity>())
                entity.Touch();

            await AuditLogger.SaveAndAuditAsync(Db, auditMetaData);
        }
    }
}
