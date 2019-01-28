using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public static class NestedTypeHelper
{
    public static object CallStaticNestedTypeMethod(System.Type type, string nestedClassName, string methodName, object[] parameters)
    {
        var nestedType = type.GetTypeInfo().DeclaredNestedTypes.First(w => w.Name == nestedClassName);
        var methodInfo = nestedType.DeclaredMethods.First(w => w.Name == methodName);
        return methodInfo.Invoke(null, parameters);
    }
}
