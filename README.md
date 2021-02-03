# Sockethead .Net Core Utilities

Welcome to Sockethead.  This is a collection of .Net Core utilities build for .Net 5.

## Sockethead.EFCore

### AuditLogging

The AuditLogging utilities allow you to track all changes to a database by capturing changes in the 
data context and committing those changes to separate Audit Log tables.

It takes a few steps to integrate this into your project, but it's worth it!

### Installation

    install-package Sockethead.EFCore

#### Create Connection String
Add a Connection String for "AuditLogConnection" in your appsettings.json:

    "ConnectionStrings": {
        "DefaultConnection": "<your default database connection string>",
        "AuditLogConnection": "<your audit log connection string>"
    },

I haven't tried using the same database for both, but I would recommend against it since the AuditLogs can potentially grow much faster.

#### Register the AuditLog context (DI)
Then register Dependency Injection in your startup:

    services.AddDbContext<AuditLogDbContext>(options =>
        options.UseSqlServer(
            Configuration.GetConnectionString("AuditLogConnection"), 
            b => b.MigrationsAssembly("<your assembly namespace>")));

Note that you must replace "&lt;your assembly namespace&gt;" 
with the assembly where you want to create and run your migrations

#### Add and Run Migration
Add the AuditLog Migrations to your project.  You want to create the migrations as it may differ based on your database driver.

    add-migration -context AuditLogDbContext -name AuditLog -OutputDir "AuditLog/Migrations"

Follow up this with actually creating the database:

    update-database -context AuditLogDbContext

#### Derive your own Repo class

    public class MyRepo : AuditedRepo<ApplicationDbContext>
    {
        public MyRepo(ApplicationDbContext db, AuditLogger auditLogger)
            : base(db, auditLogger)
        {
        }
    }

#### Add your Repo DI
To add your MyRepo and other AuditLog dependencies to your startup:

    services
        .AddScoped<AuditLogGenerator>()
        .AddScoped<AuditLogger>()
        .AddScoped<MyRepo>();

Now, inject this MyRepo instead of your ApplicationDbContext 
and instead of calling "SaveChangesAsync" call "CommitAsync" and (optionally) pass 
an IAuditMetaData to provide the email and name of the user that made the change.

#### AuditLogger TODOs
1. We need a mechanism to flush out AuditLogs either by age or some filter.


# License
[MIT](https://opensource.org/licenses/MIT)
