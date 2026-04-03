# Sockethead .NET Utilities

[![NuGet](https://img.shields.io/nuget/v/Sockethead.Razor?label=Sockethead.Razor)](https://www.nuget.org/packages/Sockethead.Razor)
[![NuGet](https://img.shields.io/nuget/v/Sockethead.EFCore?label=Sockethead.EFCore)](https://www.nuget.org/packages/Sockethead.EFCore)
[![NuGet](https://img.shields.io/nuget/v/Sockethead.Common?label=Sockethead.Common)](https://www.nuget.org/packages/Sockethead.Common)
[![llms.txt](https://img.shields.io/badge/llms.txt-available-blue)](https://sockethead.azurewebsites.net/llms.txt)

A collection of .NET utilities for ASP.NET Core. Multi-targets `net6.0`, `net8.0`, and `net10.0`.

**[Live Demo Site](https://sockethead.azurewebsites.net/)** | **[GitHub](https://github.com/eikeconsulting/Sockethead)**

## Features at a Glance

- **Sockethead.Razor** — SimpleGrid and TwoColumnGrid controls, alerts, client time rendering, PRG support, diagnostics
- **Sockethead.EFCore** — Entity Framework Core audit logging with configurable cleanup policies and soft delete
- **Sockethead.Common** — Extension methods for IQueryable, strings, collections, JSON, dates, and more

## Quick Start

```
PM> Install-Package Sockethead.Razor
```

Add to your `_ViewImports.cshtml`:
```csharp
@using Sockethead.Razor.Grid;
```

Render a grid in 3 lines:
```csharp
@(await Html
    .SimpleGrid(Model)
    .AddColumnsForModel()
    .RenderAsync())
```

See the [Demo Site](https://sockethead.azurewebsites.net/Samples/SimpleGrid/Dashboard) for 28 interactive examples covering sorting, paging, search, theming, CSS, AJAX, forms, and more.

---

## Sockethead.Razor

A library of Razor utilities for ASP.NET Core MVC.

### SimpleGrid

A full-featured grid control that renders `IQueryable<T>` as HTML tables. Supports column selection, sorting, pagination, search, CSS customization, row modifiers, embedded grids, AJAX, forms, and theming.

```csharp
@(await Html
    .SimpleGrid(Model)
    .Theme(GridTheme.Modern)
    .Sortable()
    .DefaultSortBy(m => m.Name)
    .AddColumn(col => col.For(m => m.Name).Sortable())
    .AddColumn(col => col.For(m => m.Director).Sortable())
    .AddColumnFor(m => m.Released)
    .AddSearch("Name", (source, query) =>
        source.Where(m => m.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))
    .AddPager(options =>
    {
        options.RowsPerPage = 10;
        options.DisplayTotal = true;
        options.RowsPerPageOptions = new[] { 5, 10, 20, 50 };
    })
    .RenderAsync())
```

### TwoColumnGrid

A two-column grid for rendering description lists (label-value pairs):

```csharp
@(Html.TwoColumnGrid().AddRowsForModel(movie).Render())
```

### Alerts

Add alerts to MVC pages from the controller:

```csharp
// In your _Layout.cshtml:
<partial name="_Alerts" />
```

```csharp
// In your Controller:
return View().Success("My cool success message!");
```

You may want to customize your Alert and create your own partial view, here is the source:

```html
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
```

### Client Time

An HTML helper that outputs DateTime as `<time>` tags for client-side timezone conversion. Include this JavaScript in your layout:

```html
<script type="text/javascript">
    $("time").each(function (elem) {
        var utctimeval = $(this).html();
        var date = new Date(utctimeval);
        $(this).html(date.toLocaleString());
    });
</script>
```

### RazorViewRenderer (for Email Generation)

Render a Razor view to a string, useful for constructing emails:

```csharp
// Register in DI:
services.AddScoped<IRazorViewRenderer, RazorViewRenderer>();
```

```csharp
// Use in your Controller:
public async Task<IActionResult> TestEmail([FromServices] IRazorViewRenderer renderer)
{
    return new ContentResult
    {
        ContentType = "text/html",
        StatusCode = (int)System.Net.HttpStatusCode.OK,
        Content = await renderer.RenderViewToStringAsync("Email/TestEmail", "Wow, this is cool"),
    };
}
```

### Post-Redirect-Get (PRG) Support

Attributes to serialize ModelState to TempData across redirects:

```csharp
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
```

Inspired by https://andrewlock.net/post-redirect-get-using-tempdata-in-asp-net-core/

### Diagnostics

- `Html.BuildTime` — returns a timestamp when the project was built for display in the page footer
- `[TimeTracker]` attribute — adds a StopWatch to the HTTP context for measuring controller execution time

---

## Sockethead.EFCore

```
PM> Install-Package Sockethead.EFCore
```

### Audit Logging

Track all database changes by capturing Entity Framework Core change tracker entries and committing them to separate audit log tables.

#### Setup

Add a connection string for the audit log database:

```json
"ConnectionStrings": {
    "DefaultConnection": "<your default database connection string>",
    "AuditLogConnection": "<your audit log connection string>"
}
```

I haven't tried using the same database for both, but I would recommend against it since the AuditLogs can potentially grow much faster.

Register the AuditLog context in DI:

```csharp
services.AddDbContext<AuditLogDbContext>(options =>
    options.UseSqlServer(
        Configuration.GetConnectionString("AuditLogConnection"),
        b => b.MigrationsAssembly("<your assembly namespace>")));
```

Note that you must replace `<your assembly namespace>` with the assembly where you want to create and run your migrations.

Add and run migrations:

```
add-migration -context AuditLogDbContext -name AuditLog -OutputDir "AuditLog/Migrations"
update-database -context AuditLogDbContext
```

#### Create a Repo Class

```csharp
public class MyRepo : AuditedRepo<ApplicationDbContext>
{
    public MyRepo(ApplicationDbContext db, AuditLogger auditLogger)
        : base(db, auditLogger)
    {
    }
}
```

Register your repo and audit log dependencies:

```csharp
services
    .AddScoped<AuditLogGenerator>()
    .AddScoped<AuditLogger>()
    .AddScoped<MyRepo>();
```

Now inject `MyRepo` instead of your `ApplicationDbContext` and call `CommitAsync` instead of `SaveChangesAsync`. Optionally pass `IAuditMetaData` to record the user, and `AuditLogInsertionPolicy` to control which tables and properties are logged:

```csharp
var policy = new AuditLogInsertionPolicy
{
    IncludeTables = new[] { "Table1", "Table2" },
    SensitiveProperties = new[] { "Table1.Property1", "Table2.Property2" }
};

await _myRepo.CommitAsync(auditMetaData, policy);
```

- `IncludeTables` — null = all tables, empty array = no tables
- `SensitiveProperties` — properties excluded from audit log entries

#### Audit Log Cleanup

A background service that periodically cleans up old audit logs:

```csharp
services.RegisterAuditLogCleanerBackgroundService(
    auditLogCleanupPolicy: new AuditLogCleanupPolicy
    {
        TimeWindow = TimeSpan.FromDays(30),
        ThresholdValue = null,
        ExcludeTables = new[] { "User" }
    },
    auditLogCleanupSettings: new AuditLogCleanupSettings
    {
        BatchSize = 500,
        CleanupInterval = TimeSpan.FromHours(1)
    },
    auditLogCleanupActionHandler: new SampleAuditLogCleanupActionHandler()
);
```

**AuditLogCleanupPolicy**

- `TimeWindow` — delete records older than this duration (compared to UTC). Default: 30 days.
- `ThresholdValue` — keep only the latest N records.
- If both are set, keeps the latest N records and purges anything older than the time window within that set.
- `ExcludeTables` — exempt specific tables from cleanup.

**AuditLogCleanupSettings**

- `BatchSize` — records deleted per batch (default: 500).
- `CleanupInterval` — how often the service runs (default: 1 hour).

**IAuditLogCleanupActionHandler**

Optionally implement this interface to perform actions on logs being deleted (e.g., archiving to a file). The `DeletedLogsHandler` method receives the list of logs about to be removed.

---

## Sockethead.Common

```
PM> Install-Package Sockethead.Common
```

A collection of common utilities and extension methods.

### Queryable Extensions

- `If` — conditionally transform a queryable
- `WhereIf` — conditionally apply a predicate
- `Paginate` — paginate by zero-indexed page number and page size
- `IgnoreQueryFiltersIf` — conditionally ignore query filters
- `ForEachInChunks` / `ForEachInChunksAsync` — process in batches
- `ChunkAsync` — break into smaller lists asynchronously
- `ForEachInChunksForShrinkingListAsync` — process a shrinking result set
- `ShrinkingListChunker` — process a queryable whose results change per chunk

### String Extensions

- `ToInt32OrDefault` — parse int with fallback
- `Truncate` — truncate to max length
- `StripAccentsFromUnicodeCharacters` — remove diacritics
- `ToEnum` — parse string to enum with default

### Collection Extensions

- `EmptyIfNull` — return empty collection if null

### JSON Extensions

- `ToJson` — serialize to indented camelCase JSON (Newtonsoft)
- `Deserialize` — deserialize JSON string to typed object

### Object Extensions

- `ToDictionary<T>` — convert object to dictionary
- `ToBase64` / `FromBase64` — Base64 encode/decode objects

### Dictionary Extensions

- `Accumulate` — accumulate values with a custom function
- `AccumulateSum` — accumulate decimal sums
- `ToObject` — convert dictionary to typed object

### Date Extensions

- `GetDateRange` — generate daily date range (inclusive)
- `GetMonthRange` — generate monthly date range
- `GetYearRange` — generate yearly date range

### Random Extensions

- `NextForTimes` — generate N random integers
- `UniqueNextForTimes` — generate N unique random integers

### Mail Address Collection Extensions

- `Add` — add a list of MailAddress to a collection
- `ParseAndAdd` — parse comma/semicolon-separated email string and add to collection

### Form File Extensions

- `ToBase64` — convert file content to Base64 string

### Date Utilities

- `GetDateValue` — convert day/month/year to integer (e.g., 20221002)
- `GetDay` / `GetMonth` / `GetYear` — extract components from integer date

### Random Utilities

- `GetRandomInstanceByDate` — seeded Random instance for deterministic sequences by date

---

## License

[MIT](https://opensource.org/licenses/MIT)
