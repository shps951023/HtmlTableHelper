#### features
- .NET Standard 2.0
- DLL Only 6KB

#### Get Start

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

#### Demo
*Demo Link:*[AspNetMvcDemo](https://htmltablehelperdemo.azurewebsites.net/)
![2019-01-17.10.34.47-image.png](https://raw.githubusercontent.com/shps951023/ImageHosting/master/img/2019-01-17.10.34.47-image.png)

#### TODO
- [ ] NuGet
- [ ] Support SQL Helper
- [X] JQuery DataTable Demo
- [X] Tests
      