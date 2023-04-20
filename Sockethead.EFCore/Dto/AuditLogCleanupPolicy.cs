using System;

namespace Sockethead.EFCore.Dto
{
    /// <summary>
    /// Encapsulates the policy for cleaning up audit logs from the Audit Log database. It includes a TimeWindow property which
    /// specifies the duration for which audit logs are retained, a ThresholdValue property that determines the number of
    /// latest audit log records to retain, and an ExcludeTables property that allows specified tables to be excluded from
    /// the cleanup process.
    /// If both the TimeWindow and ThresholdValue properties are set, the cleanup policy will consider both criteria when
    /// deleting audit logs. For instance, if the ThresholdValue is set to one million and the TimeWindow is set to
    /// 30 days, then this would keep the most recent one million records within the database, purging any records older
    /// than 30 days within that set of one million.
    /// </summary>
    public class AuditLogCleanupPolicy
    {
        /// <summary>
        /// The purpose of this parameter is to delete records that are older than this time window.
        /// </summary>
        public TimeSpan? TimeWindow { get; set; }
        
        /// <summary>
        /// Determines the number of latest audit log records to retain
        /// </summary>
        public int? ThresholdValue { get; set; }
        
        /// <summary>
        /// Exclude specified tables from the cleanup process.
        /// </summary>
        public string[] ExcludeTables { get; set; }
    }
}