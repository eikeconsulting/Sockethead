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
        FormMethod Method = FormMethod.Post);
    
    public interface ISimpleForm
    {
        FormOptions FormOptions { get; }
        string CssClass { get; }
        IHtmlContent RenderFormRows();
    }
    
    public class SubmitButtonVm
    {
        public string Label { get; set; }
        public string Css { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
    }
    
    public class SimpleForm<T> : ISimpleForm where T : class
    {
        HtmlContentBuilder Builder = new HtmlContentBuilder();
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

        private void AddLabelFor<TResult>(Expression<Func<T, TResult>> expression, string cssClass = "control-label")
        {
            Append(Html.LabelFor(expression, Html.DisplayNameFor(expression),
                htmlAttributes: new { @class = cssClass }));
        }
        
        private void AddTextBoxFor<TResult>(Expression<Func<T, TResult>> expression, string type = null,
            string format = null, string cssClass = "form-control", bool isReadOnly = false, bool isDisabled = false)
        {
            Dictionary<string, object> htmlAttributes = new()
            {
                { "class", cssClass }
            };

            if (!string.IsNullOrEmpty(type))
                htmlAttributes.Add("type", type);
            
            if (isReadOnly)
                htmlAttributes.Add("readonly", "readonly");
            
            if (isDisabled)
                htmlAttributes.Add("disabled", "disabled");

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

        private void AddDefaultEditorFor<TResult>(Expression<Func<T, TResult>> expression, string type = null,
            bool isReadOnly = false, string format = null, bool isDisabled = false)
        {
            using IDisposable group = FormGroup();
            AddLabelFor(expression: expression);
            AddTextBoxFor(expression: expression, type: type, isReadOnly: isReadOnly, format: format,
                isDisabled: isDisabled);
            AddValidationMessageFor(expression: expression);
        }
        
        private void AddBooleanEditorFor<TResult>(Expression<Func<T, TResult>> expression, bool isDisabled = false)
        {
            using IDisposable group = FormGroup(additionalCssClass:"form-check");
            AddTextBoxFor(expression: expression, type: "checkbox", cssClass: "form-check-input",
                isDisabled: isDisabled);
            AddLabelFor(expression: expression, cssClass: "form-check-label");
            AddValidationMessageFor(expression: expression);
        }
        
        private void AddRadioEditorFor<TResult>(Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> items, bool inline = false, bool isDisabled = false)
        {
            Dictionary<string, object> htmlAttributes = new()
            {
                { "class", "form-check-input" }
            };
            
            if (isDisabled)
                htmlAttributes.Add("disabled", "disabled");
            
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
        
        private void AddEnumEditorFor<TResult>(Expression<Func<T, TResult>> expression)
        {
            List<TResult> values = Enum.GetValues(typeof(TResult)).Cast<TResult>().ToList();
            SelectList selectList = new SelectList(values);
            AddDropDownListEditorFor(expression: expression, selectList: selectList);
        }

        private void AddDropDownListEditorFor<TResult>(Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList)
        {
            using IDisposable group = FormGroup();
            AddLabelFor(expression: expression);
            Append(Html.DropDownListFor(expression, selectList, new { @class = "custom-select" }));
            AddValidationMessageFor(expression: expression);
        }
        
        private void AddDateEditorFor<TResult>(Expression<Func<T, TResult>> expression, bool isReadOnly = false,
            bool isDisabled = false)
        {
            if (expression.GetDataTypeAttribute() == DataType.Date)
            {
                AddDefaultEditorFor(expression: expression, type: "date", format: "{0:yyyy-MM-dd}", isReadOnly: isReadOnly,
                    isDisabled: isDisabled);
                return;
            }
            
            AddDefaultEditorFor(expression: expression, type: "datetime-local", format: "{0:yyyy-MM-ddTHH:mm}",
                isReadOnly: isReadOnly, isDisabled: isDisabled);
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
            switch (typeof(TResult).Name)
            {
                case nameof(Boolean):
                    AddBooleanEditorFor(expression: expression, isDisabled: isDisabled);
                    break;
                case nameof(DateTime):
                    AddDateEditorFor(expression);
                    break;
                case nameof(Double) or nameof(Decimal) or nameof(Single):
                    string format = expression.GetAttribute<DisplayFormatAttribute, T, TResult>()?.DataFormatString;
                    AddDefaultEditorFor(expression: expression, type: "number", format: format, isReadOnly: isReadOnly,
                        isDisabled: isDisabled);
                    break;
                default:
                    AddDefaultEditorFor(expression: expression, type: GetEditorType(expression.GetDataTypeAttribute()),
                        isReadOnly: isReadOnly, isDisabled: isDisabled);
                    break;
            }

            return this;
        }

        public SimpleForm<T> EnumEditorFor<TResult>(Expression<Func<T, TResult>> expression)
        {
            AddEnumEditorFor(expression: expression);
            return this;
        }
        
        public SimpleForm<T> SelectListEditorFor<TResult>(Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList)
        {
            AddDropDownListEditorFor(expression: expression, selectList);
            return this;
        }
        
        public SimpleForm<T> RadioEditorFor<TResult>(Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList, bool inline = false, bool isDisabled = false)
        {
            AddRadioEditorFor(expression: expression, selectList, inline, isDisabled: isDisabled);
            return this;
        }
        
        public SimpleForm<T> SubmitButton(string label = "Submit", string css = "btn-primary")
        {
            Append(Html.Partial("_SHFormSubmitButton", model: new SubmitButtonVm
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
