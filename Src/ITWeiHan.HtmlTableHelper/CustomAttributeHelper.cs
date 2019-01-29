using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                    if(data!=null)
                        data.memberInfo = prop;
                    yield return data;
                }
            }

            public static TableColumnAttribute GetCustomAttribute(MemberInfo memberInfo)
            {
                return Attribute.GetCustomAttribute(memberInfo, typeof(TableColumnAttribute)) as TableColumnAttribute;
            }

            public static TableColumnAttribute GetCustomAttributeByProperty(
                IEnumerable<TableColumnAttribute> customAttributes
                , MemberInfo memberInfo)
            {
                #region Check
                if (customAttributes == null)
                {
                    throw new ArgumentNullException(nameof(customAttributes));
                }

                if (memberInfo == null)
                {
                    throw new ArgumentNullException(nameof(memberInfo));
                }
                #endregion

                var firstData = customAttributes.FirstOrDefault();
                return firstData == null ? null : customAttributes.Where(w => w.memberInfo.Name == memberInfo.Name).FirstOrDefault();
            }
        }
    }
}
