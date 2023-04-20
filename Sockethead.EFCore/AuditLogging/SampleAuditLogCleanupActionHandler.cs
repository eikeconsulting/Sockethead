using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Sockethead.EFCore.Entities;

namespace Sockethead.EFCore.AuditLogging
{
    public class SampleAuditLogCleanupActionHandler : IAuditLogCleanupActionHandler
    {
        public async Task DeletedLogsHandler(IEnumerable<AuditLog> logs)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuditLogs.txt");
            
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            
            FileMode fileMode = File.Exists(filePath) ? FileMode.Append : FileMode.CreateNew;
            await using StreamWriter writer = new StreamWriter(File.Open(filePath, fileMode));
            foreach (AuditLog auditLog in logs)
                await writer.WriteLineAsync(JsonSerializer.Serialize(auditLog, options));
        }
    }
}