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
            public static Func<T, string> GetOrAddExpressionCache<T>(PropertyInfo prop)
            {
                //Done: GetHashCode should be replaced by prop.MetadataToken + prop.Module.ModuleVersionId
                //https://codereview.stackexchange.com/questions/211976/propertyinfo-getvalue-and-expression-cache-getvalue/212056#212056
                //var key = prop?.GetHashCode() ?? throw new ArgumentNullException($"{nameof(prop)} is null");
                var key = prop != null ? $"{prop.MetadataToken}-{prop.Module.ModuleVersionId}" : throw new ArgumentNullException($"{nameof(prop)} is null");
                if (!ExpressionCaches.ContainsKey(key))
                {
                    lock (_Lock)
                    {
                        ExpressionCaches[key] = CompileGetValueExpression<T>(prop);
                    }
                }
                return ExpressionCaches[key] as Func<T, string>;
            }

            private static Func<T, string> CompileGetValueExpression<T>(PropertyInfo propertyInfo)
            {
                var instance = Expression.Parameter(propertyInfo.DeclaringType);
                var property = Expression.Property(instance, propertyInfo);
                var toString = Expression.Call(property, "ToString", Type.EmptyTypes);
                return Expression.Lambda<Func<T, string>>(toString, instance).Compile();
            }
        }
    }
}
