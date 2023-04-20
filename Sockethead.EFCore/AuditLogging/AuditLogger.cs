using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Sockethead.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

namespace Sockethead.EFCore.AuditLogging
{
    public class AuditLogger
    {
        public AuditLogGenerator AuditLogGenerator { get; }
        public AuditLogDbContext AuditLogDbContext { get; }
        public ILogger<AuditLogger> Logger { get; }

        public AuditLogger(
            AuditLogGenerator auditLogGenerator,
            AuditLogDbContext auditLogDbContext,
            ILogger<AuditLogger> logger)
        {
            AuditLogGenerator = auditLogGenerator;
            AuditLogDbContext = auditLogDbContext;
            Logger = logger;
        }
        
        public IQueryable<AuditLog> OnlyAuditLog
            => AuditLogDbContext
                .AuditLogs;

        public IQueryable<AuditLog> AuditLogQuery 
            => AuditLogDbContext
                .AuditLogs
                .Include(a => a.AuditLogChanges)
                .OrderByDescending(a => a.TimeStamp);

        public IQueryable<AuditLog> EntityLogs(string table, string recordId) 
            => AuditLogQuery
                .Where(al => al.TableName == table && al.RecordId == recordId);

        public IQueryable<AuditLog> EntityLogs<TTable>(string recordId) 
            => EntityLogs(table: typeof(TTable).Name, recordId: recordId);


        /// <summary>
        /// Performs the following steps:
        /// 1. Detect changes in the DbContext and generate AuditLogs
        /// 2. Save the data (i.e. call db.SaveChanges)
        /// 3. After saving, find newly created records and update AuditLogs
        /// 4. Commit new AuditLogs to the AuditLog database
        /// </summary>
        /// <param name="db">Target DbContext to inspect</param>
        /// <param name="meta">Additional metadata to apply to each AuditLog generated</param>
        /// <returns>Result the SaveChanges call on the db</returns>
        public async Task<int> SaveAndAuditAsync(DbContext db, IAuditMetaData meta = null)
        {
            db.ChangeTracker.DetectChanges();

            // pull initial set of tracked changes and generate Audit Logs
            var dbEntries = db.ChangeTracker.Entries()
                .Where(dbEntry => dbEntry.State != EntityState.Unchanged)
                .ToList();

            IList<AuditLog> auditLogs = GenerateLogs(dbEntries);

            // commit the changes (without the Audit Logs)
            int result = await db.SaveChangesAsync();

            if (!auditLogs.Any())
                return result;

            // RJE kludge: use the post save to match up records and set RecordId for added records
            var auditLogsPost = GenerateLogs(dbEntries);

            try
            {
                if (auditLogs.Count == auditLogsPost.Count)
                {
                    for (int i = 0; i < auditLogs.Count; i++)
                        if (auditLogs[i].Id == 0 && auditLogs[i].EntityState == EntityState.Added)
                            auditLogs[i].RecordId = auditLogsPost[i].RecordId;
                }

                // now commit the Audit Logs
                await AuditLogDbContext.AuditLogs.AddRangeAsync(auditLogs);
                await AuditLogDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // Exception in writing log must not affect normal flow.
                Logger.LogError(e, "Error Adding AuditLog");
            }

            return result;

            IList<AuditLog> GenerateLogs(IList<EntityEntry> dbEntries) 
                => AuditLogGenerator
                    .GenerateAuditLogs(dbEntries)
                    .Select(auditLog => 
                    {
                        auditLog.UserEmail = meta?.UserEmail;
                        auditLog.UserName = meta?.UserName;
                        return auditLog;
                    })
                    .ToList();
        }

        /// <summary>
        /// Performs the deletion of logs in batches of size specified. 
        /// </summary>
        /// <param name="auditLogsToDeleteQuery">Represents the audit logs to delete</param>
        /// <param name="batchSize">Performs the deletion of logs in batches of size specified</param>
        /// <param name="auditLogCleanupActionHandler">Performs the action on the logs that are being cleaned up. If null,
        /// then no action is performed.</param>
        /// <param name="stoppingToken">Allows cancellation of the operation</param>
        /// <returns>Returns the total number of logs deleted</returns>
        public async Task<int> DeleteAuditLogsInBatchesAsync(IQueryable<AuditLog> auditLogsToDeleteQuery, int batchSize,
            IAuditLogCleanupActionHandler auditLogCleanupActionHandler, CancellationToken stoppingToken)
        {
            auditLogsToDeleteQuery = auditLogsToDeleteQuery
                .Take(batchSize)
                .AsNoTracking();
            
            int totalLogsDeleted = 0;
            
            do
            {
                var auditLogsToDeleteList = await auditLogsToDeleteQuery.ToListAsync(cancellationToken: stoppingToken);

                if (auditLogCleanupActionHandler != null)
                    await auditLogCleanupActionHandler.DeletedLogsHandler(auditLogsToDeleteList);

                await AuditLogDbContext.BulkDeleteAsync(auditLogsToDeleteList, cancellationToken: stoppingToken);
                totalLogsDeleted += auditLogsToDeleteList.Count;
            }
            while (auditLogsToDeleteQuery.Any());
            
            return totalLogsDeleted;
        }
    }
}
