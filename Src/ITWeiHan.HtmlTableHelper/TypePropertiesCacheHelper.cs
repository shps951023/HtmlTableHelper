using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HtmlTableHelper
{
    public static partial class HtmlTableHelper
    {
        internal static class TypePropertiesCacheHelper
        {
            internal static readonly System.Collections.Concurrent.ConcurrentDictionary<RuntimeTypeHandle, IList<PropertyInfo>> TypeProperties
                  = new System.Collections.Concurrent.ConcurrentDictionary<RuntimeTypeHandle, IList<PropertyInfo>>();
            internal static readonly System.Collections.Concurrent.ConcurrentDictionary<string, object> TypePropertieValueFunction
                 = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
            private readonly static object _Lock = new object();

            public static IList<PropertyInfo> GetTypePropertiesCache(Type type)
            {
                if (TypeProperties.TryGetValue(type.TypeHandle, out IList<PropertyInfo> pis))
                    return pis;
                return TypeProperties[type.TypeHandle] = type.GetProperties().ToList();
            }


            public static object GetValueFromExpressionCache<T>(System.Type type, System.Reflection.PropertyInfo propertyInfo, T instance)
            {
                var key = propertyInfo != null ? $"{type.MetadataToken.ToString()}|{type.Module.ModuleVersionId.ToString()}|{propertyInfo.MetadataToken.ToString()}" /*ToString避免boxing*/
                     : throw new ArgumentNullException($"{nameof(propertyInfo)} is null");
                Func<T, object> function = null;
                if (TypePropertieValueFunction.TryGetValue(key, out object func))
                {
                    function = func as Func<T, object>;
                }
                else
                {
                    function = CompileGetValueExpression<T>(propertyInfo);
                    lock (_Lock)
                        TypePropertieValueFunction[key] = function;
                }
                return function(instance);
            }

            public static Func<T, object> CompileGetValueExpression<T>(PropertyInfo propertyInfo)
            {
                var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
                var property = Expression.Property(instance, propertyInfo);
                var convert = Expression.TypeAs(property, typeof(object));
                var lambda = Expression.Lambda<Func<T, object>>(convert, instance);
                return lambda.Compile();
            }
        }
    }
}
