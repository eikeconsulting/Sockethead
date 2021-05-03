using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq.Expressions;

namespace Sockethead.Razor.Forms
{
    public static class SimpleFormExtensions
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static SimpleForm<T> SimpleForm<T>(this IHtmlHelper<T> html, T model) where T : class
                => new SimpleForm<T>(html);
#pragma warning restore IDE0060 // Remove unused parameter
    }

    public class FormOptions
    {
        public string ActionName { get; set; } = null;
        public string ControllerName { get; set; } = null;
        public FormMethod Method { get; set; } = FormMethod.Post;
    }


    public class SimpleForm<T> where T : class
    {
        HtmlContentBuilder Builder = new HtmlContentBuilder();
        private IHtmlHelper<T> Html { get; }

        public SimpleForm(IHtmlHelper<T> html)
        {
            Html = html;
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

        private IDisposable FormGroup() => new Scope(
            onBegin: () => Append("<div class='form-group'>"), 
            onEnd: () => Append("</div>"));

        public SimpleForm<T> TextBoxFor<TResult>(Expression<Func<T, TResult>> expression, string type = "text")
        {
            using (FormGroup())
            {
                Append(Html.LabelFor(expression, htmlAttributes: new { @class = "control-label" }));
                Append(Html.TextBoxFor(expression, htmlAttributes: new { @class = "form-control", type }));
                Append(Html.ValidationMessageFor(expression, null, htmlAttributes: new { @class = "text-danger" }));
            }
            return this;
        }

        public SimpleForm<T> DatePickerFor<TResult>(Expression<Func<T, TResult>> expression)
        {
            return TextBoxFor(expression, "date");
        }


        public IHtmlContent Render()
        {
            /*
            FormExtensions.BeginForm()
            using (var form = Html.BeginForm(actionName: "FormBuilder", controllerName: "Pager", method: FormMethod.Post))
            {
            }
            */

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
