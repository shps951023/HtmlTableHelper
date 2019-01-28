
using System.Collections.Generic;
using HtmlTableHelper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;

public static class IHelperExtension
{
    public static HtmlString ToHtmlTable<T>(this IHtmlHelper htmlHelper
        , IEnumerable<T> enums)
    {
        var html = enums.ToHtmlTable();
        return new HtmlString(html);
    }
}