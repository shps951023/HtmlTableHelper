using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlTableHelper
{
    public static class HTMLTableHelper
    {

        //TODO:指定某幾欄位,需要做假如沒有特定欄位報錯動作。

        public static string ToHtmlTable<T>(this IEnumerable<T> enums)
        {
            if (enums is IEnumerable<IDictionary>)
            {
                return ToHtmlTableByKeyValue(enums as IEnumerable<IEnumerable<KeyValuePair<object, object>>>);
            }
            else
            {
                return enums.ToHtmlTableByProperties();
            }
        }

        public static string ToHtmlTable(this System.Data.DataTable dt)
        {
            var html = new StringBuilder("<table>");

            //Head
            html.Append("<thead><tr>");
            for (int i = 0; i < dt.Columns.Count; i++)
                html.Append("<th>" + dt.Columns[i].ColumnName + "</th>");
            html.Append("</tr></thead>");

            //Body
            html.Append("<tbody>");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html.Append("<tr>");
                for (int j = 0; j < dt.Columns.Count; j++)
                    html.Append("<td>" + dt.Rows[i][j].ToString() + "</td>");
                html.Append("</tr>");
            }

            html.Append("</tbody>");
            html.Append("</table>");
            return html.ToString();
        }

        private static string ToHtmlTableByProperties<T>(this IEnumerable<T> enums)
        {
            var type = typeof(T);
            var props = type.GetProperties().ToList();

            //Check
            if (props.Count == 0) throw new Exception("At least one Property");

            //Header
            var html = new StringBuilder("<table>");
            html.Append("<thead><tr>");
            foreach (var p in props)
                html.Append("<th>" + p.Name + "</th>");
            html.Append("</tr></thead>");

            //Body
            html.Append("<tbody>");
            foreach (var e in enums)
            {
                html.Append("<tr>");
                props.Select(s => s.GetValue(e)).ToList().ForEach(p =>
                {
                    html.Append("<td>" + p + "</td>");
                });
                html.Append("</tr>");
            }
            html.Append("</tbody>");

            html.Append("</table>");
            return html.ToString();
        }

        private static string ToHtmlTableByKeyValue<TKey, TValue>(this IEnumerable<IEnumerable<KeyValuePair<TKey, TValue>>> enums)
        {
            throw new Exception("Not Support Now");
            var keyType = typeof(TKey);
            var valueType = typeof(TValue);
           
            return "";
        }
    }
}
