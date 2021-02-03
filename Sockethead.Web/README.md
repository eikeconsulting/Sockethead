# Sockethead .Net Core Utilities

Welcome to Sockethead.  This is a collection of .Net Core utilities build for .Net 5.


## Sockethead.EFCore

### Installation

install-package Sockethead.EFCore -version 0.0.1-alpha

### AuditLogging

The AuditLogging utilities allow you to track all changes to a database by capturing changes in the 
data context and committing those changes to separate Audit Log tables.

#### Create Connection String
To add Audit Logging to your project add a Connection String for "AuditLogConnection" in your appsettings.json:

    "ConnectionStrings": {
        "DefaultConnection": "<your default database connection string>",
        "AuditLogConnection": "<your audit log connection string>"
    },

#### Register the AuditLog context (DI)
Then register Dependency Injection in your startup:

    services.AddDbContext<Sockethead.EFCore.AuditLogging.AuditLogDbContext>(options =>
        options.UseSqlServer(
            Configuration.GetConnectionString("AuditLogConnection"), 
            b => b.MigrationsAssembly("<your assembly namespace>")));

Note that you must replace "&lt;your assembly namespace&gt;" 
with the assembly where you want to create and run your migrations

#### Add and Run Migration

    add-migration -context AuditLogDbContext -name AuditLog -OutputDir "AuditLog/Migrations"

Followed by

    update-database -Context AuditLogDbContext

#### Add AuditedRepo (DI)
Add the AuditRepo to your starup DI setup:

    services.AddScoped<Sockethead.EFCore.AuditLogging.AuditedRepo<ApplicationDbContext>>();

Now, inject this Repo instead of your own DbContext.  Note that you still have access to the context through the property "Db" in this Repo

Instead of calling "SaveChangesAsync" call "CommitAsync" and (optionally) pass an IAuditMetaData to provide the email and name of the user that made the change.


# License
[MIT](https://opensource.org/licenses/MIT)
