using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HtmlTableHelper.Test
{
    #region TestData
    public class Test
    {
        [TableColumn(DisplayName = "Column1", Skip =true)]
        public string MyProperty1 { get; set; }
    }
    #endregion

    [TestClass]
    public class CustomAttributeTests
    {
        [TestMethod]
        public void DisplayName_Test()
        {
            var excepted = new TableColumnAttribute() { DisplayName = "Column1", Skip = true };

            var type = typeof(Test);
            var datas = NestedTypeHelper.CallStaticNestedTypeMethod(typeof(HtmlTableHelper)
                , "CustomAttribute", "GetCustomAttributes", new[] { type }) as IEnumerable<TableColumnAttribute>;
            var data = datas.First();

            Assert.AreEqual(data.DisplayName,excepted.DisplayName);
        }
    }
}
