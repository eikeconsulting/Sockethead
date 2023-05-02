using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sockethead.Razor.Forms
{
    public class FormOptions
    {
        public string ActionName { get; set; } = null;
        public string ControllerName { get; set; } = null;
        public FormMethod FormMethod { get; set; } = FormMethod.Post;
        public string CssClass { get; set; } = "";
    }
}