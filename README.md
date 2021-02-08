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

#### Add and Run Migrations
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
1. Audit Log UI!
1. Unit Tests
1. Integration Tests
1. Mechanism to flush out AuditLogs either by age or a filter.



## Sockethead.Razor
A library of Razor utilities.

### Installation

    install-package Sockethead.Razor

### Alerts
This allows you to add an alert to an MVC page in the Controller.

Add the following to your _Layout.cshtml page:

    <partial name="_Alerts" />

Then the Extensions in your Controller:

    using Sockethead.Razor.Alert.Extensions;

    public IActionResult Index()
    {
        return View().Success("My cool success message!");
    }

You may want to customize your Alert and create your own partial view, here is the source:

    @using Sockethead.Razor.Alert.Extensions

    @foreach (string key in Alerts.ALL)
    {
        if (TempData.ContainsKey(key))
        {
            <div class="alert alert-@key">
                <button type="button" class="close" data-dismiss="alert">x</button>
                @Html.Raw(TempData[key])
            </div>
        }
    }
    @if (ViewData.ModelState.Any(x => x.Value.Errors.Any()))
    {
        <div class="alert alert-error">
            <a class="close" data-dismiss="alert">&times;</a>
            <h5>Oops!  We had a problem processing the form:</h5>
            @Html.ValidationSummary(false)
        </div>
    }

### TinyTable
TinyTable takes a Dictionary<string, object> and renders a pretty Bootstrap Table:

    <partial name="_TinyTable" model="<your dictionary>" />

### RazorViewRenderer (for Email Generation)
This is a utility function to render a Razor view to a string 
which is useful for constructing Emails to be sent by your website.

Register in your DI:

    using Sockethead.Razor.Utilities;
    ...
    services.AddScoped<IRazorViewRenderer, RazorViewRenderer>();

Then inject and use in your Controller:

    public async Task<IActionResult> TestEmail([FromServices] IRazorViewRenderer renderer)
    {
        return new ContentResult
        {
            ContentType = "text/html",
            StatusCode = (int)System.Net.HttpStatusCode.OK,
            Content = await renderer.RenderViewToStringAsync("Email/TestEmail", "Wow, this is cool"),
        };
    }



   


# License
[MIT](https://opensource.org/licenses/MIT)
