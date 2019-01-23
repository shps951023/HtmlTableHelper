using System.Collections;
using System.Collections.Generic;

namespace HtmlTableHelper
{
    public static partial class HTMLTableHelper
    {
        public static string ToHtmlTable<T>(this IEnumerable<T> enums, object tableAttributes = null, object trAttributes = null, object tdAttributes = null, HTMLTableSetting HTMLTableSetting = null)
        {
            return ToHtmlTableByIEnumrable(enums, tableAttributes, trAttributes, tdAttributes, HTMLTableSetting);
        }

        public static string ToHtmlTable(this System.Data.DataTable dt, HTMLTableSetting HTMLTableSetting = null)
        {
            var htmltablegenerater = new HtmlTableGenerater(null, null, null, HTMLTableSetting);
            return htmltablegenerater.ToHtmlTablByDataTable(dt);
        }

        private static string ToHtmlTableByIEnumrable<T>(IEnumerable<T> enums, object tableAttributes = null, object trAttributes = null, object tdAttributes = null, HTMLTableSetting HTMLTableSetting = null)
        {
            var htmltablegenerater = new HtmlTableGenerater(tableAttributes, trAttributes, tdAttributes, HTMLTableSetting);

            if (enums is IEnumerable<IDictionary<string, object>>)
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
