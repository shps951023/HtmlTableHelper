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
        public void Display_Test()
        {
            var expected = @"<table><thead><tr><th>Column1</th><th>Column2</th></tr></thead><tbody><tr><td>MyProperty1Value1</td><td>MyProperty1Value2</td></tr><tr><td>MyProperty2Value1</td><td>MyProperty2Value2</td></tr></tbody></table>";
            var sourceData = new Test[] {
                new Test { MyProperty1 = "MyProperty1Value1", MyProperty2 = "MyProperty1Value2" },
                new Test { MyProperty1 = "MyProperty2Value1", MyProperty2 = "MyProperty2Value2" },
            };

            var html = sourceData.ToHtmlTable();
            Assert.AreEqual(html, expected);
        }

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
        public void GetCustomAttribute_RenderHtmlTable_Test()
        {
            var paras = new[] { typeof(Test) };
            var customAttributes = NestedTypeHelper.CallStaticNestedTypeMethod(typeof(HtmlTableHelper)
                , "CustomAttributeHelper", "GetCustomAttributes", paras) as IEnumerable<TableColumnAttribute>;
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
