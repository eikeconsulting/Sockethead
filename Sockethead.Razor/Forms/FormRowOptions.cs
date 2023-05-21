using System.Collections.Generic;

namespace Sockethead.Razor.Forms
{
    public enum FormRowType
    {
        Generic,
        Checkbox,
        Radios,
    }
    
    public class FormRowOptions
    {
        public FormRowType FormRowType { get; set; } = FormRowType.Generic;
        public bool IsReadOnly { get; set; }
        public bool IsDisabled { get; set; }
        public string CssClass { get; set; }
        public string Type { get; set; }
        public bool Inline { get; set; }
        public FormRowSize FormRowSize { get; set; } = FormRowSize.Default;

        public Dictionary<string, object> ExtraAttributes { get; } = new();

        public FormRowOptions()
        {
            IsReadOnly = false;
            IsDisabled = false;
            CssClass = "form-control";
            Type = null;
            Inline = false;
        }
        
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

            if (FormRowSize != FormRowSize.Default)
                map["class"] = $"{CssClass} {GetFormInputSizeClass(CssClass, FormRowSize)}";

            foreach (KeyValuePair<string, object> pair in ExtraAttributes)
                map[pair.Key] = pair.Value;

            return map;
        }

        private static string GetFormInputSizeClass(string cssClass, FormRowSize formRowSize)
        {
            return cssClass switch
            {
                { } a when a.Contains("form-control") => $"form-control-{GetFormRowSize(formRowSize)}",
                { } b when b.Contains("custom-select") => $"custom-select-{GetFormRowSize(formRowSize)}",
                // ToDo: Add more cases here
                _ => string.Empty
            };
        }

        private static string GetFormRowSize(FormRowSize formRowSize)
        {
            return formRowSize switch
            {
                FormRowSize.Small => "sm",
                FormRowSize.Large => "lg",
                _ => string.Empty
            };
        }
        
        public static string GetFormLabelSizeClass(string cssClass, FormRowSize formRowSize)
        {
            return cssClass switch
            {
                { } a when a.Contains("form-control") || a.Contains("custom-select") 
                    => $"col-form-label-{GetFormRowSize(formRowSize)}",
                // ToDo: Add more cases here
                _ => string.Empty
            };
        }
    }
}