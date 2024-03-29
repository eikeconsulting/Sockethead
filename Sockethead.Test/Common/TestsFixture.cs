﻿using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sockethead.EFCore.AuditLogging;
using Sockethead.Web.Data;

namespace Sockethead.Test.Common
{
    public class TestsFixture : IDisposable
    {
        private ServiceProvider ServiceProvider { get; }
    
        public T GetService<T>() => ServiceProvider.GetRequiredService<T>();

        public TestsFixture()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
                .Build();

            // ToDo: Move the below DI to a common place
            ServiceProvider = new ServiceCollection()
                .AddSingleton(config)
                .AddDbContext<AuditLogDbContext>(options =>
                    options.UseSqlServer(
                        config.GetConnectionString("AuditLogConnection")))
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        config.GetConnectionString("DefaultConnection")))
                .AddScoped<AuditLogGenerator>()
                .AddScoped<AuditLogger>()
                .AddScoped<MyRepo>()
                .AddLogging()
                .BuildServiceProvider();
        }
    
        public void Dispose()
        {
            ServiceProvider.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}