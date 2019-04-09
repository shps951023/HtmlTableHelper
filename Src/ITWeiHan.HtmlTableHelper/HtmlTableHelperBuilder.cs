using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlTableHelper
{
    public class HtmlTableHelperBuilder
    {
        public IEnumerable<object> Enums { get; set; }
        public HtmlCaption Caption { get; set; }
        public HtmlTableHelperBuilder(IEnumerable<object> enums) => Enums = enums;
        public static HtmlTableHelperBuilder Create(IEnumerable<object> enums) => new HtmlTableHelperBuilder(enums);
        public HtmlTableHelperBuilder SetCaption(string captionContent, object captionAttributes = null)
        {
            this.Caption = new HtmlCaption() { Content = captionContent, Attributes = captionAttributes };
            return this;
        }
    }

    public class HtmlCaption
    {
        public string Content { get; set; }
        public object Attributes { get; set; }
    }

    public static class HtmlTableHelperBuilderExtension
    {
        public static HtmlTableHelperBuilder CreateBuilder(this IEnumerable<object> enums) => new HtmlTableHelperBuilder(enums);
    }
}
