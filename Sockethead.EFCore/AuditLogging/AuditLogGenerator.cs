using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Sockethead.EFCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Sockethead.EFCore.AuditLogging
{
    /// <summary>
    /// AuditLog generation, with the help of
    /// http://jmdority.wordpress.com/2011/07/20/using-entity-framework-4-1-dbcontext-change-tracking-for-audit-logging/
    /// with changes for .net Core
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
                return dbEntries.Select(dbEntry => GenerateAuditLog(dbEntry));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error Generating Audit Logs");
                return new List<AuditLog>();
            }
        }

        private AuditLog GenerateAuditLog(EntityEntry dbEntry)
        {
            // Get the Table() attribute, if one exists
            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = GetTableName(dbEntry);

            // Get primary key value
            string recordId = GetPrimaryKeyValue(dbEntry);

            var changes = new List<AuditLogChange>();

            bool isOriginal = dbEntry.State == EntityState.Modified || dbEntry.State == EntityState.Deleted;
            bool isCurrent = dbEntry.State == EntityState.Modified || dbEntry.State == EntityState.Added;

            if (isOriginal || isCurrent)
            {
                IEnumerable<IProperty> propertyNames = isOriginal
                   ? dbEntry.OriginalValues.Properties
                   : dbEntry.CurrentValues.Properties;

                changes = (from propertyName in propertyNames
                           where !Equals(
                           isOriginal ? GetPropertyValue(dbEntry.OriginalValues.ToObject(), propertyName.Name) : "",
                           isCurrent ? GetPropertyValue(dbEntry.CurrentValues.ToObject(), propertyName.Name) : "")
                           select new AuditLogChange
                           {
                               Property = propertyName.Name,
                               Original = ToStringSafe(isOriginal ? GetPropertyValue(dbEntry.OriginalValues.ToObject(), propertyName.Name) : ""),
                               Current = ToStringSafe(isCurrent ? GetPropertyValue(dbEntry.CurrentValues.ToObject(), propertyName.Name) : ""),
                           })
                           .ToList();
            }

            return new AuditLog
            {
                EntityState = dbEntry.State,
                TableName = tableName,
                RecordId = recordId,
                AuditLogChanges = changes,
                TimeStamp = DateTime.UtcNow,
            };
        }

        public string GetTableName(EntityEntry dbEntry)
            // Get the Table() attribute, if one exists
            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            => dbEntry.Entity.GetType()
               .GetCustomAttributes(typeof(TableAttribute), false)
               .SingleOrDefault() is TableAttribute tableAttr
               ? tableAttr.Name
               : dbEntry.Entity.GetType().Name;

        public string GetPrimaryKeyValue(EntityEntry dbEntry)
        {
            try
            {
                IKey pk = dbEntry.Metadata.FindPrimaryKey();

                if (pk == null)
                    return "N/A";

                IEnumerable<object> values = pk.Properties.Select(p => dbEntry.Property(p.Name).CurrentValue);
                return string.Join("-", values.Select(v => v.ToString()));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retriving recordId through primary key");
                // swallow error
                return "[error]";
            }

        }

        private static string ToStringSafe(object s) => s?.ToString();

        private static object GetPropertyValue(object keyProperty, string name)
            => keyProperty
                .GetType()
                .GetProperty(name)
                .GetValue(keyProperty, null);
    }
}
