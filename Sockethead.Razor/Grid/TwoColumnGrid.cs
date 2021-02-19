﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Sockethead.Razor.Css;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .AddClass("table-striped")
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

        public TwoColumnGridBuilder Add<T>(Dictionary<string, T> dictionary)
        {
            foreach (var kvp in dictionary)
                Data.Add(new KeyValuePair<string, string>(kvp.Key, kvp.Value.ToString()));
            return this;
        }

        public TwoColumnGridBuilder Add<T>(T model)
        {
            return Add(Helpers.ExpressionHelpers.ModelToDictionary(model));
        }

        public TwoColumnGridBuilder Add(string label, string value)
        {
            Data.Add(new KeyValuePair<string, string>(label, value));
            return this;
        }

        public TwoColumnGridBuilder Options(Action<TwoColumnGridOptions> action)
        {
            action(GridOptions);
            return this;
        }


        public TwoColumnGridBuilder Css(Action<TwoColumnGridOptionsCssOptions> cssOptionsSetter) 
        {
            cssOptionsSetter(CssOptions);
            return this; 
        }

        public async Task<IHtmlContent> RenderAsync()
        {
            var viewData = new ViewDataDictionary(source: Html.ViewData) 
            { 
                { "Options", GridOptions },
                { "CssOptions", CssOptions },
            };

            var data = GridOptions.AllowDuplicates
                ? Data
                : Data
                    .GroupBy(kvp => kvp.Key)
                    .Select(x => new KeyValuePair<string, string>(x.Key, x.First().Value))
                    .ToList();

            return await Html.PartialAsync(
                partialViewName: "_SHTwoColumnGrid",
                model: data,
                viewData: viewData);
        }
    }

}
