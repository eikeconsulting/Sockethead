using Sockethead.Web.Areas.Samples.ViewModels;
using System.Collections.Generic;

namespace Sockethead.Web.Areas.Samples.Utilities
{
    public class SimpleGridFeatures
    {
        public static List<Feature> Features { get; } = new()
        {
            new()
            {
                Name = "Installation",
                Description = "Just install the NuGet package and go!",
            },
            new()
            {
                Name = "Basic Usage",
                Description = "A minimal SimpleGrid.  This includes the Movie model for reference which is used in many of the other Samples as well.",
            },
            new()
            {
                Name = "Kitchen Sink",
                Description = "A sample SimpleGrid using many of the options available.",
            },
            new()
            {
                Name = "Column Selection",
                Description = "There are several ways to control which columns to include in your grid.",
            },
            new()
            {
                Name = "Order Columns",
                Description = "Order the columns based on the Display.Order Attribute",
            },
            new()
            {
                Name = "Column Headers",
                Description = "How to specify what text to render for column headers.",
            },
            new()
            {
                Name = "No Headers",
                Description = "Suppress column headers altogether.",
            },
            new()
            {
                Name = "Column Display",
                Description = "How to build the item render.",
            },
            new()
            {
                Name = "Row Numbers",
                Description = "Include a column with the column number.",
            },
            new()
            {
                Name = "Encoding",
                Description = "Disable encoding to embed raw HTML",
            },
            new()
            {
                Name = "Links",
                Description = "Create links in item render",
            },
            new()
            {
                Name = "Bullet List",
                Description = "Render an HTML bullet list",
            },
            new()
            {
                Name = "Enum",
                Description = "Render an enumerated value as an int, field name, or Display Name.",
            },
            new()
            {
                Name = "Date and Time",
                Description = "Handle local timezone DateTime.",
            },
            new()
            {
                Name = "Footer",
                Description = "Add a footer row to the Grid.",
            },
            new()
            {
                Name = "Embedding Grids",
                Description = "Embed TwoColumnGrids and SimpleGrids inside your grid.  How meta!",
            },
            new()
            {
                Name = "Sorting",
                Description = "Enable dynamic sorting on based on columns and specify the default sort order.",
            },
            new()
            {
                Name = "Pagination",
                Description = "Add a Pager to the grid for large data sets.",
            },
            new()
            {
                Name = "Search",
                Description = "Add custom search capabilities quickly and easily.",
            },
            new()
            {
                Name = "Multiple Grids",
                Description = "Provide a GridId if you want to paginate or search multiple grids on one page.",
            },
            new()
            {
                Name = "CSS",
                Description = "Apply CSS classes and styles to the table, header, and rows.",
            },
            new()
            {
                Name = "Row Modifier",
                Description = "Apply CSS on a row based on criteria.",
            },
            new()
            {
                Name = "Options",
                Description = "Set some grid options such as maximum rows and the No records message.  Also override partial view with your own.",
            },
            new()
            {
                Name = "AJAX",
                Description = "Use AJAX for paging and search in the grid.",
            },
            new()
            {
                Name = "Form",
                Url = "Form",
                Description = "Use a SimpleGrid as form.  This demonstrates checkbox handling.",
            },
            new()
            {
                Name = "Form2",
                Url = "Form2",
                Description = "Use a SimpleGrid as form.  This demonstrates editing models and receiving updated data in your controller.",
            },
            new()
            {
                Name = "Extensions",
                Description = "Create your own custom SimpleGrid extension!",
            },
        };
    }
}
