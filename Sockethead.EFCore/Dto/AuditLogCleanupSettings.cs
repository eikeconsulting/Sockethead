using System;

namespace Sockethead.EFCore.Dto
{
    /// <summary>
    /// Provides settings for controlling the audit logs cleanup process.
    /// </summary>
    public class AuditLogCleanupSettings
    {
        /// <summary>
        /// The batch size parameter controls how many audit log records are deleted in each batch to avoid overwhelming
        /// the database with a large number of deletions at once.
        /// </summary>
        public uint BatchSize { get; set; } = 500;
        
        /// <summary>
        /// Specifies the time interval at which the cleanup service should be invoked. It can be set to any valid
        /// TimeSpan value, such as one hour or five minutes, to determine how frequently the service will run and purge
        /// old audit logs.
        /// </summary>
        public TimeSpan CleanupInterval { get; set; }
    }
}