using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sockethead.EFCore.AuditLogging;
using Sockethead.Web.Data;

namespace Sockethead.Test.Common
{
    public class TestsFixture : IDisposable
    {
        private ServiceProvider ServiceProvider { get; }
        private SqliteConnection AuditLogConnection { get; }
        private SqliteConnection DefaultConnection { get; }

        public T GetService<T>() => ServiceProvider.GetRequiredService<T>();

        public TestsFixture()
        {
            // SQLite in-memory databases require the connection to stay open
            AuditLogConnection = new SqliteConnection("DataSource=:memory:");
            AuditLogConnection.Open();

            DefaultConnection = new SqliteConnection("DataSource=:memory:");
            DefaultConnection.Open();

            ServiceProvider = new ServiceCollection()
                .AddDbContext<AuditLogDbContext>(options =>
                    options.UseSqlite(AuditLogConnection))
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(DefaultConnection))
                .AddScoped<AuditLogGenerator>()
                .AddScoped<AuditLogger>()
                .AddScoped<MyRepo>()
                .AddLogging()
                .BuildServiceProvider();

            // Create the schemas
            using var scope = ServiceProvider.CreateScope();
            scope.ServiceProvider.GetRequiredService<AuditLogDbContext>().Database.EnsureCreated();
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
        }

        public void Dispose()
        {
            ServiceProvider.Dispose();
            AuditLogConnection.Dispose();
            DefaultConnection.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
