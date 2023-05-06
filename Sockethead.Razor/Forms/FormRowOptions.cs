using System.Collections.Generic;

namespace Sockethead.Razor.Forms
{
    public class FormRowOptions
    {
        public bool IsReadOnly { get; set; }
        public bool IsDisabled { get; set; }
        internal string CssClass { get; set; }
        public string Type { get; set; }
        public bool Inline { get; set; }

        public FormRowOptions(
            bool isReadOnly = false, 
            bool isDisabled = false,
            string cssClass = "form-control", 
            string type = null)
        {
            IsReadOnly = isReadOnly;
            IsDisabled = isDisabled;
            CssClass = cssClass;
            Type = type;
            Inline = false;
        }

        public Dictionary<string, object> GetHtmlAttributes()
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