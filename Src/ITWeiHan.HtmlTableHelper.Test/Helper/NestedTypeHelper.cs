using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public static class NestedTypeHelper
{
    public static object CallStaticNestedTypeMethod(System.Type type, string nestedClassName, string methodName, object[] parameters=null)
    {
        #region NullCheck
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (string.IsNullOrWhiteSpace(nestedClassName))
        {
            throw new ArgumentException("message", nameof(nestedClassName));
        }

        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentException("message", nameof(methodName));
        }
        #endregion

        var nestedType = type.GetTypeInfo().DeclaredNestedTypes.SingleOrDefault(w => w.Name == nestedClassName)
            ?? throw new Exception($"{nameof(nestedClassName)} {nestedClassName}'s nestedType not found.");
        
        var methodInfo = nestedType.DeclaredMethods.SingleOrDefault(w => w.Name == methodName)
            ?? throw new Exception($"{nameof(methodName)} {methodName}'s method not found.");
        return methodInfo.Invoke(null, parameters);
    }
}
