﻿using Sockethead.Razor.Helpers;
using System;
using System.Linq.Expressions;
using System.Web;

namespace Sockethead.Razor.Grid
{
    public interface ISimpleGridColumn
    {
        string Css();
        string DisplayRender(object model);

        SimpleGridColumnLabel LabelDetails { get; }
    }

    public class SimpleGridColumnLabel : SimpleGridBase
    {
        public string Display { get; set; }
        public string SortUrl { get; set; }
        public SortOrder CurrentSortOrder { get; set; }
    }

    public class Column<T> : SimpleGridBase, ISimpleGridColumn where T : class
    {
        internal Expression<Func<T, object>> Expression { get; set; }
        private Func<T, object> CompiledExpression { get; set; }
        internal Sort<T> Sort { get; set; } = new Sort<T>();
        private Func<T, object> DisplayBuilder { get; set; } = model => null;
        private Func<T, object> LinkBuilder { get; set; } = null;
        private string LinkTarget { get; set; }
        internal bool IsEncoded { get; set; } = true;
        private string LabelValue { get; set; } = null;

        public SimpleGridColumnLabel LabelDetails { get; } = new SimpleGridColumnLabel();

        internal string LabelRender() => HttpUtility.HtmlEncode(LabelValue);

        public string DisplayRender(object model)
        {
            object result = DisplayBuilder((T)model);
            string display = result == null ? "" : result.ToString();

            if (IsEncoded)
                display = HttpUtility.HtmlEncode(display);

            if (LinkBuilder != null)
                display = $"<a href='{LinkBuilder.Invoke((T)model)}' target='{LinkTarget}'>{display}</a>";

            return display;
        }

        public Column<T> Label(string label) 
        { 
            LabelValue = label; 
            return this; 
        }

        public Column<T> DisplayAs(Func<T, object> displayBuilder) 
        { 
            DisplayBuilder = displayBuilder; 
            return this; 
        }

        public Column<T> Encoded(bool isEncoded) 
        { 
            IsEncoded = isEncoded; 
            return this; 
        }

        public Column<T> LinkTo(Func<T, string> linkBuilder, string target = "_self") 
        { 
            LinkBuilder = linkBuilder; 
            LinkTarget = target; 
            return this; 
        }

        public Column<T> Sortable(bool enable = true, SortOrder sortOrder = SortOrder.Ascending)
        {
            Sort.IsEnabled = enable;

            if (enable && Expression == null)
                throw new ArgumentException("You must pass an Expression into sort if not already specified.");

            if (!enable)
                return this;

            return SortableBy(Expression, sortOrder);
        }

        public Column<T> SortableBy(Expression<Func<T, object>> expression, SortOrder sortOrder = SortOrder.Ascending)
        {
            Sort.IsEnabled = true;
            Sort.Expression = expression;
            Sort.SortOrder = sortOrder;
            return this;
        }

        public Column<T> For(Expression<Func<T, object>> expression)
        {
            Expression = expression;
            Label(expression.FriendlyName());
            CompiledExpression = expression.Compile();
            DisplayAs(model => CompiledExpression.Invoke(model));
            return this;
        }

        public Column<T> AddLabelCssClass(string cssClass)
        {
            LabelDetails.CssClasses.Add(cssClass);
            return this;
        }

        public Column<T> AddLabelCssStyle(string cssStyle)
        {
            LabelDetails.CssStyles.Add(cssStyle);
            return this;
        }

        public Column<T> AddItemCssClass(string cssClass)
        {
            CssClasses.Add(cssClass);
            return this;
        }

        public Column<T> AddItemCssStyle(string cssStyle)
        {
            CssStyles.Add(cssStyle);
            return this;
        }

    }
}