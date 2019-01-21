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
 
        private class HTMLTableGenerater
        {
            private static readonly HTMLTableSetting _DefualtHTMLTableSetting = new HTMLTableSetting()
            {
                IsHtmlEncodeMode = true
            };
            public HTMLTableSetting mHTMLTableSetting { get; set; }
            public HTMLTableGenerater()
            {
                mHTMLTableSetting = _DefualtHTMLTableSetting;
            }

            public string ToHtmlTablByDataTable(System.Data.DataTable dt)
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

            public string ToHtmlTableByProperties<T>(IEnumerable<T> enums)
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

            public string ToHtmlTableByKeyValue(IEnumerable<IDictionary<string, object>> enums)
            {
                //TODO:How to Slove No Data
                var html = GenerateTableHtml(heads: enums.First().Keys, bodys: enums.Select(s => s.Values));
                return html.ToString();
            }

            public string ToHtmlTableByKeyValue(IEnumerable<IDictionary> enums)
            {
                //TODO:How to Slove No Data
                var html = GenerateTableHtml(heads: enums.First().Keys, bodys: enums.Select(s => s.Values));
                return html.ToString();
            }

            private StringBuilder GenerateTableHtml(IEnumerable heads
                , IEnumerable<IEnumerable> bodys)
            {
                var html = new StringBuilder("<table>");

                //Head
                html.Append("<thead><tr>");
                foreach (var p in heads)
                {
                    var thInnerHTML = mHTMLTableSetting.IsHtmlEncodeMode ? HttpUtility.HtmlEncode(p.ToString()) : p.ToString();
                    html.Append($"<th>{thInnerHTML}</th>");
                }
                html.Append("</tr></thead>");

                //Body
                html.Append("<tbody>");
                foreach (var values in bodys)
                {
                    html.Append("<tr>");
                    foreach (var v in values)
                    {
                        var tdInnerHTML = mHTMLTableSetting.IsHtmlEncodeMode 
                            ? HttpUtility.HtmlEncode(v.ToString()) 
                            : v.ToString();
                        html.Append($"<td>{tdInnerHTML}</td>");
                    }
                    html.Append("</tr>");
                }
                html.Append("</tbody>");

                html.Append("</table>");
                return html;
            }
        }

        public static string ToHtmlTable<T>(this IEnumerable<T> enums, HTMLTableSetting HTMLTableSetting = null)
        {
            var htmltablegenerater = CreatHTMLTableGeneraterBySetting(HTMLTableSetting);

            if (enums is IEnumerable<IDictionary<string, object>>)
                return htmltablegenerater.ToHtmlTableByKeyValue(enums as IEnumerable<IDictionary<string, object>>);       
            else if (enums is IEnumerable<IDictionary>)
                return htmltablegenerater.ToHtmlTableByKeyValue(enums as IEnumerable<IDictionary>); 
            else
                return htmltablegenerater.ToHtmlTableByProperties(enums);
        }

        public static string ToHtmlTable(this System.Data.DataTable dt, HTMLTableSetting HTMLTableSetting = null)
        {
            var htmltablegenerater = CreatHTMLTableGeneraterBySetting(HTMLTableSetting);
            return htmltablegenerater.ToHtmlTablByDataTable(dt);
        }

        private static HTMLTableGenerater CreatHTMLTableGeneraterBySetting(HTMLTableSetting HTMLTableSetting)
        {
            var htmltablegenerater = new HTMLTableGenerater();
            if (HTMLTableSetting != null)
                htmltablegenerater.mHTMLTableSetting = HTMLTableSetting;
            return htmltablegenerater;
        }

    }
}
