using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sockethead.Razor.Helpers;

namespace Sockethead.Razor.Forms
{
    public partial class SimpleForm<T>
    {
        /// <summary>
        /// Build form automatically from the model via Reflection
        /// </summary>
        public SimpleForm<T> AddRowsForModel()
        {
            SimpleFormMagic<T> magic = new(this, EnumRegistry);
            magic.AddRowsForModel();
            return this;
        }

        public SimpleForm<T> AddRowFor<TResult>(
            Expression<Func<T, TResult>> expression, 
            Action<FormRowOptions> optionsSetter = null)
        {
            // if row is hidden, nothing else matters
            if (expression.GetAttribute<HiddenInputAttribute, T, TResult>() != null)
                return AddHiddenFor(expression);
            
            // first try to resolve the control based on the DataType Attribute
            switch (expression.GetDataTypeAttribute())
            {
                case DataType.Text: return AddTextBoxRowFor(expression, optionsSetter: optionsSetter);
                case DataType.MultilineText: return AddTextAreaRowFor(expression, optionsSetter: optionsSetter);
                case DataType.EmailAddress: return AddEmailRowFor(expression, optionsSetter);
                case DataType.Password: return AddPasswordRowFor(expression, optionsSetter);
                case DataType.PhoneNumber: return AddPhoneRowFor(expression, optionsSetter);
                case DataType.DateTime: return AddDateTimeRowFor(expression, optionsSetter);
                case DataType.Date: return AddDateRowFor(expression, optionsSetter);
                case DataType.Upload: return AddFileUploadRowFor(expression, optionsSetter: optionsSetter);
                case DataType.CreditCard: return AddCreditCardRowFor(expression, optionsSetter: optionsSetter);

                // ReSharper disable RedundantCaseLabel
                case DataType.Time:
                case DataType.Currency:
                case DataType.Custom: // skip 
                case DataType.Duration: // skip
                case DataType.Html:
                case DataType.Url:
                case DataType.ImageUrl:
                case DataType.PostalCode:
                case null:
                default:
                    
                    // if no DataType attribute, try to resolve the control based on the property type
                    Type resultType = typeof(TResult);

                    // if the type is an enum, use a select list with the Enumerated values
                    if (resultType.IsEnum)
                        return AddEnumRowFor(expression, optionsSetter);

                    // if the type is numeric, use a number input
                    if (resultType.IsNumeric())
                        return AddNumberRowFor(expression, optionsSetter);

                    return resultType switch
                    {
                        // Booleans are Checkboxes
                        _ when resultType == typeof(bool) => 
                            AddCheckBoxRowFor(expression as Expression<Func<T, bool>>, optionsSetter),
                        
                        // DateTime will still work if no DataType is specified
                        _ when resultType == typeof(DateTime) => 
                            AddDateTimeRowFor(expression, optionsSetter),
                        
                        // TextBox is the  default control
                        _ => 
                            AddTextBoxRowFor(expression, optionsSetter: optionsSetter)
                    };
            }
        }

        public SimpleForm<T> AddTextBoxRowFor<TResult>(
            Expression<Func<T, TResult>> expression, 
            string format = null,
            string type = null,
            Action<FormRowOptions> optionsSetter = null)
        {
            FormRowOptions options = new();

            // if no format provided, check the DisplayFormat attribute
            format ??= expression.GetAttribute<DisplayFormatAttribute, T, TResult>()?.DataFormatString;
            
            options.Type = type;

            // resolve options after setting type so the caller overrides it
            Resolve(optionsSetter, options);
                
            return AddControlRowFor(
                expression:expression,
                formRowOptions: options,
                htmlContent: Html.TextBoxFor(
                    expression: expression, 
                    format: format, 
                    htmlAttributes: options.GetHtmlAttributes()));
        }

        public SimpleForm<T> AddTextAreaRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            int? rows = null,
            int? columns = null,
            Action<FormRowOptions> optionsSetter = null)
        {
            FormRowOptions options = Resolve(optionsSetter);
            Dictionary<string, object> attributes = options.GetHtmlAttributes();

            // ReSharper disable PossibleInvalidOperationException
            IHtmlContent content = rows == null && columns == null
                ? Html.TextAreaFor(expression, htmlAttributes: attributes)
                : Html.TextAreaFor(expression, htmlAttributes: attributes, rows: rows.Value, columns: columns.Value);
            
            return AddControlRowFor(
                expression: expression,
                formRowOptions: options,
                htmlContent: content);
        }

        public SimpleForm<T> AddEmailRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            Action<FormRowOptions> optionsSetter = null) => 
            AddTextBoxRowFor(
                expression: expression, 
                type: "email", 
                optionsSetter: optionsSetter);

        public SimpleForm<T> AddPasswordRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            Action<FormRowOptions> optionsSetter = null) => 
            AddTextBoxRowFor(
                expression: expression, 
                type: "password", 
                optionsSetter: optionsSetter);

        public SimpleForm<T> AddPhoneRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            Action<FormRowOptions> optionsSetter = null) =>
            AddTextBoxRowFor(
                expression: expression, 
                type: "tel", 
                optionsSetter: optionsSetter);

        public SimpleForm<T> AddCreditCardRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            Action<FormRowOptions> optionsSetter = null) =>
            AddTextBoxRowFor(
                expression: expression, 
                type: "tel", 
                optionsSetter: opt => 
                {
                    // https://stackoverflow.com/a/59757039/910348
                    opt.ExtraAttributes["inputmode"] = "numeric";
                    opt.ExtraAttributes["pattern"] = @"[0-9\s]{13,19}";
                    opt.ExtraAttributes["autocomplete"] = "cc-number";
                    opt.ExtraAttributes["maxlength"] = "19";
                    opt.ExtraAttributes["placeholder"] = "xxxx xxxx xxxx xxxx";
                    optionsSetter?.Invoke(opt);
                });
        
        public SimpleForm<T> AddDateTimeRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            Action<FormRowOptions> optionsSetter = null) =>
            AddTextBoxRowFor(
                expression: expression, 
                format: "{0:yyyy-MM-ddTHH:mm}", 
                type: "datetime-local", 
                optionsSetter: optionsSetter);

        public SimpleForm<T> AddDateRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            Action<FormRowOptions> optionsSetter = null) =>
            AddTextBoxRowFor(
                expression: expression, 
                format: "{0:yyyy-MM-dd}", 
                type: "date", 
                optionsSetter: optionsSetter);

        public SimpleForm<T> AddNumberRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            Action<FormRowOptions> optionsSetter = null) =>
            AddTextBoxRowFor(
                expression: expression, 
                format: null, 
                type: "number", 
                optionsSetter: optionsSetter);

        public SimpleForm<T> AddEnumRowFor<TResult>(
            Expression<Func<T, TResult>> expression, 
            Action<FormRowOptions> optionsSetter = null) =>
            AddDropDownListRowFor(
                expression: expression, 
                selectList: new SelectList(Enum
                    .GetValues(typeof(TResult))
                    .Cast<TResult>()
                    .ToList()), 
                optionsSetter: optionsSetter);

        public SimpleForm<T> AddDropDownListRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList, 
            Action<FormRowOptions> optionsSetter = null)
        {
            FormRowOptions options = Resolve(optionsSetter);
            options.CssClass = "custom-select";

            return AddControlRowFor(
                expression: expression,
                formRowOptions: options,
                htmlContent: Html.DropDownListFor(
                    expression: expression, 
                    selectList: selectList, 
                    htmlAttributes: options.GetHtmlAttributes()));
        }

        public SimpleForm<T> AddListBoxFor<TResult>(
            Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList, 
            Action<FormRowOptions> optionsSetter = null)
        {
            FormRowOptions options = Resolve(optionsSetter);
            options.CssClass = "custom-select";

            return AddControlRowFor(
                expression: expression, 
                formRowOptions: options,
                htmlContent: Html.ListBoxFor(
                    expression: expression, 
                    selectList: selectList, 
                    htmlAttributes: options.GetHtmlAttributes()));
        }
        
        public SimpleForm<T> AddRadioButtonRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            IEnumerable<SelectListItem> selectList, 
            Action<FormRowOptions> optionsSetter = null)
        {
            FormRowOptions options = Resolve(optionsSetter); 
            options.CssClass = "form-check-input";
            Dictionary<string, object> htmlAttributes = options.GetHtmlAttributes();
            options.FormRowType = FormRowType.Radios;
            
             return AppendHtml(
                Html.Partial(
                    partialViewName: "_SHFormRow", 
                    model: new FormRowViewModel
                    {
                        FormOptions = FormOptions,
                        FormRowOptions = options,
                        Label = cssClass => Html.LabelFor(
                            expression: expression, 
                            labelText: Html.DisplayNameFor(expression), 
                            htmlAttributes: new { @class = cssClass }),
                        SelectListItems = selectList,
                        RenderSelectListItem = item =>
                        {
                            htmlAttributes["id"] = item.Value;
                            return Html.RadioButtonFor(
                                expression: expression,
                                value: item.Value,
                                htmlAttributes: htmlAttributes);
                        },
                        ValidationMessage = Html.ValidationMessageFor(
                            expression: expression, 
                            message: null, 
                            htmlAttributes: new { @class = "text-danger" }),
                    }));
        }
        
        public SimpleForm<T> AddCheckBoxRowFor(
            Expression<Func<T, bool>> expression, 
            Action<FormRowOptions> optionsSetter = null)
        {
            FormRowOptions options = Resolve(optionsSetter);
            options.CssClass = "form-check-input";

            using IDisposable group = Div("form-group form-check");

            if (FormOptions.HorizontalForm)
                AppendHtml("<div class='row'><div class='col-sm-9 offset-sm-3'>");
            
            AppendHtml(Html.CheckBoxFor(
                expression: expression, 
                htmlAttributes: options.GetHtmlAttributes()));
            AddLabelFor(
                expression: expression, 
                cssClass: "form-check-label");
            AddValidationMessageFor(expression);

            if (FormOptions.HorizontalForm)
                AppendHtml("</div></div>");
            
            return this;
        }
        
        public SimpleForm<T> AddFileUploadRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            bool multiple = false, 
            string accept = "", 
            Action<FormRowOptions> optionsSetter = null)
        {
            FormRowOptions options = Resolve(optionsSetter);
            
            options.CssClass = "custom-file-input";
            options.Type = "file";
            
            Dictionary<string, object> htmlAttributes = options.GetHtmlAttributes();
           
            if (multiple)
                htmlAttributes["multiple"] = "multiple";
            
            if (!string.IsNullOrEmpty(accept))
                htmlAttributes["accept"] = accept;
            
            using IDisposable group = Div("form-group form-row");
            using IDisposable div = Div("custom-file");
            AppendHtml(Html.TextBoxFor(
                expression: expression, 
                htmlAttributes: htmlAttributes));
            AddLabelFor(
                expression: expression, 
                cssClass: "custom-file-label");
            AddValidationMessageFor(expression: expression);
            
            return this;
        }
 
        public SimpleForm<T> AddHiddenFor<TResult>(Expression<Func<T, TResult>> expression) => 
            AppendHtml(Html.HiddenFor(expression));

        public SimpleForm<T> AddSubmitButton(
            string label = "Submit", 
            string actionName = null, 
            string controllerName = null, 
            string css = "btn-primary") =>
            AppendHtml(
                Html.Partial(
                    partialViewName: "_SHFormButton", 
                    model: new ButtonViewModel
                    {
                        Label = label,
                        Action = actionName ?? FormOptions.ActionName,
                        Controller = controllerName ?? FormOptions.ControllerName,
                        Css = css,
                    }));

        public SimpleForm<T> AddLinkButton(
            string label, 
            string url, 
            string css = "btn-primary") =>
            AppendHtml(
                Html.Partial(
                    partialViewName: "_SHFormButton", 
                    model: new ButtonViewModel
                    {
                        Label = label,
                        Url = url,
                        Css = css,
                    }));
        
        private SimpleForm<T> AddControlRowFor<TResult>(
            Expression<Func<T, TResult>> expression,
            FormRowOptions formRowOptions,
            IHtmlContent htmlContent) =>
            AppendHtml(
                Html.Partial(
                    partialViewName: "_SHFormRow", 
                    model: new FormRowViewModel
                    {
                        FormOptions = FormOptions,
                        FormRowOptions = formRowOptions,
                        Label = cssClass => Html.LabelFor(
                            expression: expression, 
                            labelText: Html.DisplayNameFor(expression), 
                            htmlAttributes: new { @class = cssClass }),
                        Input = htmlContent,
                        ValidationMessage = Html.ValidationMessageFor(expression, message: null, htmlAttributes: new { @class = "text-danger" }),
                    }));
    }
}
