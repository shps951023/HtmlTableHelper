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
    public static partial class HTMLTableHelper
    {
        private class HtmlTableGenerater
        {
            private static readonly HTMLTableSetting _DefualtHTMLTableSetting = new HTMLTableSetting()
            {
                IsHtmlEncodeMode = true
            };

            #region Prop
            private HTMLTableSetting _htmlTableSetting { get; set; }
            private Dictionary<string,string> _tableAttributes { get; set; }
            private Dictionary<string, string> _trAttributes { get; set; }
            private Dictionary<string, string> _tdAttributes { get; set; }
            #endregion

            public HtmlTableGenerater(object tableAttributes, object trAttributes, object tdAttributes, HTMLTableSetting htmlTableSetting)
            {
                this._tableAttributes = AttributeToHtml(tableAttributes);
                this._trAttributes = AttributeToHtml(trAttributes);
                this._tdAttributes = AttributeToHtml(tdAttributes);
                this._htmlTableSetting = htmlTableSetting ?? _DefualtHTMLTableSetting;
            }

            private Dictionary<string, string> AttributeToHtml(object tableAttributes)
            {
                if (tableAttributes == null) return null;
                var type = tableAttributes.GetType();
                var dic = type.GetProperties()
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

                var bodys = new List<List<object>>();
                foreach (var e in enums)
                {
                    bodys.Add(props.Select(prop =>
                    {
                        var func = GetOrAddExpressionCache<T>(prop);
                        var value = func(e);
                        return value as object;
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
                var tableAttHtml = _tableAttributes != null
                    ? string.Join("", _tableAttributes.Select(s => $" {s.Key}=\"{HttpUtility.HtmlEncode(s.Value)}\" "))
                    : "";
                var trAttHtml = _trAttributes != null
                    ? string.Join("", _trAttributes.Select(s => $" {s.Key}=\"{HttpUtility.HtmlEncode(s.Value)}\" "))
                    : "";
                var tdAttHtml = _tdAttributes != null
                    ? string.Join("", _tdAttributes.Select(s => $" {s.Key}=\"{HttpUtility.HtmlEncode(s.Value)}\" "))
                    : "";
                #endregion

                var html = new StringBuilder($"<table{tableAttHtml}>");

                //Head
                html.Append($"<thead><tr{trAttHtml}>");
                foreach (var p in heads)
                {
                    var thInnerHTML = _htmlTableSetting.IsHtmlEncodeMode ? HttpUtility.HtmlEncode(p.ToString()) : p.ToString();
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
                        var tdInnerHTML = _htmlTableSetting.IsHtmlEncodeMode
                            ? HttpUtility.HtmlEncode(v.ToString())
                            : v.ToString();
                        html.Append($"<td{tdAttHtml}>{tdInnerHTML}</td>");
                    }
                    html.Append("</tr>");
                }
                html.Append("</tbody>");

                html.Append("</table>");
                return html;
            }

            #region Cache
            public static Dictionary<int, object> ExpressionCaches = new Dictionary<int, object>();
            private Func<T, string> GetOrAddExpressionCache<T>(PropertyInfo prop)
            {
                var key = prop?.GetHashCode() ?? throw new ArgumentNullException($"{nameof(prop)} is null");
                if (!ExpressionCaches.ContainsKey(key))
                {
                    ExpressionCaches[key] = GetValueGetter<T>(prop);
                }

                return ExpressionCaches[key] as Func<T, string>;
            }

            private static Func<T, string> GetValueGetter<T>(PropertyInfo propertyInfo)
            {
                var instance = Expression.Parameter(propertyInfo.DeclaringType);
                var property = Expression.Property(instance, propertyInfo);
                var toString = Expression.Call(property, "ToString", Type.EmptyTypes);
                return Expression.Lambda<Func<T, string>>(toString, instance).Compile();
            }
            #endregion
        }
    }
}
