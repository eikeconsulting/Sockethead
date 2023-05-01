using System.Collections.Generic;

namespace Sockethead.Razor.Forms
{
    public class HtmlAttributeOptions
    {
        public bool IsReadOnly { get; set; }
        public bool IsDisabled { get; set; }
        public string CssClass { get; set; }
        public string Type { get; set; }
        
        public HtmlAttributeOptions(
            bool isReadOnly = false, 
            bool isDisabled = false,
            string cssClass = "form-control", 
            string type = null)
        {
            IsReadOnly = isReadOnly;
            IsDisabled = isDisabled;
            CssClass = cssClass;
            Type = type;
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> map = new();
            
            if (!string.IsNullOrEmpty(CssClass))
                map["class"] = CssClass;
            
            if (IsReadOnly)
                map["readonly"] = "readonly";
            
            if (IsDisabled)
                map["disabled"] = "disabled";
            
            if (!string.IsNullOrEmpty(Type))
                map["type"] = Type;

            return map;
        }
    }
}