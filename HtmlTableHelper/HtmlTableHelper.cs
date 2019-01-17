using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlTableHelper
{
    public static class HtmlTableHelper
    {
        public static string ToHtmlTable<T>(this IEnumerable<T> enums)
        {
            var type = typeof(T);
            var props = type.GetProperties();
            var html = new StringBuilder("<table>");

            //Header
            html.Append("<thead><tr>");
            foreach (var p in props)
                html.Append("<th>" + p.Name + "</th>");
            html.Append("</tr></thead>");

            //Body
            html.Append("<tbody>");
            foreach (var e in enums)
            {
                html.Append("<tr>");
                props.Select(s => s.GetValue(e)).ToList().ForEach(p => {
                    html.Append("<td>" + p + "</td>");
                });
                html.Append("</tr>");
            }
            html.Append("</tbody>");

            html.Append("</table>");
            return html.ToString();
        }
    }
}
