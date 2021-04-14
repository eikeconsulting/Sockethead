using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Sockethead.Razor.Css;
using Sockethead.Razor.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
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
                //.AddClass("table-striped")
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

        public TwoColumnGridBuilder(IHtmlHelper html)
        {
            Html = html;
        }

        private static string Encode(string s) => HttpUtility.HtmlEncode(s);

        public TwoColumnGridBuilder Add<T>(Dictionary<string, T> dictionary)
        {
            foreach (var kvp in dictionary)
            {
                string value = kvp.Value == null
                    ? ""
                    : kvp.Value.ToString();
                Data.Add(new KeyValuePair<string, string>(kvp.Key, Encode(value)));
            }
            return this;
        }

        public TwoColumnGridBuilder Add<T>(T model)
        {
            return Add(ExpressionHelpers.ModelToDictionary(model));
        }

        public TwoColumnGridBuilder Add(string label, string value, bool encode = true)
        {
            if (encode)
                value = Encode(value);
            Data.Add(new KeyValuePair<string, string>(label, value));
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
            => GridOptions.AllowDuplicates
                ? Data
                : Data
                    .GroupBy(kvp => kvp.Key)
                    .Select(x => new KeyValuePair<string, string>(x.Key, x.First().Value))
                    .ToList();

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
