using System.Collections.Generic;
using System.Linq;
using Sockethead.EFCore.Entities;

namespace Sockethead.EFCore.Dto
{
    /// <summary>
    /// Defines a policy for inserting audit logs into the AuditLog database. It has two properties: IncludeTables and
    /// SensitiveProperties. IncludeTables is an array of table names that specifies which logs should be included, while
    /// SensitiveProperties is an array of property names that should be excluded from the logs.
    /// </summary>
    public class AuditLogInsertionPolicy
    {
        /// <summary>
        /// This property is used to filter the list of AuditLog objects by table name. If IncludeTables is not null, only
        /// the logs whose TableName property matches one of the values in the IncludeTables array are allowed. If
        /// IncludeTables is null, all logs are allowed. If IncludeTables is an empty array, no logs are allowed.
        /// </summary>
        public string[] IncludeTables { get; set; }
        
        /// <summary>
        /// This property is used to filter the list of AuditLogChange objects within each AuditLog object by property name.
        /// If SensitiveProperties is not null, only the changes whose property matches the format "{TableName}.{Property}"
        /// and is not in the SensitiveProperties array are allowed. If SensitiveProperties is null, all changes are allowed.
        /// If a change is filtered out, it means that the corresponding property in the database is considered sensitive
        /// and should not be logged.
        /// </summary>
        public string[] SensitiveProperties {get; set;}

        public IList<AuditLog> ApplyPolicy(IList<AuditLog> logs)
        {
            // Filter logs by table name
            if (IncludeTables != null)
            {
                if (IncludeTables.Length == 0)
                    return new List<AuditLog>();

                logs = logs.Where(log => IncludeTables.Contains(log.TableName)).ToList();
            }
            
            
            if (SensitiveProperties == null) return logs;
            // Filter log changes by sensitive properties
            foreach (var log in logs)
            {
                List<AuditLogChange> changes = log.AuditLogChanges
                    .Where(c => !SensitiveProperties.Contains($"{log.TableName}.{c.Property}")).ToList();
                log.AuditLogChanges = changes;
            }

            return logs;
        }
    }
}