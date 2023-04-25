using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Sockethead.Razor.Helpers;

namespace Sockethead.Razor.Forms
{
    public static class SimpleFormExtensions
    {
        public static SimpleForm<T> SimpleForm<T>(this IHtmlHelper<T> html, T model, FormOptions options = default,
            string cssClass = "")
            where T : class => new(html: html, options: options, cssClass: cssClass);
    }

    public record FormOptions(string ActionName = null, string ControllerName = null,
        FormMethod FormMethod = FormMethod.Post);
    
    public interface ISimpleForm
    {
        FormOptions FormOptions { get; }
        string CssClass { get; }
        IHtmlContent RenderFormRows();
    }

    public class SimpleForm<T> : ISimpleForm where T : class
    {
        HtmlContentBuilder Builder = new();
        private IHtmlHelper<T> Html { get; }
        public FormOptions FormOptions { get; }
        public string CssClass { get; }

        public SimpleForm(IHtmlHelper<T> html, FormOptions options, string cssClass)
        {
            Html = html;
            FormOptions = options ?? new FormOptions();
            CssClass = cssClass;
        }

        public SimpleForm<T> HiddenFor<TResult>(Expression<Func<T, TResult>> expression)
        {
            Builder.AppendHtml(Html.HiddenFor(expression));
            return this;
        }

        private void Append(IHtmlContent htmlContent)
        {
            Builder.AppendHtml(htmlContent);
            Builder.AppendHtml("\n");
        }

        private void Append(string html)
        {
            Builder.AppendHtml(html);
            Builder.AppendHtml("\n");
        }

        private class Scope : IDisposable
        {
            Action OnDispose { get; }

            public Scope(Action onBegin, Action onEnd)
            {
                onBegin();
                OnDispose = onEnd;
            }

            public void Dispose() => OnDispose();
        }

        private IDisposable FormGroup(string additionalCssClass = "") => new Scope(
            onBegin: () => Append($"<div class='form-group {additionalCssClass}'>"), 
            onEnd: () => Append("</div>"));

        private static Dictionary<string, object> GetHtmlAttributes(HtmlAttributeOptions options)
        {
            Dictionary<string, object> htmlAttributes = new();
            
            if (!string.IsNullOrEmpty(options.CssClass))
                htmlAttributes.Add("class", options.CssClass);
            
            if (options.IsReadOnly)
                htmlAttributes.Add("readonly", "readonly");
            
            if (options.IsDisabled)
                htmlAttributes.Add("disabled", "disabled");
            
            if (!string.IsNullOrEmpty(options.Type))
                htmlAttributes.Add("type", options.Type);

            return htmlAttributes;
        }

        private void AddLabelFor<TResult>(Expression<Func<T, TResult>> expression, string cssClass = "control-label")
        {
            Append(Html.LabelFor(expression, Html.DisplayNameFor(expression),
                htmlAttributes: new { @class = cssClass }));
        }
        
        private void AddTextBoxFor<TResult>(Expression<Func<T, TResult>> expression,
            HtmlAttributeOptions htmlAttributeOptions, string format = null)
        {
            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(options: htmlAttributeOptions);
            
            if (expression.GetDataTypeAttribute() == DataType.MultilineText)
            {
                Append(Html.TextAreaFor(expression, htmlAttributes: htmlAttributes));
                return;
            }

            Append(Html.TextBoxFor(expression, format, htmlAttributes: htmlAttributes));
        }
        
        private void AddValidationMessageFor<TResult>(Expression<Func<T, TResult>> expression)
        {
            Append(Html.ValidationMessageFor(expression, null, htmlAttributes: new { @class = "text-danger" }));
        }

        private void AddDefaultEditorFor<TResult>(Expression<Func<T, TResult>> expression, 
            HtmlAttributeOptions htmlAttributeOptions, string format = null)
        {
            using IDisposable group = FormGroup();
            AddLabelFor(expression: expression);
            AddTextBoxFor(expression: expression, htmlAttributeOptions: htmlAttributeOptions, format: format);
            AddValidationMessageFor(expression: expression);
        }
        
        private void AddBooleanEditorFor(Expression<Func<T, bool>> expression, HtmlAttributeOptions htmlAttributeOptions)
        {
            htmlAttributeOptions.CssClass = "form-check-input";

            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(htmlAttributeOptions);

            using IDisposable group = FormGroup(additionalCssClass:"form-check");
            Append(Html.CheckBoxFor(expression, htmlAttributes: htmlAttributes));
            AddLabelFor(expression: expression, cssClass: "form-check-label");
            AddValidationMessageFor(expression: expression);
        }
        
        private void AddRadioEditorFor<TResult>(Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> items, HtmlAttributeOptions htmlAttributeOptions, bool inline = false)
        {
            htmlAttributeOptions.CssClass = "form-check-input";

            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(htmlAttributeOptions);
            
            foreach (SelectListItem item in items)
            {
                using IDisposable group =
                    FormGroup(additionalCssClass: $"form-check {(inline ? "form-check-inline" : "")}");
                Append(Html.RadioButtonFor(expression, item.Value, htmlAttributes: htmlAttributes));
                Append(Html.LabelFor(expression, item.Text, htmlAttributes: new { @class = "form-check-label" }));
            }
            AddValidationMessageFor(expression: expression);
            
            if(inline)
                Append("<br/>");
        }
        
        private void AddEnumEditorFor<TResult>(Expression<Func<T, TResult>> expression, 
            HtmlAttributeOptions htmlAttributeOptions)
        {
            List<TResult> values = Enum.GetValues(typeof(TResult)).Cast<TResult>().ToList();
            SelectList selectList = new SelectList(values);
            AddDropDownListEditorFor(expression: expression, selectList: selectList, htmlAttributeOptions);
        }

        private void AddDropDownListEditorFor<TResult>(Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList, HtmlAttributeOptions htmlAttributeOptions)
        {
            htmlAttributeOptions.CssClass = "custom-select";
            Dictionary<string, object> htmlAttributes = GetHtmlAttributes(htmlAttributeOptions);
            
            using IDisposable group = FormGroup();
            AddLabelFor(expression: expression);
            Append(Html.DropDownListFor(expression, selectList, htmlAttributes: htmlAttributes));
            AddValidationMessageFor(expression: expression);
        }
        
        private void AddDateEditorFor<TResult>(Expression<Func<T, TResult>> expression,
            HtmlAttributeOptions htmlAttributeOptions)
        {
            bool isDateOnly = expression.GetDataTypeAttribute() == DataType.Date;
            htmlAttributeOptions.Type = isDateOnly ? "date" : "datetime-local";
            AddDefaultEditorFor(expression: expression, htmlAttributeOptions: htmlAttributeOptions,
                format: isDateOnly ? "{0:yyyy-MM-dd}" : "{0:yyyy-MM-ddTHH:mm}");
        }
        
        private static string GetEditorType(DataType? dataType) => dataType switch
        {
            DataType.Password => "password",
            DataType.EmailAddress => "email",
            _ => "text"
        };
        
        public SimpleForm<T> EditorFor<TResult>(Expression<Func<T, TResult>> expression, bool isReadOnly = false,
            bool isDisabled = false)
        {
            HtmlAttributeOptions options = new HtmlAttributeOptions(isDisabled: isDisabled, isReadOnly: isReadOnly);

            switch (typeof(TResult).Name)
            {
                case nameof(DateTime):
                    AddDateEditorFor(expression, options);
                    break;
                case nameof(Double) or nameof(Decimal) or nameof(Single):
                    string format = expression.GetAttribute<DisplayFormatAttribute, T, TResult>()?.DataFormatString;
                    options.Type = "number";
                    AddDefaultEditorFor(expression: expression, htmlAttributeOptions: options, format: format);
                    break;
                default:
                    options.Type = GetEditorType(expression.GetDataTypeAttribute());
                    AddDefaultEditorFor(expression: expression, htmlAttributeOptions: options);
                    break;
            }

            return this;
        }

        public SimpleForm<T> EnumEditorFor<TResult>(Expression<Func<T, TResult>> expression, bool isDisabled = false)
        {
            HtmlAttributeOptions options = new HtmlAttributeOptions(isDisabled: isDisabled);
            AddEnumEditorFor(expression: expression, htmlAttributeOptions: options);
            return this;
        }
        
        public SimpleForm<T> SelectListEditorFor<TResult>(Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList, bool isDisabled = false)
        {
            HtmlAttributeOptions options = new HtmlAttributeOptions(isDisabled: isDisabled);
            AddDropDownListEditorFor(expression: expression, selectList, htmlAttributeOptions: options);
            return this;
        }
        
        public SimpleForm<T> RadioButtonEditorFor<TResult>(Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList, bool inline = false, bool isDisabled = false)
        {
            HtmlAttributeOptions options = new HtmlAttributeOptions(isDisabled: isDisabled);
            AddRadioEditorFor(expression: expression, selectList, htmlAttributeOptions: options, inline);
            return this;
        }
        
        public SimpleForm<T> CheckBoxEditorFor(Expression<Func<T, bool>> expression, bool isDisabled = false)
        {
            HtmlAttributeOptions options = new HtmlAttributeOptions(isDisabled: isDisabled);
            AddBooleanEditorFor(expression: expression, htmlAttributeOptions: options);
            return this;
        }
        
        public SimpleForm<T> SubmitButton(string label = "Submit", string css = "btn-primary")
        {
            Append(Html.Partial("_SHFormSubmitButton", model: new SubmitButton
            {
                Label = label,
                Action = FormOptions.ActionName,
                Controller = FormOptions.ControllerName,
                Css = css,
            }));
            return this;
        }
        
        public SimpleForm<T> AppendHtml(Func<object, IHtmlContent> contentFunc)
        {
            Append(contentFunc(null));
            return this;
        }

        public SimpleForm<T> AppendHtmlIf(bool condition, Func<object, IHtmlContent> contentFunc)
        {
            if (condition)
                Append(contentFunc(null));
            return this;
        }
        
        public SimpleForm<T> AppendHtml(IHtmlContent content)
        {
            Append(content);
            return this;
        }
        
        public SimpleForm<T> AppendHtml(string encoded)
        {
            Append(encoded);
            return this;
        }

        public async Task<IHtmlContent> RenderAsync()
        {
            return await Html.PartialAsync("_SHSimpleForm", this);
        }

        public IHtmlContent RenderFormRows()
        {
            try
            {
                return Builder;
            }
            finally
            {
                Builder = new HtmlContentBuilder();
            }
        }
    }
}