using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace HtmlTableHelper
{
    public static partial class HtmlTableHelper
    {
        private static class ExpressionCache
        {
            private static Dictionary<string, object> ExpressionCaches = new Dictionary<string, object>();
            private readonly static object _Lock = new object();
            public static Func<T, object> GetOrAddExpressionCache<T>(PropertyInfo prop)
            {
                //Done: GetHashCode should be replaced by prop.MetadataToken + prop.Module.ModuleVersionId
                //https://codereview.stackexchange.com/questions/211976/propertyinfo-getvalue-and-expression-cache-getvalue/212056#212056
                //var key = prop?.GetHashCode() ?? throw new ArgumentNullException($"{nameof(prop)} is null");
                var type = typeof(T);
                var key = prop != null ? $"{type.MetadataToken}-{type.Module.ModuleVersionId}-{prop.MetadataToken}-{prop.Module.ModuleVersionId}" : throw new ArgumentNullException($"{nameof(prop)} is null");
                if (!ExpressionCaches.ContainsKey(key))
                {
                    lock (_Lock)
                    {
                        if (!ExpressionCaches.ContainsKey(key))
                            ExpressionCaches[key] = CompileGetValueExpression<T>(prop);
                    }
                }
                return ExpressionCaches[key] as Func<T, object>;
            }

            private static Func<T, object> CompileGetValueExpression<T>(PropertyInfo propertyInfo)
            {
                if (typeof(T) != propertyInfo.DeclaringType)
                {
                    throw new Exception("Parameter T Type should be equals propertyInfo.DeclaringType");
                }
                var instance = Expression.Parameter(propertyInfo.DeclaringType, "i");
                var property = Expression.Property(instance, propertyInfo);
                var convert = Expression.TypeAs(property, typeof(object));
                return Expression.Lambda<Func<T, object>>(convert, instance).Compile();
            }
        }
    }
}
