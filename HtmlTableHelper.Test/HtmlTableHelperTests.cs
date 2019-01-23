using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HtmlTableHelper.Test
{
    [TestClass]
    public class HtmlTableHelperTests
    {
        [TestMethod]
        public void Non_Property_Test()
        {
            var datas = new[] { new { } };
            try
            {
                var html = datas.ToHtmlTable();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "At least one Property");
            }
        }

        [TestMethod]
        public void IEnumerable_Test()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>Taiwan</td></tr></tbody></table>";
            var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Country = "Taiwan" } };

            var array = sourceData.ToArray().ToHtmlTable();
            Assert.AreEqual(array, expected);

            var set = sourceData.ToHashSet().ToHtmlTable();
            Assert.AreEqual(set, expected);

            var list = sourceData.ToList().ToHtmlTable();
            Assert.AreEqual(list, expected);

            var enums = sourceData.AsEnumerable().ToHtmlTable();
            Assert.AreEqual(enums, expected);
        }

        [TestMethod]
        public void DataTableToHtml_Test()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>Taiwan</td></tr></tbody></table>";

            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Age", typeof(int));
            table.Columns.Add("Country", typeof(string));

            table.Rows.Add("ITWeiHan", 25, "Taiwan");

            var html = table.ToHtmlTable();
            Assert.AreEqual(html, expected);
        }

        [TestMethod]
        public void DictinaryToHtml_Test()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>Taiwan</td></tr></tbody></table>";
            var sourceData = new[] {
               new Dictionary<string, object> (){
                     {"Name" , "ITWeiHan" }
                    ,{"Age",25}
                    ,{"Country","Taiwan"}
               }
            };
            var html = sourceData.ToHtmlTable();
            Assert.AreEqual(html, expected);
        }

        [TestMethod]
        public void EncodeProventXSS()
        {
            var expected = @"<table><thead><tr><th>Name</th></tr></thead><tbody><tr><td>&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;</td></tr></tbody></table>";
            var sourceData = new[] {new {Name = "<script>alert('XSS')</script>"  }};
            var html = sourceData.ToHtmlTable();
            Assert.AreEqual(expected, html);
        }

        [TestMethod]
        public void NonEncodeModeTest()
        {
            var expected = @"<table><thead><tr><th>Name</th></tr></thead><tbody><tr><td><b>ITWeiHan</b></td></tr></tbody></table>";
            var htmltablesetting = new HTMLTableSetting()
            {
                IsHtmlEncodeMode = false
            };
            var sourceData = new[] { new { Name = "<b>ITWeiHan</b>" } };
            var html = sourceData.ToHtmlTable(HTMLTableSetting: htmltablesetting);
            Assert.AreEqual(expected, html);
        }

        [TestMethod]
        public void AttributeTableTest()
        {
            var expected = @"<table class=""SomeClass"" ><thead><tr><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>Taiwan</td></tr></tbody></table>";
            var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Country = "Taiwan" } };

            var array = sourceData.ToArray().ToHtmlTable(tableAttributes:new {@class="SomeClass"});
            Assert.AreEqual(array, expected);
        }

        [TestMethod]
        public void AttributeTrTest()
        {
            var expected = @"<table><thead><tr class=""SomeClass"" ><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr class=""SomeClass"" ><td>ITWeiHan</td><td>25</td><td>Taiwan</td></tr></tbody></table>";
            var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Country = "Taiwan" } };

            var array = sourceData.ToArray().ToHtmlTable(trAttributes: new { @class = "SomeClass" });
            Assert.AreEqual(array, expected);
        }

        [TestMethod]
        public void AttributeTdTest()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr><td class=""SomeClass"" >ITWeiHan</td><td class=""SomeClass"" >25</td><td class=""SomeClass"" >Taiwan</td></tr></tbody></table>";
            var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Country = "Taiwan" } };

            var array = sourceData.ToArray().ToHtmlTable(tdAttributes: new { @class = "SomeClass" });
            Assert.AreEqual(array, expected);
        }
    }
}
