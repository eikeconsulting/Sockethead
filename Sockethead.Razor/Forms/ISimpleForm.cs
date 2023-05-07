using Microsoft.AspNetCore.Html;

namespace Sockethead.Razor.Forms
{
    public interface ISimpleForm
    {
        FormOptions FormOptions { get; }
        IHtmlContent RenderForm();
    }
}