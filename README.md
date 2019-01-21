[![NuGet](https://img.shields.io/nuget/v/HtmlTableHelper.svg)](https://www.nuget.org/packages/HtmlTableHelper)
![](https://img.shields.io/nuget/dt/HtmlTableHelper.svg)

### Features
- .NET Standard 2.0 (Support ASP.NET MVC5 / ASP.NET Core..)
- DLL Size Only 9KB
- Without JSON.NET
- Support Anonymous Types

### Installation

You can install the package [from NuGet](https://www.nuget.org/packages/HtmlTableHelper) using the Visual Studio Package Manager or NuGet UI:

```cmd
PM> install-package HtmlTableHelper
```

or the `dotnet` command line:

```cmd
dotnet add package HtmlTableHelper
```

### Get Start

List/Array/Set/Enumrable non Key/Value Type Example
```C#
using HtmlTableHelper;
..
var sourceData = new[] { new { Name = "ITWeiHan", Age = "25",Country = "Taiwan" } };
var tablehtml = sourceData.ToHtmlTable();
/*
Result:
<table><thead><tr><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>Taiwan</td></tr></tbody></table>
*/
```

Dapper Example
```C#
using (var cn = "Your Connection")
{
	var sourceData = cn.Query(@"select 'ITWeiHan' Name,25 Age,'Taiwan' Country");
	var tablehtml = sourceData.ToHtmlTable();
}
```

Dictionary Example
```C#
var sourceData = new[] {
    new Dictionary<string, object> (){
        {"Name" , "ITWeiHan" },{"Age",25},{"Country","Taiwan"}
    }
};
var tablehtml = sourceData.ToHtmlTable();
```

**HTMLTableSetting**  
Configurable InnerHtml Encoding (Recommended not to do so without a specific reason,because XSS Attack)
```C#
var sourceData = new[] { new { Name = "<b>ITWeiHan</b>" } };

//Default Encoding
var encodinghtml = sourceData.ToHtmlTable();
//Result: <table>..&lt;b&gt;ITWeiHan&lt;/b&gt;..</table>

var htmltablesetting = new HTMLTableSetting()
{
    IsHtmlEncodeMode = false
};
var notEncodinghtml = sourceData.ToHtmlTable(HTMLTableSetting: htmltablesetting);
//Result: <table>..<b>ITWeiHan</b>..</table>
```


### Demo
**ASP.NET MVC 5 JQuery DataTable Demo:**  
*Demo Link:*[ASP.NET MVC 5 Demo](https://htmltablehelperdemo.azurewebsites.net/)
![2019-01-17.10.34.47-image.png](https://raw.githubusercontent.com/shps951023/ImageHosting/master/img/2019-01-17.10.34.47-image.png)
```C#
using HtmlTableHelper;
//..
public class HomeController : Controller
{
    public ActionResult Index()
    {
        var datas = new[] { new { Name = "ITWeiHan", Age = "25",Country = "Taiwan" } };
        ViewBag.Table = datas.ToHtmlTable();
        return View();
    }
}
```

```C#
@{
    Layout = null;
}

<!DOCTYPE html>

<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>AspNetMvcDemo</title>
    <link href="//cdn.datatables.net/1.10.19/css/jquery.dataTables.min.css" rel="stylesheet" />
</head>
<body>
    <div>
        @Html.Raw(ViewBag.Table)
    </div>
    <script src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-2.2.0.min.js"></script>
    <script src="//cdn.datatables.net/1.10.19/js/jquery.dataTables.min.js"></script>
    <script>
        $(document).ready(function () {
            $('Table').DataTable();
        });
    </script>
</body>
</html>
```


**ASP.NET Core Demo:**
```C#
public class Startup
{
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        app.Run(async (context) =>
        {
            var sourceData = new[] {
                new Dictionary<string, object> (){
                    {"Name" , "ITWeiHan" },{"Age",25},{"Country","Taiwan"}
                }
            };
            var tablehtml = sourceData.ToHtmlTable();
            await context.Response.WriteAsync(tablehtml);
        });
    }
}
```

<!--
«ü©wÄæ¦ì
```C#
using HtmlTableHelper;
..
public class Person{
	public string Name { get; set; }
	public int Age { get; set; }
	public string Country { get; set; }
}
..
var sourceData = new[] { new Person{ Name = "ITWeiHan", Age = "25",Country = "Taiwan" } };
var tablehtml = sourceData.ToHtmlTable(new[]{name});
/*
Result:
<table><thead><tr><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>Taiwan</td></tr></tbody></table>
*/
```
-->

### TODO
- [X] NuGet
- [X] JQuery DataTable Demo
- [X] Dapper Demo
- [X] ASP.NET Core Demo
- [X] Support Dapper Dynamic Query
- [X] Support Dictionary
- [X] Support DataTable

- [ ] Support SQL Helper
- [ ] Support Annotation DisplayName
- [ ] Support All Key/Value Object
- [ ] Support MVC HTML Helper
- [ ] Support i18n
- [ ] Support filter column
- [ ] Support Paging

- [X] Defalut html encode prevent xss
<!--
Read This Page
[security - Will HTML Encoding prevent all kinds of XSS attacks? - Stack Overflow]
(https://stackoverflow.com/questions/53728/will-html-encoding-prevent-all-kinds-of-xss-attacks)
-->