namespace Sockethead.Razor.Forms
{
    public class HtmlAttributeOptions
    {
        public bool IsReadOnly { get; set; }
        public bool IsDisabled { get; set; }
        public string CssClass { get; set; } = "form-control";
        public string Type { get; set; }
        
        public HtmlAttributeOptions(){}
        
        public HtmlAttributeOptions(bool isReadOnly = false, bool isDisabled = false,
            string cssClass = "form-control", string type = null)
        {
            IsReadOnly = isReadOnly;
            IsDisabled = isDisabled;
            CssClass = cssClass;
            Type = type;
        }
    }
}