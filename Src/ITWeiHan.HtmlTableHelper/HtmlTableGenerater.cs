using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValueGetter;

namespace HtmlTableHelper
{
    public static partial class HtmlTableHelper
    {
        private class HtmlTableGeneraterFactory
        {
            internal static readonly HtmlTableSetting _DefualtHTMLTableSetting = new HtmlTableSetting()
            {
                IsHtmlEncodeMode = true
            };

            public static HtmlTableGenerater CreateInstance<T1, T2, T3>(T1 tableAttributes, T2 trAttributes, T3 tdAttributes, HtmlTableSetting htmlTableSetting)
            {
                var htmltablegenerater = new HtmlTableGenerater
                {
                    _HtmlTableSetting = htmlTableSetting ?? _DefualtHTMLTableSetting,
                    _TableAttributes = AttributeToHtml(tableAttributes),
                    _TrAttributes = AttributeToHtml(trAttributes),
                    _TdAttributes = AttributeToHtml(tdAttributes),
                };
                htmltablegenerater.RenderTableTrTdAttributehtml();
                return htmltablegenerater;
            }

            public static Dictionary<string, string> AttributeToHtml<T>(T tableAttributes)
            {
                if (tableAttributes == null)
                    return null;

                var type = tableAttributes.GetType();
                var dic = tableAttributes.GetToStringValues();
                return dic;
            }
        }

        private class HtmlTableGenerater
        {
#region Prop

            internal HtmlTableSetting _HtmlTableSetting { get; set; }
            internal Dictionary<string, string> _TableAttributes { get; set; }
            internal Dictionary<string, string> _TrAttributes { get; set; }
            internal Dictionary<string, string> _TdAttributes { get; set; }
            internal string _TableAttHtml { get; set; }
            internal string _TrAttHtml { get; set; }
            internal string _TdAttHtml { get; set; }
            internal IEnumerable<TableColumnAttribute> _customAttributes { get; set; }
            internal HtmlTableHelperBuilder _HtmlTableHelperBuilder { get; set; }
#endregion

            internal HtmlTableGenerater(){}

            internal void RenderTableTrTdAttributehtml()
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

            private StringBuilder RenderHtmlTable(StringBuilder thead, StringBuilder tbody)
            {
                var html = new StringBuilder($"<table{_TableAttHtml}>");
                if (this._HtmlTableHelperBuilder != null)
                {
                    var attrs = this._HtmlTableHelperBuilder.Caption.Attributes;
                    var htmlAtt = attrs != null
                    ? string.Join("", HtmlTableGeneraterFactory.AttributeToHtml(attrs).Select(s => $" {s.Key}=\"{Encode(s.Value)}\" "))
                    : "";
                    html.Append($"<caption{htmlAtt}>{_HtmlTableHelperBuilder.Caption.Content}</caption>");
                }
                    
                html.Append($"<thead><tr{_TrAttHtml}>{thead}</tr></thead>");
                html.Append($"<tbody>{tbody.ToString()}</tbody>");
                html.Append("</table>");
                return html;
            }

            public string ToHtmlTableByDataTable(System.Data.DataTable dt) //Not Support Annotation
            {
                //Head
                var thead = new StringBuilder();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string thInnerHTML = Encode(dt.Columns[i].ColumnName);
                    thead.Append($"<th>{thInnerHTML}</th>");
                }

                //Body
                var tbody = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tbody.Append($"<tr{_TrAttHtml}>");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var value = dt.Rows[i][j];
                        string tdInnerHTML = Encode(value);
                        tbody.Append($"<td{_TdAttHtml}>{tdInnerHTML}</td>");
                    }
                    tbody.Append("</tr>");
                }

                //Table html
                var html = RenderHtmlTable(thead, tbody);

                return html.ToString();
            }

            public string ToHtmlTableByProperties<T>(IEnumerable<T> enums)
            {
                var firstData = enums.FirstOrDefault();
                var type = firstData?.GetType();
                var props = GetGetPropertiesByAttrSkipFiliter(type);

#region Check
                if (props.Count == 0)
                {
                    throw new Exception("At least one Property");
                }
#endregion

                //Head
                var thead = new StringBuilder();
                foreach (var p in props)
                {
                    var costomAtt = CustomAttributeHelper.GetCustomAttributeByProperty(_customAttributes, p);
                    string thInnerHTML = costomAtt != null ? costomAtt.DisplayName : Encode(p.Name);
                    thead.Append($"<th>{thInnerHTML}</th>");
                }

                //Body
                var tbody = new StringBuilder();
                foreach (var e in enums)
                {
                    tbody.Append($"<tr{_TrAttHtml}>");
                    foreach (var prop in props)
                    {
                        var value = prop.GetToStringValue(e);
                        string tdInnerHTML = Encode(value);
                        tbody.Append($"<td{_TdAttHtml}>{tdInnerHTML}</td>");
                    }
                    tbody.Append("</tr>");
                }

                //Table html
                var html = RenderHtmlTable(thead, tbody);

                return html.ToString();
            }

            private IList<System.Reflection.PropertyInfo> GetGetPropertiesByAttrSkipFiliter(Type type)
            {
                var props = type.GetPropertiesFromCache();
                _customAttributes = CustomAttributeHelper.GetCustomAttributes(type);
                if (_customAttributes.FirstOrDefault() != null)
                {
                    _customAttributes = _customAttributes.Where(attr => attr.Skip == false);
                    var notSkipAttrsName = _customAttributes.Select(attr => attr.memberInfo.Name);
                    props = props.Where(prop => notSkipAttrsName.Contains(prop.Name)).ToArray();
                }
                return props;
            }

            //Q:    Why use two overload ToHtmlTableByKeyValue , it looks like same logic?
            //A:    Because IDictionary<TKey, TValue> and IDictionary they are not same type.
            public string ToHtmlTableByKeyValue<TKey, TValue>(IEnumerable<IDictionary<TKey, TValue>> enums)
            {
                //Head
                var thead = new StringBuilder();
                foreach (var p in enums.First().Keys)
                {
                    string thInnerHTML = Encode(p);
                    thead.Append($"<th>{thInnerHTML}</th>");
                }

                //Body
                var tbody = new StringBuilder();
                foreach (var values in enums)
                {
                    tbody.Append($"<tr{_TrAttHtml}>");
                    foreach (var v in values)
                    {
                        string tdInnerHTML = Encode(v.Value);
                        tbody.Append($"<td{_TdAttHtml}>{tdInnerHTML}</td>");
                    }
                    tbody.Append("</tr>");
                }

                //Table html
                var html = RenderHtmlTable(thead, tbody);
                return html.ToString();
            }

            public string ToHtmlTableByKeyValue(IEnumerable<IDictionary> enums)
            {
                //Head
                var thead = new StringBuilder();
                foreach (var p in enums.First().Keys)
                {
                    string thInnerHTML = Encode(p);
                    thead.Append($"<th>{thInnerHTML}</th>");
                }

                //Body
                var tbody = new StringBuilder();
                foreach (var values in enums)
                {
                    tbody.Append($"<tr{_TrAttHtml}>");
                    foreach (var v in values.Values)
                    {
                        string tdInnerHTML = Encode(v);
                        tbody.Append($"<td{_TdAttHtml}>{tdInnerHTML}</td>");
                    }
                    tbody.Append("</tr>");
                }

                //Table html
                var html = RenderHtmlTable(thead, tbody);
                return html.ToString();
            }

            private string Encode(object obj)
            {
                if(obj!=null)
                    return _HtmlTableSetting.IsHtmlEncodeMode ? Utilities.HtmlUtils.HtmlEncode(obj.ToString()) : obj.ToString();
                else
                    return "";
            }
        }
    }
}
