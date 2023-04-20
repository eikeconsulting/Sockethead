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
You can also pass an optional AuditLogInsertionPolicy object to specify which logs to include or exclude.

Here's an example of how to use the `AuditLogInsertionPolicy`:

    var policy = new AuditLogInsertionPolicy
    {
        IncludeTables = new[] { "Table1", "Table2" },
        SensitiveProperties = new[] { "Table1.Property1", "Table2.Property2" }
    };
    
    await _myRepo.CommitAsync(auditMetaData, policy);

The `IncludeTables` property is an array of table names that specifies which logs should be included. If IncludeTables is null, all logs are allowed. If IncludeTables is an empty array, no logs are allowed.

The `SensitiveProperties` property is an array of property names that should be excluded from the logs. If a change is filtered out, it means that the corresponding property in the database is considered sensitive and should not be logged.

#### Configuring Audit Logs Cleanup
The package provides a background service called `AuditLogCleaner` that can be used to periodically clean up old audit logs from the AuditLog database. The `RegisterAuditLogCleanerBackgroundService` extension method can be used to register the Audit Log Cleaner background service in the `IServiceCollection` interface. This extension method expects the following parameters:

- `AuditLogCleanupPolicy` - An optional parameter that specifies the policy to be used for cleaning up audit logs. If no policy is provided, the default policy will be used.
- `AuditLogCleanupSettings` - An optional parameter that specifies the settings to be used for cleaning up audit logs. If no settings are provided, the default settings will be used.
- `IAuditLogCleanupActionHandler` - An optional parameter that specifies an implementation of the `IAuditLogCleanupActionHandler` interface to perform the action on the logs that are being cleaned up. If no implementation is provided, then no action is performed.

More details about above parameters are added below.

In your Startup class, add the following code to register the `AuditLogCleaner` background service:


    services
        .RegisterAuditLogCleanerBackgroundService(
            auditLogCleanupPolicy: new AuditLogCleanupPolicy
            {
                TimeWindow = TimeSpan.FromDays(30),
                ThresholdValue = null,
                ExcludeTables = new []{"User"}
            },
            auditLogCleanupSettings: new AuditLogCleanupSettings
            {
                BatchSize = 500,
                CleanupInterval = TimeSpan.FromHours(1)
            },
            auditLogCleanupActionHandler: new SampleAuditLogCleanupActionHandler()
        );

The above code configures the `AuditLogCleaner` service based on specified cleanup policy, settings and logs action handler. You can customize the above parameters to suit your requirements.

**AuditLogCleanupPolicy**

Encapsulates the policy for cleaning up audit logs from the Audit Log database.

`TimeWindow` - This parameter serves the purpose of removing records that exceed a specified time window, with the comparison being made relative to UTC time. When assigned a value of TimeSpan.FromDays(30), any records that are more than 30 days old will be deleted. This is an optional parameter.

`ThresholdValue` - An optional parameter which determines the number of latest audit log records to retain.

If both the `TimeWindow` and `ThresholdValue` properties are set, the cleanup policy will consider both criteria when deleting audit logs. For instance, if the ThresholdValue is set to one million and the TimeWindow is set to 30 days, then this would keep the most recent one million records within the database, purging any records older than 30 days within that set of one million.

If both the `TimeWindow` and `ThresholdValue` properties are not set, then default policy would be used which uses Time Window of 30 days for the cleanup process.

`ExcludeTables` - Exclude specified tables from the cleanup process.

**AuditLogCleanupSettings**

Provides settings for controlling the audit logs cleanup process.

`BatchSize` - The batch size parameter controls how many audit log records are deleted in each batch to avoid overwhelming the database with a large number of deletions at once. It must be a positive integer. The default value for BatchSize is 500.

`CleanupInterval` - Specifies the time interval at which the cleanup service should be invoked. It can be set to any valid TimeSpan value, such as one hour or five minutes, to determine how frequently the service will run and purge old audit logs. The default value for CleanupInterval is one hour.

After configuring the service, it will automatically clean up old audit logs in the background based on specified cleanup policy and settings at the configured interval.

**IAuditLogCleanupActionHandler**

You can optionally provide an implementation of the `IAuditLogCleanupActionHandler` interface to perform additional actions on the logs that are being cleaned up, such as saving them to a text file. By default, no action is performed if no implementation is provided.

To create a custom implementation for performing additional actions on the logs being cleaned up, you can create a class that implements the `IAuditLogCleanupActionHandler` interface and define the desired logic within the `DeletedLogsHandler` method.

For instance, in the above code we have provided a sample implementation named `SampleAuditLogCleanupActionHandler`. The `DeletedLogsHandler` method of this class is called with the list of logs that are being cleaned up, which saves the logs to a text file.

#### AuditLogger TODOs
1. Audit Log UI!
1. Unit Tests
1. Integration Tests

## Sockethead.Common
Sockethead.Common is a collection of common utilities and extension methods that can be utilized in any project. It provides various useful utilities and extensions for string, JSON, collection, date, and more.

### Installation

    install-package Sockethead.Common

### Extensions

#### Queryable Extensions
* `If` - Transforms a queryable source based on a condition and returns the transformed source, otherwise returns the original source.
* `WhereIf` - Include "predicate" if "condition" is true.
* `Paginate` - Enables pagination of a queryable source by returning a specific number of elements based on a zero-indexed page number and a specified page size.
* `IgnoreQueryFiltersIf` - Applies the IgnoreQueryFilters feature if the provided condition is true, otherwise returns the original source.
* `ForEachInChunks` - This is equivalent to the standard ForEach extension, but divides the original collection into chunks rather than pulling it all in one query.
* `ForEachInChunksAsync` -  This is an asynchronous equivalent of the `ForEachInChunks` method, which divides the original collection into chunks rather than pulling it all in one query.
* `ChunkAsync` - Breaks the original collection into smaller "chunks" based on a specified chunk size, and return these chunks as a sequence of lists. This method allows for asynchronous processing of large collections, where pulling all the data into memory at once may not be feasible or efficient.
* `ForEachInChunksForShrinkingListAsync` - This allows you to loop over a list of items that shrinks the original query as you process it. It will terminate in one of two cases:
  - The query returns zero results .
  - The query returns the same or more results than the previous time through the loop.
* `ShrinkingListChunker` - Process a Queryable whose result set changes as you process each chunk.

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
* `ToDictionary<T>` - Convert the object to a Dictionary. Use only public properties of type T.
* `ToBase64` - Converts an object to its Base64-encoded string representation.
* `FromBase64` - Converts a Base64-encoded string to an object of type T.

#### Mail Address Collection Extensions
* `Add` - It takes a list of MailAddress and adds each MailAddress to the collection.
* `ParseAndAdd` - Parse a string (may contain one or multiple email addresses separated by comma or semicolon) and add to a MailAddressCollection.

#### Form File Extensions
* `ToBase64` - Converts the file content to its Base64-encoded string representation.

#### Dictionary Extensions
* `Accumulate` -  This method can be used to accumulate values for a key in a dictionary using a custom accumulator function. It adds the key and the new value to the dictionary if it doesn't exist, otherwise, it applies the accumulator function to the existing value and the new value.
* `AccumulateSum` - This method is a specific overload of Accumulate that is used to accumulate the sum of decimal values into a key in a dictionary.
* `ToObject` -  Converts a dictionary of string keys and object values to an instance of a class.

#### Date Extensions
* `GetDateRange` - Returns a range of DateTime that starts with the startDate and ends with the endDate, inclusive of both endpoints.
* `GetMonthRange` - Returns a range of DateTime objects that starts with startDate and ends with endDate, inclusive of both endpoints, incrementing by one month at a time.
* `GetYearRange` - Returns a range of DateTime objects that starts with startDate and ends with endDate, inclusive of both endpoints, incrementing by one year at a time.

### Utilities

#### Date Utils
* `GetDateValue` - Converts the specified day, month, and year to its equivalent integer value. For example, day:2/month:10/year:2022 would return 20221002.
* `GetDay` - Extracts the day information from the provided integer date value.
* `GetMonth` - Extracts the month information from the provided integer date value.
* `GetYear` - Extracts the year information from the provided integer date value.

#### Random Utils
* `GetRandomInstanceByDate` - Returns a new instance of the Random class, using the specified date as the seed value. The returned instance generates a same sequence of random numbers for a specified date.


# License
[MIT](https://opensource.org/licenses/MIT)
