using System.Collections;
using System.Collections.Generic;

namespace HtmlTableHelper
{
    public static partial class HtmlTableHelper
    {
        /// <summary>
        /// The Custom Table/TR/TD Attributes Way:
        /// var html = data.ToHtmlTable(tableAttributes: new { @class = "SomeClass"}  ,trAttributes: new { ID = "SomeID" },tdAttributes: new { width = "120 px" });
        /// </summary>
        public static string ToHtmlTable<T>(this IEnumerable<T> enums, object tableAttributes = null, object trAttributes = null, object tdAttributes = null, HtmlTableSetting HTMLTableSetting = null)
        {
            return ToHtmlTableByIEnumrable(enums, tableAttributes, trAttributes, tdAttributes, HTMLTableSetting);
        }

        /// <summary>
        /// The Custom Table/TR/TD Attributes Way:
        /// var html = data.ToHtmlTable(tableAttributes: new { @class = "SomeClass"}  ,trAttributes: new { ID = "SomeID" },tdAttributes: new { width = "120 px" });
        /// </summary>
        public static string ToHtmlTable(this System.Data.DataTable datatable, object tableAttributes = null, object trAttributes = null, object tdAttributes = null, HtmlTableSetting HTMLTableSetting = null)
        {
            var htmltablegenerater = new HtmlTableGenerater(tableAttributes, trAttributes, tdAttributes, HTMLTableSetting);
            return htmltablegenerater.ToHtmlTablByDataTable(datatable);
        }

        private static string ToHtmlTableByIEnumrable<T>(IEnumerable<T> enums, object tableAttributes = null, object trAttributes = null, object tdAttributes = null, HtmlTableSetting HTMLTableSetting = null)
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
