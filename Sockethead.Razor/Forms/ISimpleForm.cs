using Microsoft.AspNetCore.Html;

namespace Sockethead.Razor.Forms
{
    public interface ISimpleForm
    {
        FormOptions FormOptions { get; }
        string CssClass { get; }
        IHtmlContent RenderFormRows();
    }
}