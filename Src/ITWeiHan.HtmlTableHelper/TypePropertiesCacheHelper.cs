using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HtmlTableHelper
{
    public static partial class HtmlTableHelper
    {
        internal static class TypePropertiesCacheHelper
        {
            internal static readonly System.Collections.Concurrent.ConcurrentDictionary<RuntimeTypeHandle, IList<PropertyInfo>> TypeProperties
                 = new System.Collections.Concurrent.ConcurrentDictionary<RuntimeTypeHandle, IList<PropertyInfo>>();

            public static IList<PropertyInfo> GetTypePropertiesCache(Type type)
            {
                if (TypeProperties.TryGetValue(type.TypeHandle, out IList<PropertyInfo> pis))
                    return pis;
                return TypeProperties[type.TypeHandle] = type.GetProperties().ToList();
            }
        }
    }
}
