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
    public class SimpleForm<T> : ISimpleForm where T : class
    {
        private HtmlContentBuilder Builder = new();
        private IHtmlHelper<T> Html { get; }
        public FormOptions FormOptions { get; }
        
        public SimpleForm(IHtmlHelper<T> html, Action<FormOptions> optionsAction = null)
        {
            Html = html;
            FormOptions = new FormOptions();
            optionsAction?.Invoke(FormOptions);
        }

        public SimpleForm<T> AddRowFor<TResult>(Expression<Func<T, TResult>> expression, Action<HtmlAttributeOptions> optionsSetter = null)
        {
            HtmlAttributeOptions options = ResolveOptions(optionsSetter);
            string format = null;
            
            switch (typeof(TResult).Name)
            {
                case nameof(DateTime):
                    if (expression.GetDataTypeAttribute() == DataType.Date)
                    {
                        options.Type = "date";
                        format = "{0:yyyy-MM-dd}";
                    }
                    else
                    {
                        options.Type = "datetime-local";
                        format = "{0:yyyy-MM-ddTHH:mm}";
                    }
                    break;

                case nameof(Double) or nameof(Decimal) or nameof(Single):
                    format = expression.GetAttribute<DisplayFormatAttribute, T, TResult>()?.DataFormatString;
                    options.Type = "number";
                    break;

                default:
                    options.Type = GetEditorType(expression.GetDataTypeAttribute());
                    break;
            }

            Dictionary<string, object> htmlAttributes = options.ToDictionary();

            using IDisposable group = CreateFormGroup();
            AddLabelFor(expression: expression);
            
            switch (expression.GetDataTypeAttribute())
            {
                case DataType.MultilineText: 
                    AppendHtml(Html.TextAreaFor(expression, htmlAttributes: htmlAttributes));
                    break;
                default:
                    AppendHtml(Html.TextBoxFor(expression, format, htmlAttributes: htmlAttributes));
                    break;
            }
            
            return AddValidationMessageFor(expression: expression);
        }

        public SimpleForm<T> AddHiddenRowFor<TResult>(Expression<Func<T, TResult>> expression)
        {
            return AppendHtml(Html.HiddenFor(expression));
        }
        
        public SimpleForm<T> AddEnumRowFor<TResult>(
            Expression<Func<T, TResult>> expression, 
            Action<HtmlAttributeOptions> optionsSetter = null)
        {
            List<TResult> values = Enum
                .GetValues(typeof(TResult))
                .Cast<TResult>()
                .ToList();
            
            return AddSelectListRowFor(
                expression: expression, 
                selectList: new SelectList(values), 
                optionsSetter: optionsSetter);
        }

        public SimpleForm<T> AddSelectListRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList, 
            Action<HtmlAttributeOptions> optionsSetter = null)
        {
            HtmlAttributeOptions options = ResolveOptions(optionsSetter);
            options.CssClass = "custom-select";
            using IDisposable group = CreateFormGroup();
            AddLabelFor(expression: expression);
            AppendHtml(Html.DropDownListFor(expression, selectList, htmlAttributes: options.ToDictionary()));
            AddValidationMessageFor(expression: expression);
            return this;
        }
        
        public SimpleForm<T> AddRadioButtonRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList, 
            bool inline = false, 
            bool isDisabled = false)
        {
            HtmlAttributeOptions options = new(isDisabled: isDisabled);
            AddRadioEditorFor(expression: expression, selectList, htmlAttributeOptions: options, inline);
            return this;
        }
        
        public SimpleForm<T> AddCheckBoxRowFor(Expression<Func<T, bool>> expression, bool isDisabled = false)
        {
            HtmlAttributeOptions options = new(isDisabled: isDisabled);
            options.CssClass = $"form-check-input {options.CssClass}";

            Dictionary<string, object> htmlAttributes = options.ToDictionary();

            using IDisposable group = CreateFormGroup(additionalCssClass:"form-check");
            AppendHtml(Html.CheckBoxFor(expression, htmlAttributes: htmlAttributes));
            AddLabelFor(expression: expression, cssClass: "form-check-label");
            AddValidationMessageFor(expression: expression);
            return this;
        }
        
        public SimpleForm<T> AddFileUploadRowFor<TResult>(Expression<Func<T, TResult>> expression,
            bool multiple = false, string accept = "", bool isDisabled = false)
        {
            HtmlAttributeOptions options = new(isDisabled: isDisabled);
            AddFileUploadEditorFor(expression: expression, htmlAttributeOptions: options, multiple: multiple, accept: accept);
            return this;
        }
        
        public SimpleForm<T> AddSubmitButtonRow(string label = "Submit", string css = "btn-primary")
        {
            return AppendHtml(Html.Partial("_SHFormSubmitButton", model: new SubmitButton
            {
                Label = label,
                Action = FormOptions.ActionName,
                Controller = FormOptions.ControllerName,
                Css = css,
            }));
        }

        /// <summary>
        /// Build form from the model via Reflection
        /// </summary>
        public SimpleForm<T> AddRowsForModel()
        {
            SimpleFormMagic<T> magic = new(this);
            magic.AddRowsForModel();
            return this;
        }
        
        public SimpleForm<T> AppendHtml(Func<object, IHtmlContent> contentFunc)
        {
            return AppendHtml(contentFunc(null));
        }

        public SimpleForm<T> AppendHtmlIf(bool condition, Func<object, IHtmlContent> contentFunc)
        {
            return condition ? AppendHtml(contentFunc(null)) : this;
        }
        
        public SimpleForm<T> AppendHtml(IHtmlContent content)
        {
            _ = Builder
                .AppendHtml(content)
                .AppendHtml("\n");
            return this;
        }
        
        public SimpleForm<T> AppendHtml(string encoded)
        {
            _ = Builder
                .AppendHtml(encoded)
                .AppendHtml("\n");
            return this;
        }

        private void AddLabelFor<TResult>(Expression<Func<T, TResult>> expression, string cssClass = "control-label")
        {
            AppendHtml(Html.LabelFor(expression, Html.DisplayNameFor(expression), htmlAttributes: new { @class = cssClass }));
        }
        
        private SimpleForm<T> AddValidationMessageFor<TResult>(Expression<Func<T, TResult>> expression)
        {
            return AppendHtml(Html.ValidationMessageFor(expression, null, htmlAttributes: new { @class = "text-danger" }));
        }

        private void AddRadioEditorFor<TResult>(
            Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> items, 
            HtmlAttributeOptions htmlAttributeOptions, 
            bool inline = false)
        {
            htmlAttributeOptions.CssClass = "form-check-input";

            Dictionary<string, object> htmlAttributes = htmlAttributeOptions.ToDictionary();
            
            foreach (SelectListItem item in items)
            {
                using IDisposable group = CreateFormGroup(additionalCssClass: $"form-check {(inline ? "form-check-inline" : "")}");
                AppendHtml(Html.RadioButtonFor(expression, item.Value, htmlAttributes: htmlAttributes));
                AppendHtml(Html.LabelFor(expression, item.Text, htmlAttributes: new { @class = "form-check-label" }));
            }
            AddValidationMessageFor(expression: expression);
            
            if (inline)
                AppendHtml("<br/>");
        }
        
        private void AddFileUploadEditorFor<TResult>(
            Expression<Func<T, TResult>> expression,
            HtmlAttributeOptions htmlAttributeOptions, 
            bool multiple = false, 
            string accept = "")
        {
            htmlAttributeOptions.CssClass = "custom-file-input";
            htmlAttributeOptions.Type = "file";
            Dictionary<string, object> htmlAttributes = htmlAttributeOptions.ToDictionary();
            
            if (multiple)
                htmlAttributes["multiple"] = "multiple";
            
            if (!string.IsNullOrEmpty(accept))
                htmlAttributes["accept"] = accept;
            
            using IDisposable group = CreateFormGroup();
            using IDisposable div = Div("custom-file");
            AppendHtml(Html.TextBoxFor(expression, htmlAttributes: htmlAttributes));
            AddLabelFor(expression: expression, cssClass: "custom-file-label");
            AddValidationMessageFor(expression: expression);
        }
        
        private static string GetEditorType(DataType? dataType) => 
            dataType switch
            {
                DataType.Password => "password",
                DataType.EmailAddress => "email",
                _ => "text"
            };
        
        private IDisposable CreateFormGroup(string additionalCssClass = "") => Div($"form-group {additionalCssClass}");
        
        private IDisposable Div(string cssClass) => new Scope(
            onBegin: () => AppendHtml($"<div class='{cssClass}'>"), 
            onEnd: () => AppendHtml("</div>"));

        private static HtmlAttributeOptions ResolveOptions(Action<HtmlAttributeOptions> optionsSetter)
        {
            HtmlAttributeOptions options = new();
            optionsSetter?.Invoke(options);
            return options;
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
