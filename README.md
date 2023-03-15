# Sockethead .Net Core Utilities

Welcome to Sockethead.  This is a collection of .Net Core utilities build for .Net 5.

## Sockethead.Razor
A library of Razor utilities.

See the [Sockethead Demo Site](https://sockethead.azurewebsites.net/) 
for usage examples and documentation of the SimpleGrid and TwoColumnGrid.

### Installation

    install-package Sockethead.Razor

### Alerts
Namespace: Sockethead.Razor.Alert.Extensions

This allows you to add an alert to an MVC page in the Controller.

Add the following to your _Layout.cshtml page:

    <partial name="_Alerts" />

Then the Extensions in your Controller:

    public IActionResult Index()
    {
        return View().Success("My cool success message!");
    }

You may want to customize your Alert and create your own partial view, here is the source:

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

### Client Time
Namespace: Sockethead.Razor.Helpers 

There is a HTMLHelper called "ClientTime" which 
will output a DateTime as a &lt;time&gt; html tag.  The following JavaScript 
should be included in the main layout page to make this render correctly:

    <script type="text/javascript">

        $("time").each(function (elem) {
            var utctimeval = $(this).html();
            var date = new Date(utctimeval);
            $(this).html(date.toLocaleString());
        });

    </script>


### TinyTable
TinyTable takes a Dictionary<string, object> and renders a pretty Bootstrap Table:

    <partial name="_TinyTable" model="<your dictionary>" />

### RazorViewRenderer (for Email Generation)
Namespace: Sockethead.Razor.Utilities

This is a utility function to render a Razor view to a string 
which is useful for constructing Emails to be sent by your website.

Register in your DI:

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


### Post Redirect Get (PRG) Support
Namespace: Sockethead.Razor.PRG 

There are two Attributes to support serializing the ModelState to 
TempData so that you can Redirect after a Post and maintain state.

    [HttpGet, RestoreModelState]
    public IActionResult SomeEndpoint(int id)
    ...

    [HttpPost, ValidateAntiForgeryToken, SaveModelState]
    public IActionResult SomeEndpoint(SomeModel someModel)
    {
        // always redirect here for both success and error
        var result = RedirectToAction(
                actionName: nameof(SomeEndpoint),
                routeValues: new { id = someModel.Id });

        if (!ModelState.IsValid)
            return result.Error("Error Saving Response.");

        ... handle form ...

        return result.Success("Successfully did it!");
    }


Inspired by https://andrewlock.net/post-redirect-get-using-tempdata-in-asp-net-core/


### Diagnostics
Namespace: Sockethead.Razor.Diagnostics

Html.BuildTime - returns a Timestamp when the project was built for display in 
the page footer or wherever you like.

TimeTracker Attribute
Apply to a Controller or Controller method and it adds a StopWatch to the 
HTTP context.  Then you can retrieve the page execution time 
(only the Controller part) in the footer.

   

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

#### Configuring Audit Logs Cleanup
The package provides a background service called `AuditLogCleaner` that can be used to periodically clean up old audit logs from the AuditLog database. You can configure the service by adding the following code to your Startup.cs file:


    services
        .AddHostedService(provider =>
            new AuditLogCleaner(
                provider.GetRequiredService<ILogger<AuditLogCleaner>>(),
                provider.GetRequiredService<IServiceScopeFactory>(),
                TimeSpan.FromHours(1), // run every hour
                TimeSpan.FromDays(30), // keep logs for up to 30 days
                500
            )
        );

The above code configures the `AuditLogCleaner` service to run every hour, delete logs that are older than 30 days, and delete up to 500 records at once. You can customize these settings to suit your requirements.

After configuring the service, it will automatically clean up old audit logs in the background at the configured interval.

#### AuditLogger TODOs
1. Audit Log UI!
1. Unit Tests
1. Integration Tests

## Sockethead.ExtensionsAndUtilities
Sockethead.ExtensionsAndUtilities is a collection of utilities and extension methods that can be utilized in any project. It provides various useful utilities and extensions for string, JSON, collection, date, and more.

### Installation

    install-package Sockethead.ExtensionsAndUtilities

### Extensions

#### Queryable Extensions
* `If` - Transforms a queryable source based on a condition and returns the transformed source, otherwise returns the original source.
* `WhereIf` - Include "predicate" if "condition" is true.
* `Paginate` - Enables pagination of a queryable source by returning a specific number of elements based on a zero-indexed page number and a specified page size.
* `IgnoreQueryFiltersIf` - Applies the IgnoreQueryFilters feature if the provided condition is true, otherwise returns the original source.

#### String Extensions
* `ToInt32OrDefault` - Attempts to convert the string to an int. If the conversion is successful, the method returns the converted int value. If the conversion fails, the method returns a default value specified by the caller.
* `Truncate` - Returns a truncated version of a given string up to a specified maximum length
* `StripAccentsFromUnicodeCharacters` - Removes accents from Unicode characters in a given string.
* `ToEnum` - Converts the string representation to specified Enum. Returns defaultValue if value was not converted successfully.

#### Collection Extensions
* `EmptyIfNull` - Returns empty collection if source is null

#### Random Extensions
* `NextForTimes` - Returns an IEnumerable of random integers up to a maximum value, with the number of integers determined by a specified count.
* `UniqueNextForTimes` - Returns an IEnumerable of unique random integers up to a maximum value, with the number of integers determined by a specified count.

#### Json Extensions
* `ToJson` - Converts an input object to its JSON representation using the Newtonsoft.Json library, with indentation and camel-cased property names. If the input object is null, the method returns null.
* `Deserialize` - Deserializes a JSON string input to an object of type T using the Newtonsoft.Json library. If the input string is null, the method returns the default value of type T.

#### Object Extensions
* `ToBase64` - Converts an object to its Base64-encoded string representation.
* `FromBase64` - Converts a Base64-encoded string to an object of type T.

### Utilities

#### Date Utils
* `GetDateRange` - Returns a range of DateTime that starts with the startDate and ends with the endDate, inclusive of both endpoints.




# License
[MIT](https://opensource.org/licenses/MIT)
