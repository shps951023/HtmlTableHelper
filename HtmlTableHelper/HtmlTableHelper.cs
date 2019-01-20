using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HtmlTableHelper
{
    public static class HTMLTableHelper
    {
        //TODO:Specify fields,And need to throw error if there is no specific column.

        public static string ToHtmlTable<T>(this IEnumerable<T> enums)
        {
            if (enums is IEnumerable<IDictionary<string, object>>)
                return ToHtmlTableByKeyValue(enums as IEnumerable<IDictionary<string, object>>);       
            else if (enums is IEnumerable<IDictionary>)
                return ToHtmlTableByKeyValue(enums as IEnumerable<IDictionary>); 
            else
                return enums.ToHtmlTableByProperties();
        }

        public static string ToHtmlTable(this System.Data.DataTable dt)
        {
            var heads = new List<string>();
            for (int i = 0; i < dt.Columns.Count; i++)
                heads.Add(dt.Columns[i].ColumnName);

            var bodys = new List<List<object>>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var list = new List<object>();
                bodys.Add(list);
                for (int j = 0; j < dt.Columns.Count; j++)
                    list.Add(dt.Rows[i][j]);
            }

            var html = GenerateTableHtml(heads: heads, bodys: bodys);
            return html.ToString();
        }

        private static string ToHtmlTableByProperties<T>(this IEnumerable<T> enums)
        {
            var type = typeof(T);
            var props = type.GetProperties().ToList();

            //Check
            if (props.Count == 0) throw new Exception("At least one Property");

            var heads = props.Select(s => s.Name);

            var bodys = new List<List<object>>();
            foreach (var e in enums)
                bodys.Add(props.Select(s => s.GetValue(e)).ToList());

            var html = GenerateTableHtml(heads: heads, bodys: bodys);
            return html.ToString();
        }

        private static string ToHtmlTableByKeyValue(this IEnumerable<IDictionary<string, object>> enums)
        {
            //TODO:How to Slove No Data
            var html = GenerateTableHtml(heads: enums.First().Keys, bodys: enums.Select(s => s.Values));
            return html.ToString();
        }

        private static string ToHtmlTableByKeyValue(this IEnumerable<IDictionary> enums)
        {
            //TODO:How to Slove No Data
            var html = GenerateTableHtml(heads:enums.First().Keys, bodys:enums.Select(s=>s.Values));
            return html.ToString();
        }

        private static StringBuilder GenerateTableHtml(IEnumerable heads
            ,IEnumerable<IEnumerable> bodys)
        {
            var html = new StringBuilder("<table>");

            //Head
            html.Append("<thead><tr>");
            foreach (var p in heads)
                html.Append($"<th>{HttpUtility.HtmlEncode(p.ToString())}</th>");
            html.Append("</tr></thead>");

            //Body
            html.Append("<tbody>");
            foreach (var e in bodys)
            {
                html.Append("<tr>");
                foreach (var element in e)
                    html.Append($"<td>{HttpUtility.HtmlEncode(element.ToString())}</td>");
                html.Append("</tr>");
            }
            html.Append("</tbody>");

            html.Append("</table>");
            return html;
        }
    }
}
