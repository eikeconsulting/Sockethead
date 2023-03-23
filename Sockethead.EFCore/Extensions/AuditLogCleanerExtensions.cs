using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sockethead.EFCore.AuditLogging;
using Sockethead.EFCore.Dto;

namespace Sockethead.EFCore.Extensions
{
    public static class AuditLogCleanerExtensions
    {
        /// <summary>
        /// Adds a hosted service to the service collection that will perform audit logs cleanup process based on a
        /// provided policy and settings.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="auditLogCleanupPolicy">Encapsulates the policy for cleaning up audit logs from the Audit Log
        /// database. If null, then default policy would be used.</param>
        /// <param name="auditLogCleanupSettings">Provides settings for controlling the audit logs cleanup process.
        /// If null, then default settings are used.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterAuditLogCleanerBackgroundService(this IServiceCollection services,
            AuditLogCleanupPolicy auditLogCleanupPolicy = null, AuditLogCleanupSettings auditLogCleanupSettings = null)
        {
            return services
                .AddHostedService(provider =>
                    new AuditLogCleaner(
                        provider.GetRequiredService<ILogger<AuditLogCleaner>>(),
                        provider.GetRequiredService<IServiceScopeFactory>(),
                        auditLogCleanupPolicy,
                        auditLogCleanupSettings
                    )
                );
        }
    }
}