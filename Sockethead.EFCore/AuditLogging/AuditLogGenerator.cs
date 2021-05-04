using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Sockethead.EFCore.Entities;
using Sockethead.EFCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sockethead.EFCore.AuditLogging
{
    /// <summary>
    /// AuditLog generation, inspired by
    /// http://jmdority.wordpress.com/2011/07/20/using-entity-framework-4-1-dbcontext-change-tracking-for-audit-logging/
    /// With changes for .Net Core
    /// </summary>
    public class AuditLogGenerator
    {
        private ILogger<AuditLogGenerator> Logger { get; }

        public AuditLogGenerator(ILogger<AuditLogGenerator> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Generate a detailed Audit Logs of changes to tracked entities
        /// </summary>
        public IEnumerable<AuditLog> GenerateAuditLogs(IEnumerable<EntityEntry> dbEntries)
        {
            try
            {
                return dbEntries.Select(GenerateAuditLog).ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error Generating Audit Logs.");
                return new List<AuditLog>();
            }
        }

        private AuditLog GenerateAuditLog(EntityEntry dbEntry)
        {
            object orig = GetOriginalEntity(dbEntry);
            object curr = GetCurrentEntity(dbEntry);

            return new AuditLog
            {
                EntityState = dbEntry.State,
                TableName = dbEntry.GetTableName(),
                RecordId = dbEntry.GetPrimaryKeyValue(),
                TimeStamp = DateTime.UtcNow,
                AuditLogChanges = dbEntry
                    .CurrentValues
                    .Properties
                    .Where(prop => !prop.IsIndexerProperty())
                    .Select(prop => new AuditLogChange
                    {
                        Property = prop.Name,
                        Original = GetPropertyValue(orig, prop.Name),
                        Current = GetPropertyValue(curr, prop.Name),
                    })
                    .Where(change => change.Original != change.Current)
                    .ToList(),
            };
        }

        private static object GetOriginalEntity(EntityEntry dbEntry)
        {
            switch (dbEntry.State)
            {
                case EntityState.Modified:
                case EntityState.Deleted:
                    return dbEntry.OriginalValues.ToObject();

                default:
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Added:
                    return null;
            };
        }

        private static object GetCurrentEntity(EntityEntry dbEntry)
        {
            switch (dbEntry.State)
            {
                case EntityState.Modified:
                case EntityState.Added:
                    return dbEntry.CurrentValues.ToObject();

                default:
                case EntityState.Deleted:
                case EntityState.Detached:
                case EntityState.Unchanged:
                    return null;
            };
        }

        private string GetPropertyValue(object entity, string name)
        {
            try
            {
                if (entity == null)
                    return "";

                object o = entity
                    .GetType()
                    .GetProperty(name)
                    .GetValue(entity, null);

                return o == null ? "" : o.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error resolving property {name} in entity [{entity}].");
                return "";
            }
        }
    }
}
