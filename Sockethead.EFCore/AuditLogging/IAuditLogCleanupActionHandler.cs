using System.Collections.Generic;
using System.Threading.Tasks;
using Sockethead.EFCore.Entities;

namespace Sockethead.EFCore.AuditLogging
{
    /// <summary>
    /// Performs the action on the logs that are being cleaned up.
    /// </summary>
    public interface IAuditLogCleanupActionHandler
    {
        /// <summary>
        /// A function that handles the logs that are going to be deleted.
        /// It can perform any action on the logs, such as saving them to a text file.
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        Task DeletedLogsHandler(IEnumerable<AuditLog> logs);
    }
}