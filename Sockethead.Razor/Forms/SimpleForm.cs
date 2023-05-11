using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
 
namespace Sockethead.Razor.Forms
{
    public partial class SimpleForm<T> : ISimpleForm where T : class
    {
        private readonly HtmlContentBuilder Builder = new();
        private IHtmlHelper<T> Html { get; }
        public FormOptions FormOptions { get; }
        private EnumRegistry<T> EnumRegistry { get; set; }
        
        public SimpleForm(IHtmlHelper<T> html, Action<FormOptions> optionsAction = null)
        {
            Html = html;
            FormOptions = new FormOptions();
            optionsAction?.Invoke(FormOptions);
        }

        
        //======================= Enum Support =======================
        
        public SimpleForm<T> RegisterEnums<TEnum1>()
        {
            EnumRegistry = new EnumRegistry<T, TEnum1>();
            return this;
        }
        public SimpleForm<T> RegisterEnums<TEnum1, TEnum2>()
        {
            EnumRegistry = new EnumRegistry<T, TEnum1, TEnum2>();
            return this;
        }
        public SimpleForm<T> RegisterEnums<TEnum1, TEnum2, TEnum3>()
        {
            EnumRegistry = new EnumRegistry<T, TEnum1, TEnum2, TEnum3>();
            return this;
        }

        //======================= HTML Helpers =======================
        
        public SimpleForm<T> AddDiv(string cssClass, Action<SimpleForm<T>> formAction)
        {
            using IDisposable div = Div(cssClass);
            formAction(this);
            return this;
        }
        
        public SimpleForm<T> AddRowDiv(Action<SimpleForm<T>> formAction) => AddDiv("form-row", formAction);

        public SimpleForm<T> AddColDiv(int width, Action<SimpleForm<T>> formAction) => AddDiv($"col-{width}", formAction);

        public SimpleForm<T> AddColDiv(Action<SimpleForm<T>> formAction) => AddDiv("col", formAction);
        
        public SimpleForm<T> AppendHtml(Func<object, IHtmlContent> contentFunc)
        {
            return AppendHtml(contentFunc(null));
        }

        public SimpleForm<T> AppendHtmlIf(bool condition, Func<object, IHtmlContent> contentFunc)
        {
            return condition 
                ? AppendHtml(contentFunc(null)) 
                : this;
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

        public async Task<IHtmlContent> RenderAsync()
        {
            return await Html.PartialAsync("_SHSimpleForm", this);
        }

        public IHtmlContent RenderForm() => Builder;
        
        
        //======================= Private Methods =======================
        
        private void AddLabelFor<TResult>(Expression<Func<T, TResult>> expression, string cssClass)
        {
            AppendHtml(Html.LabelFor(
                expression: expression, 
                labelText: Html.DisplayNameFor(expression), 
                htmlAttributes: new { @class = FormOptions.HideLabels ? "sr-only" : cssClass }));
        }
        
        private void AddValidationMessageFor<TResult>(Expression<Func<T, TResult>> expression)
        {
            AppendHtml(Html.ValidationMessageFor(expression, message: null, htmlAttributes: new { @class = "text-danger" }));
        }

        private IDisposable Div(string cssClass) => new Scope(
            onBegin: () => AppendHtml($"<div class='{cssClass}'>"), 
            onEnd: () => AppendHtml("</div>"));

        private static TFormOptions Resolve<TFormOptions>(Action<TFormOptions> optionsSetter, TFormOptions options) where TFormOptions : new()
        {
            optionsSetter?.Invoke(options);
            return options;
        }
        
        private static TFormOptions Resolve<TFormOptions>(Action<TFormOptions> optionsSetter) where TFormOptions : new()
        {
            return Resolve(optionsSetter, new TFormOptions());
        }
    }
}
