using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HtmlTableHelper
{
    public static partial class HtmlTableHelper
    {
        private static class CustomAttributeHelper
        {
            public static IEnumerable<TableColumnAttribute> GetCustomAttributes(System.Type type)
            {
                var props = type.GetProperties();
                foreach (var prop in props)
                {
                    var data = GetCustomAttribute(prop);
                    data.memberInfo = prop;
                    yield return data;
                }
            }

            public static TableColumnAttribute GetCustomAttribute(MemberInfo memberInfo)
            {
                return Attribute.GetCustomAttribute(memberInfo, typeof(TableColumnAttribute)) as TableColumnAttribute;
            }
        }
    }
}
