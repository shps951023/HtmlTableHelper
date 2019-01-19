[![NuGet](https://img.shields.io/nuget/v/HtmlTableHelper.svg)](https://www.nuget.org/packages/HtmlTableHelper)
![](https://img.shields.io/nuget/dt/HtmlTableHelper.svg)

### Features
- .NET Standard 2.0
- DLL Size Only 8KB
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

### Demo
*Demo Link:*[AspNetMvcDemo](https://htmltablehelperdemo.azurewebsites.net/)
![2019-01-17.10.34.47-image.png](https://raw.githubusercontent.com/shps951023/ImageHosting/master/img/2019-01-17.10.34.47-image.png)

<!--
���w���
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
- [ ] Dapper Demo
- [ ] ASP.NET Core Demo
- [X] Support Dapper Dynamic Query
- [X] Support Dictionary
- [X] Support DataTable

- [ ] Support SQL Helper
- [ ] Support Annotation DisplayName
- [ ] Support All Key/Value Object
- [ ] Support HTML Helper
- [ ] Support i18n
- [ ] Support filter column

