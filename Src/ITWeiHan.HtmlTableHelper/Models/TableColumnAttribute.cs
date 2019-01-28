using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HtmlTableHelper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TableColumnAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public bool Skip { get; set; }
        public MemberInfo memberInfo { get; set; }
    }
}
