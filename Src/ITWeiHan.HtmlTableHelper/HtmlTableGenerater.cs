using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HtmlTableHelper
{
    public static partial class HtmlTableHelper
    {
        private class HtmlTableGenerater
        {
            #region Prop
            private static readonly HtmlTableSetting _DefualtHTMLTableSetting = new HtmlTableSetting()
            {
                IsHtmlEncodeMode = true
            };
            private HtmlTableSetting _HtmlTableSetting { get; set; }
            private Dictionary<string, string> _TableAttributes { get; set; }
            private Dictionary<string, string> _TrAttributes { get; set; }
            private Dictionary<string, string> _TdAttributes { get; set; }
            private string _TableAttHtml { get; set; }
            private string _TrAttHtml { get; set; }
            private string _TdAttHtml { get; set; }
            private IEnumerable<TableColumnAttribute> _customAttributes { get; set; }
            #endregion

            public HtmlTableGenerater(object tableAttributes, object trAttributes, object tdAttributes, HtmlTableSetting htmlTableSetting)
            {
                this._HtmlTableSetting = htmlTableSetting ?? _DefualtHTMLTableSetting;
                this._TableAttributes = AttributeToHtml(tableAttributes);
                this._TrAttributes = AttributeToHtml(trAttributes);
                this._TdAttributes = AttributeToHtml(tdAttributes);
            }

            private Dictionary<string, string> AttributeToHtml(object tableAttributes)
            {
                if (tableAttributes == null)
                {
                    return null;
                }

                var type = tableAttributes.GetType();
                var dic = type.GetProperties()
                    //TODO:Convert to Cache
                    .Select(prop => new { Key = prop.Name, Value = prop.GetValue(tableAttributes).ToString() })
                    .ToDictionary(key => key.Key, value => value.Value);
                return dic;
            }

            /// <summary>
            /// Not Support Annotation
            /// </summary>
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

                var html = GenerateTableHtml<object>(heads: heads, bodys: bodys, enums: null);
                return html.ToString();
            }

            public string ToHtmlTableByProperties<T>(IEnumerable<T> enums)
            {
                RenderTableTrTdAttributehtml();

                var type = typeof(T);
                var props = type.GetProperties().ToList();

                #region Get Custom Attributes
                _customAttributes = CustomAttributeHelper.GetCustomAttributes(type);
                #endregion 

                #region Check
                if (props.Count == 0)
                {
                    throw new Exception("At least one Property");
                }
                #endregion

                //heads
                var html = new StringBuilder($"<table{_TableAttHtml}>");

                //Head
                html.Append($"<thead><tr{_TrAttHtml}>");
                foreach (var p in props)
                {
                    var costomAtt = CustomAttributeHelper.GetCustomAttributeByProperty(_customAttributes, p);
                    string thInnerHTML = costomAtt != null ? costomAtt.DisplayName : Encode(p.Name);
                    html.Append($"<th>{thInnerHTML}</th>");
                }
                html.Append("</tr></thead>");

                //Body
                html.Append("<tbody>");
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

                foreach (var values in bodys)
                {
                    html.Append($"<tr{_TrAttHtml}>");
                    foreach (var v in values)
                    {
                        string tdInnerHTML = Encode(v);
                        html.Append($"<td{_TdAttHtml}>{tdInnerHTML}</td>");
                    }
                    html.Append("</tr>");
                }
                html.Append("</tbody>");

                html.Append("</table>");
                return html.ToString();
            }

            public string ToHtmlTableByKeyValue(IEnumerable<IDictionary<string, object>> enums)
            {
                var html = GenerateTableHtml(heads: enums.First().Keys, bodys: enums.Select(s => s.Values), enums: enums);
                return html.ToString();
            }

            public string ToHtmlTableByKeyValue(IEnumerable<IDictionary> enums)
            {
                var html = GenerateTableHtml(heads: enums.First().Keys, bodys: enums.Select(s => s.Values), enums: enums);
                return html.ToString();
            }

            private StringBuilder GenerateTableHtml<T>(IEnumerable heads
                , IEnumerable<IEnumerable> bodys, IEnumerable<T> enums)
            {
                RenderTableTrTdAttributehtml();

                var html = new StringBuilder($"<table{_TableAttHtml}>");

                //Head
                html.Append($"<thead><tr{_TrAttHtml}>");
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
                    html.Append($"<tr{_TrAttHtml}>");
                    foreach (var v in values)
                    {
                        string tdInnerHTML = Encode(v);
                        html.Append($"<td{_TdAttHtml}>{tdInnerHTML}</td>");
                    }
                    html.Append("</tr>");
                }
                html.Append("</tbody>");

                html.Append("</table>");
                return html;
            }

            private void RenderTableTrTdAttributehtml()
            {
                this._TableAttHtml = _TableAttributes != null
                    ? string.Join("", _TableAttributes.Select(s => $" {s.Key}=\"{Encode(s.Value)}\" "))
                    : "";
                this._TrAttHtml = _TrAttributes != null
                    ? string.Join("", _TrAttributes.Select(s => $" {s.Key}=\"{Encode(s.Value)}\" "))
                    : "";
                this._TdAttHtml = _TdAttributes != null
                    ? string.Join("", _TdAttributes.Select(s => $" {s.Key}=\"{Encode(s.Value)}\" "))
                    : "";
            }

            private string Encode(object obj)
            {
                return _HtmlTableSetting.IsHtmlEncodeMode ? HttpUtility.HtmlEncode(obj.ToString()) : obj.ToString();
            }
        }
    }
}
