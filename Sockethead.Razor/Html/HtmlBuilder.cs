using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Sockethead.Razor.Html
{
    public static class HtmlBuilderExtensions
    {
        public static HtmlBuilder HtmlBuilder(this IHtmlHelper html) => new HtmlBuilder(html);

    }

    public class HtmlBuilder
    {
        private IHtmlHelper Html { get; }
        public HtmlElementList ActiveList = new HtmlElementList();

        public HtmlBuilder(IHtmlHelper htmlHelper)
        {
            Html = htmlHelper;
        }

        public HtmlBuilder Content(string content, bool encode = true)
        {
            var htmlContent = new HtmlContent
            {
                Content = content,
                Encode = encode,
            };

            ActiveList.Elements.Add(htmlContent);

            return this;
        }


        public HtmlBuilder Container(Action<HtmlTagBuilder> builder = null, Action<HtmlBuilder> then = null)
        {
            return Div(tag =>
            {
                tag.Class("container");
                if (builder != null)
                    builder(tag);
                tag.Then(then);
            });
        }

        public HtmlBuilder Row(Action<HtmlTagBuilder> builder = null, Action<HtmlBuilder> then = null)
        {
            return Div(tag =>
            {
                tag.Class("row");
                if (builder != null)
                    builder(tag);
                tag.Then(then);
            });
        }

        public HtmlBuilder Col(Action<HtmlTagBuilder> builder = null, Action<HtmlBuilder> then = null)
        {
            return Div(tag =>
            {
                if (builder != null)
                    builder(tag);
                tag.Then(then);
            });
        }


        public HtmlBuilder Div(Action<HtmlTagBuilder> tagBuilderAction)
        {
            return Tag(tag => 
            {
                tag.Name("div");
                tagBuilderAction(tag);
            });
        }

        public HtmlBuilder Tag(Action<HtmlTagBuilder> tagBuilderAction)
        {
            var tag = new HtmlTag();
            ActiveList.Elements.Add(tag);

            tagBuilderAction(new HtmlTagBuilder
            {
                HtmlTag = tag,
                HtmlBuilder = this,
            });

            return this;
        }

        public IHtmlContent Render()
        {
            return Html.Raw(ActiveList.Render());
        }
    }

    public class HtmlTagBuilder
    {
        internal HtmlTag HtmlTag { get; set; }
        internal HtmlBuilder HtmlBuilder { get; set; }

        public HtmlTagBuilder Name(string name)
        {
            HtmlTag.Name = name;
            return this;
        }

        public HtmlTagBuilder Class(string classes)
        {
            HtmlTag.Class = classes;
            return this;
        }

        public HtmlTagBuilder Style(string style)
        {
            HtmlTag.Style = style;
            return this;
        }

        public HtmlTagBuilder Then(Action<HtmlBuilder> builderAction)
        {
            if (builderAction == null)
                return this;

            var cur = HtmlBuilder.ActiveList;
            HtmlBuilder.ActiveList = HtmlTag;
            builderAction(HtmlBuilder);
            HtmlBuilder.ActiveList = cur;
            return this;
        }
    }

    public abstract class HtmlElement
    {
        public abstract string Render();
    }

    public class HtmlElementList : HtmlElement
    {
        public List<HtmlElement> Elements { get; } = new List<HtmlElement>();

        public override string Render() => string.Join("", Elements.Select(e => e.Render()));
    }

    public class HtmlContent : HtmlElement
    {
        public string Content { get; set; }
        public bool Encode { get; set; } = true;

        public override string Render() => Encode ? HttpUtility.HtmlEncode(Content) : Content;
    }

    public class HtmlTag : HtmlElementList
    {
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; } = new Dictionary<string, string>();
        public bool Encode { get; set; } = true;
        public bool NewLine { get; set; } = true;

        public string Class 
        {
            get => Attributes.ContainsKey("class") ? Attributes["class"] : "";
            set => Attributes["class"] = value;
        }

        public string Style
        {
            get => Attributes.ContainsKey("style") ? Attributes["style"] : "";
            set => Attributes["style"] = value;
        }

        public override string Render()
        {
            var sb = new StringBuilder();

            sb.Append($"<{Name}");

            foreach (var kvp in Attributes)
                sb.Append($" {kvp.Key}='{kvp.Value}'");

            if (Elements.Any())
            {
                sb.Append($">{base.Render()}</{Name}>");
            }
            else
            {
                sb.Append(" />");
            }

            if (NewLine)
                sb.Append('\n');

            return sb.ToString();
        }

    }
}
