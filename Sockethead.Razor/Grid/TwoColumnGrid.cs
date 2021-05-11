using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Sockethead.Razor.Css;
using Sockethead.Razor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Sockethead.Razor.Grid
{
    public enum TwoColumnGridStyle
    { 
        Table,
        DescriptionList,
    }

    public class TwoColumnGridOptions
    {
        public TwoColumnGridStyle TwoColumnGridStyle { get; set; } = TwoColumnGridStyle.Table;
        public bool AllowDuplicates { get; set; } = true;
    }

    public class TwoColumnGridOptionsCssOptions
    {
        public CssBuilder Table { get; } = new CssBuilder();
        public CssBuilder Row { get; } = new CssBuilder();
        public CssBuilder Label { get; } = new CssBuilder();
        public CssBuilder Item { get; } = new CssBuilder();

        public TwoColumnGridOptionsCssOptions()
        {
            SetDefaultCss();
        }

        public void SetDefaultCss()
        {
            Table
                .AddClass("table")
                .AddClass("table-sm")
                .AddStyle("table-layout:fixed;");

            Label
                .AddStyle("width: 140px;");

            Item
                .AddClass("wrapword");
        }

        public void ClearAll()
        {
            Table.Clear();
            Row.Clear();
            Label.Clear();
            Item.Clear();
        }
    }

    public class TwoColumnGridBuilder
    {
        private IHtmlHelper Html { get; }
        private TwoColumnGridOptions GridOptions { get; set; } = new TwoColumnGridOptions();
        private List<KeyValuePair<string, string>> Data { get; } = new List<KeyValuePair<string, string>>();
        private TwoColumnGridOptionsCssOptions CssOptions { get; } = new TwoColumnGridOptionsCssOptions();
        private Func<KeyValuePair<string, string>, KeyValuePair<string, string>> RowProcessor { get; set; }
        public TwoColumnGridBuilder(IHtmlHelper html)
        {
            Html = html;
        }

        private static string Encode(string s) => HttpUtility.HtmlEncode(s);

        /// <summary>
        /// Helper class to add rows via Expressions for a Model
        /// </summary>
        public class TwoColumnGridModelBuilder<TModel>
        {
            private TModel Model { get; }
            private TwoColumnGridBuilder GridBuilder { get; }

            internal TwoColumnGridModelBuilder(TModel model, TwoColumnGridBuilder gridBuilder)
            {
                Model = model;
                GridBuilder = gridBuilder;
            }

            /// <summary>
            /// Add a row to the TwoColumnGrid via an Expression
            /// </summary>
            public TwoColumnGridModelBuilder<TModel> Add(Expression<Func<TModel, object>> expression)
            {
                object value = expression.Compile().Invoke(Model);
                
                //if (GridBuilder.Html is IHtmlHelper<TModel> html)
                //    value = html.DisplayFor(expression).ToString() + " boom";

                GridBuilder.Add(expression.FriendlyName(), value == null ? "" : value.ToString());
                return this;
            }
        }

        /// <summary>
        /// Add all public properties from the Model into the TwoColumnGrid 
        /// </summary>
        public TwoColumnGridBuilder Add<T>(T model)
        {
            return Add(ExpressionHelpers.ModelToDictionary(model));
        }

        /// <summary>
        /// Add a Model to the TwoColumnGrid and specify exactly which properties to include via a builder callback
        /// </summary>
        public TwoColumnGridBuilder Add<TModel>(TModel model, Action<TwoColumnGridModelBuilder<TModel>> builderAction)
        {
            builderAction(new TwoColumnGridModelBuilder<TModel>(model, this));
            return this;
        }

        /// <summary>
        /// Add all KeyValuePairs in a dictionary to a TwoColumnGrid
        /// </summary>
        public TwoColumnGridBuilder Add<T>(Dictionary<string, T> dictionary)
        {
            foreach (var kvp in dictionary)
                Data.Add(new KeyValuePair<string, string>(kvp.Key, Encode(kvp.Value == null ? "" : kvp.Value.ToString())));
            return this;
        }

        /// <summary>
        /// Add a manually specified Label/Value pair to a TwoColumnGrid
        /// </summary>
        public TwoColumnGridBuilder Add(string label, string value, bool encode = true)
        {
            Data.Add(new KeyValuePair<string, string>(label, encode ? Encode(value) : value));
            return this;
        }

        /// <summary>
        /// Add a TwoColumnGrid inside a TwoColumnGrid
        /// </summary>
        public TwoColumnGridBuilder AddTwoColumnGrid(string label, Action<TwoColumnGridBuilder> gridBuilder)
        {
            var grid = Html.TwoColumnGrid();
            gridBuilder(grid);
            Add(label, grid.RenderToString(), encode: false);
            return this;
        }

        /// <summary>
        /// Add a SimpleGrid inside a TwoColumnGrid
        /// </summary>
        public TwoColumnGridBuilder AddSimpleGrid<T>(string label, IQueryable<T> model, Action<SimpleGrid<T>> gridBuilder) where T : class
        {
            var grid = Html.SimpleGrid(model);
            gridBuilder(grid);
            Add(label, grid.RenderToString(), encode: false);
            return this;
        }

        public TwoColumnGridBuilder AddRowProcessor(Func<KeyValuePair<string, string>, KeyValuePair<string, string>> processor)
        {
            RowProcessor = processor;
            return this;
        }

        public TwoColumnGridBuilder Options(Action<TwoColumnGridOptions> optionsSetter)
        {
            optionsSetter(GridOptions);
            return this;
        }

        public TwoColumnGridBuilder Css(Action<TwoColumnGridOptionsCssOptions> optionsSetter) 
        {
            optionsSetter(CssOptions);
            return this; 
        }

        private ViewDataDictionary BuildViewData()
            => new ViewDataDictionary(source: Html.ViewData)
            {
                { "Options", GridOptions },
                { "CssOptions", CssOptions },
            };

        private List<KeyValuePair<string, string>> BuildGridData()
        {
            var data = GridOptions.AllowDuplicates
                ? Data
                : Data
                    .GroupBy(kvp => kvp.Key)
                    .Select(x => new KeyValuePair<string, string>(x.Key, x.First().Value));

            if (RowProcessor != null)
                data = data
                    .Select(RowProcessor)
                    .Where(d => !string.IsNullOrEmpty(d.Key));

            return data.ToList();
        }

        public IHtmlContent Render()
            => Html.Partial(
                partialViewName: "_SHTwoColumnGrid",
                model: BuildGridData(),
                viewData: BuildViewData());

        public async Task<IHtmlContent> RenderAsync()
            => await Html.PartialAsync(
                partialViewName: "_SHTwoColumnGrid",
                model: BuildGridData(),
                viewData: BuildViewData());

        public string RenderToString()
            => Html.RenderPartialToString(
                partialViewName: "_SHTwoColumnGrid",
                model: BuildGridData(),
                viewData: BuildViewData());
    }
}
