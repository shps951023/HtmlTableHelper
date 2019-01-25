using HtmlTableHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

namespace HtmlTableHelper
{
    public static partial class HtmlTableHelper
    {
        private class HtmlTableGenerater
        {
            private static readonly HtmlTableSetting _DefualtHTMLTableSetting = new HtmlTableSetting()
            {
                IsHtmlEncodeMode = true
            };

            #region Prop
            private HtmlTableSetting _HtmlTableSetting { get; set; }
            private Dictionary<string,string> _TableAttributes { get; set; }
            private Dictionary<string, string> _TrAttributes { get; set; }
            private Dictionary<string, string> _TdAttributes { get; set; }
            #endregion

            public HtmlTableGenerater(object tableAttributes, object trAttributes, object tdAttributes, HtmlTableSetting htmlTableSetting)
            {
                this._TableAttributes = AttributeToHtml(tableAttributes);
                this._TrAttributes = AttributeToHtml(trAttributes);
                this._TdAttributes = AttributeToHtml(tdAttributes);
                this._HtmlTableSetting = htmlTableSetting ?? _DefualtHTMLTableSetting;
            }

            private Dictionary<string, string> AttributeToHtml(object tableAttributes)
            {
                if (tableAttributes == null) return null;
                var type = tableAttributes.GetType();
                var dic = type.GetProperties()
                    //TODO:Convert to Cache
                    .Select(prop=>new { Key= prop.Name,Value=prop.GetValue(tableAttributes).ToString() })
                    .ToDictionary(key=>key.Key,value=>value.Value);
                return dic;
            }

            public string ToHtmlTablByDataTable(System.Data.DataTable dt)
            {
                var heads = new List<string>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    heads.Add(dt.Columns[i].ColumnName);
                }

                var bodys = new List<List<object>>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var list = new List<object>();
                    bodys.Add(list);
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        list.Add(dt.Rows[i][j]);
                    }
                }

                var html = GenerateTableHtml(heads: heads, bodys: bodys);
                return html.ToString();
            }

            public string ToHtmlTableByProperties<T>(IEnumerable<T> enums)
            {
                var type = typeof(T);
                var props = type.GetProperties().ToList();

                //Check
                if (props.Count == 0)
                {
                    throw new Exception("At least one Property");
                }

                var heads = props.Select(s => s.Name);

                var bodys = new List<List<string>>(); //TODO:'new List' this looks like it can be replaced
                foreach (var e in enums)
                {
                    bodys.Add(props.Select(prop =>
                    {
                        var compiledExpression = ExpressionCache.GetOrAddExpressionCache<T>(prop);
                        var value = compiledExpression(e);
                        return value;
                    }).ToList());
                }
                var html = GenerateTableHtml(heads: heads, bodys: bodys);
                return html.ToString();
            }

            public string ToHtmlTableByKeyValue(IEnumerable<IDictionary<string, object>> enums)
            {
                var html = GenerateTableHtml(heads: enums.First().Keys, bodys: enums.Select(s => s.Values));
                return html.ToString();
            }

            public string ToHtmlTableByKeyValue(IEnumerable<IDictionary> enums)
            {
                var html = GenerateTableHtml(heads: enums.First().Keys, bodys: enums.Select(s => s.Values));
                return html.ToString();
            }

            private StringBuilder GenerateTableHtml(IEnumerable heads
                , IEnumerable<IEnumerable> bodys)
            {
                #region attrs
                var tableAttHtml = _TableAttributes != null
                    ? string.Join("", _TableAttributes.Select(s => $" {s.Key}=\"{Encode(s.Value)}\" "))
                    : "";
                var trAttHtml = _TrAttributes != null
                    ? string.Join("", _TrAttributes.Select(s => $" {s.Key}=\"{Encode(s.Value)}\" "))
                    : "";
                var tdAttHtml = _TdAttributes != null
                    ? string.Join("", _TdAttributes.Select(s => $" {s.Key}=\"{Encode(s.Value)}\" "))
                    : "";
                #endregion

                var html = new StringBuilder($"<table{tableAttHtml}>");

                //Head
                html.Append($"<thead><tr{trAttHtml}>");
                foreach (var p in heads)
                {
                    string thInnerHTML = Encode(p);
                    html.Append($"<th>{thInnerHTML}</th>");
                }
                html.Append("</tr></thead>");

                //Body
                html.Append("<tbody>");
                foreach (var values in bodys)
                {
                    html.Append($"<tr{trAttHtml}>");
                    foreach (var v in values)
                    {
                        string tdInnerHTML = Encode(v);
                        html.Append($"<td{tdAttHtml}>{tdInnerHTML}</td>");
                    }
                    html.Append("</tr>");
                }
                html.Append("</tbody>");

                html.Append("</table>");
                return html;
            }

            private string Encode(object obj)
            {
                return _HtmlTableSetting.IsHtmlEncodeMode ? HttpUtility.HtmlEncode(obj.ToString()) : obj.ToString();
            }
        }
    }
}
