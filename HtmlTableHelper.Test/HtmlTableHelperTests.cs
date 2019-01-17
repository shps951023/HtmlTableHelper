using Microsoft.VisualStudio.TestTools.UnitTesting;
using HtmlTableHelper;
using System.Linq;

namespace HtmlTableHelper.Test
{
    [TestClass]
    public class HtmlTableHelperTests
    {
        [TestMethod]
        public void ToHtmlTableTest()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Value</th></tr></thead><tbody><tr><td>Test1</td><td>Test2</td></tr></tbody></table>";
            var sourceData = new[] { new { Name = "Test1", Value = "Test2" } };

            var array = sourceData.ToArray().ToHtmlTable(); 
            Assert.AreEqual(array, expected);

            var set = sourceData.ToHashSet().ToHtmlTable();
            Assert.AreEqual(set, expected);

            var list = sourceData.ToList().ToHtmlTable(); 
            Assert.AreEqual(list, expected);

            var enums = sourceData.AsEnumerable().ToHtmlTable(); 
            Assert.AreEqual(enums, expected);
        }
    }
}
