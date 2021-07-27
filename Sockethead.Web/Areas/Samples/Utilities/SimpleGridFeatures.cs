using Sockethead.Web.Areas.Samples.ViewModels;
using System.Collections.Generic;

namespace Sockethead.Web.Areas.Samples.Utilities
{
    public class SimpleGridFeatures
    {
        public static List<Feature> Features { get; } = new List<Feature>
        {
            new Feature
            {
                Name = "Installation",
                Description = "Just install the NuGet package and go!",
            },
            new Feature
            {
                Name = "Basic Usage",
                Description = "A minimial SimpleGrid.  This includes the Movie model for reference which is used in many of the other Samples as well.",
            },
            new Feature
            {
                Name = "Kitchen Sink",
                Description = "A sample SimpleGrid using many of the options available.",
            },
            new Feature
            {
                Name = "Column Selection",
                Description = "There are several ways to control which columns to include in your grid.",
            },
            new Feature
            {
                Name = "Order Columns",
                Description = "Order the columns based on the Display.Order Attribute",
            },
            new Feature
            {
                Name = "Column Headers",
                Description = "How to specify what text to render for column headers.",
            },
            new Feature
            {
                Name = "No Headers",
                Description = "Surpress column headers altogether.",
            },
            new Feature
            {
                Name = "Column Display",
                Description = "How to build the item render.",
            },
            new Feature
            {
                Name = "Row Numbers",
                Description = "Include a column with the column number.",
            },
            new Feature
            {
                Name = "Encoding",
                Description = "Disable encoding to embed raw HTML",
            },
            new Feature
            {
                Name = "Links",
                Description = "Create links in item render",
            },
            new Feature
            {
                Name = "Bullet List",
                Description = "Render an HTML bullet list",
            },
            new Feature
            {
                Name = "Enum",
                Description = "Render an enumerated value as an int, field name, or Display Name.",
            },
            new Feature
            {
                Name = "Date and Time",
                Description = "Handle local timezone DateTime.",
            },
            new Feature
            {
                Name = "Footer",
                Description = "Add a footer row to the Grid.",
            },
            new Feature
            {
                Name = "Embedding Grids",
                Description = "Embed TwoColumnGrids and SimpleGrids inside your grid.  How meta!",
            },
            new Feature
            {
                Name = "Sorting",
                Description = "Enable dynamic sorting on based on columns and specify the default sort order.",
            },
            new Feature
            {
                Name = "Pagination",
                Description = "Add a Pager to the grid for large data sets.",
            },
            new Feature
            {
                Name = "Search",
                Description = "Add custom search capabilities quickly and easily.",
            },
            new Feature
            {
                Name = "Multiple Grids",
                Description = "Provide a GridId if you want to paginate or search multiple grids on one page.",
            },
            new Feature
            {
                Name = "CSS",
                Description = "Apply CSS classes and styles to the table, header, and rows.",
            },
            new Feature
            {
                Name = "Row Modifier",
                Description = "Apply CSS on a row based on criteria.",
            },
            new Feature
            {
                Name = "Options",
                Description = "Set some grid options such as maximum rows and the No records message.  Also override partial view with your own.",
            },
            new Feature
            {
                Name = "AJAX",
                Description = "Use AJAX for paging and search in the grid.",
            },
            new Feature
            {
                Name = "Form",
                Url = "Form",
                Description = "Use a SimpleGrid as form.  This demonstrates checkbox handling.",
            },
            new Feature
            {
                Name = "Form2",
                Url = "Form2",
                Description = "Use a SimpleGrid as form.  This demonstrates editing models and recieving updated data in your controller.",
            },
            new Feature
            {
                Name = "Extensions",
                Description = "Create your own custom SimpleGrid extension!",
            },

            /*
            new Feature
            {
                Name = "Sample1",
                Model = "SampleData",
                Description = "Placeholder.  Working on dates and more complex forms.",
            },
            new Feature
            {
                Name = "Playground",
                Url = "Playground",
                Description = "A sample for me to play around in...",
            },
            */
        };
    }
}
