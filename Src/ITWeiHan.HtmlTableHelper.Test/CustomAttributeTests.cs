using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HtmlTableHelper.Test
{
    [TestClass]
    public class CustomAttributeTests
    {
        #region TestData
        private class Test
        {
            [TableColumn(DisplayName = "Column1", Skip = true)]
            public string MyProperty1 { get; set; }
            [TableColumn(DisplayName = "Column2", Skip = false)]
            public string MyProperty2 { get; set; }
        }
        #endregion

        [TestMethod]
        public void GetCustomAttribute_Test()
        {
            var excepted = new[]{
                new TableColumnAttribute() { DisplayName = "Column1", Skip = true },
                new TableColumnAttribute() { DisplayName = "Column2", Skip = false }
            };
            var paras =  new[] { typeof(Test) };
            var customAttributes = NestedTypeHelper.CallStaticNestedTypeMethod(typeof(HtmlTableHelper)
                , "CustomAttributeHelper", "GetCustomAttributes", paras) as IEnumerable<TableColumnAttribute>;

            customAttributes.Select((attribute, index) =>
            {
                Assert.AreEqual(attribute.DisplayName, excepted[index].DisplayName);
                Assert.AreEqual(attribute.Skip, excepted[index].Skip);
                return true;
            }).ToArray();
        }
        
        [TestMethod]
        public void RenderHtmlTable_Test()
        {
            var type = typeof(Test);
            var customAttributes = (NestedTypeHelper.CallStaticNestedTypeMethod(typeof(HtmlTableHelper)
                , "CustomAttributeHelper", "GetCustomAttributes", new[] { type }) as IEnumerable<TableColumnAttribute>).ToList();
            var exceptedHtml = @"<table><tr><th>Column1<th><th>Column2<th></tr><tr><td>Row1Column1</td><td>Row1Column2</td></tr><tr><td>Row2Column1</td><td>Row2Column2</td></tr></table>";
            var sourceData = new Test[] { new Test { MyProperty1 = "Row1Column1", MyProperty2 = "Row1Column2" },
                new Test { MyProperty1 = "Row2Column1", MyProperty2 = "Row2Column2" }
            };
            var th = string.Join(string.Empty, customAttributes.Select(s => $"<th>{s.DisplayName}<th>"));
            var td = string.Join(string.Empty, sourceData.Select(s => $"<tr><td>{s.MyProperty1}</td><td>{s.MyProperty2}</td></tr>"));
            var html = $@"<table><tr>{th}</tr>{td}</table>".ToString();
            Assert.AreEqual(exceptedHtml, html);
        }
    }
}
