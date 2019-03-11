using System.Collections;
using System.Collections.Generic;

namespace HtmlTableHelper
{
    public static partial class HtmlTableHelper
    {
        public static string ToHtmlTable<T>(this IEnumerable<T> enums, object tableAttributes = null, object trAttributes = null, object tdAttributes = null, HtmlTableSetting HTMLTableSetting = null)
        {
            return ToHtmlTableByIEnumrable(enums, tableAttributes, trAttributes, tdAttributes, HTMLTableSetting);
        }

        public static string ToHtmlTable(this System.Data.DataTable datatable, object tableAttributes = null, object trAttributes = null, object tdAttributes = null, HtmlTableSetting HTMLTableSetting = null)
        {
            var htmltablegenerater = HtmlTableGeneraterFactory.CreateInstance(tableAttributes, trAttributes, tdAttributes, HTMLTableSetting);
            return htmltablegenerater.ToHtmlTableByDataTable(datatable);
        }

        private static string ToHtmlTableByIEnumrable<T>(IEnumerable<T> enums, object tableAttributes = null, object trAttributes = null, object tdAttributes = null, HtmlTableSetting HTMLTableSetting = null)
        {
            var htmltablegenerater = HtmlTableGeneraterFactory.CreateInstance(tableAttributes, trAttributes, tdAttributes, HTMLTableSetting);

            // Q:   Why not only IEnumerable<IDictionary> ?
            // A:   Example Dapper Dynamic Query Only implement IDictionary<string,object> without IDictionary
            // Q:   Why not use overload ToHtmlTable<TKey,TValue>(this IEnumerable<Dictionary<Tkey,TValue>> enums)?
            // A:   Because ToHtmlTable<T>(this IEnumerable<T> enums) and ToHtmlTable<TKey,TValue>(this IEnumerable<Dictionary<Tkey,TValue>> enums)
            //      System prefer use the former
            //      ps. https://stackoverflow.com/questions/54251262/c-sharp-overload-key-value-and-non-key-value-type-using-var-without-specifying
            if (enums is IEnumerable<IDictionary<string, object>>) //Special for Dapper Dynamic Query
            {
                return htmltablegenerater.ToHtmlTableByKeyValue(enums as IEnumerable<IDictionary<string, object>>);
            }
            else if (enums is IEnumerable<IDictionary>)
            {
                return htmltablegenerater.ToHtmlTableByKeyValue(enums as IEnumerable<IDictionary>);
            }
            else
            {
                return htmltablegenerater.ToHtmlTableByProperties(enums);
            }
        }
    }
}
