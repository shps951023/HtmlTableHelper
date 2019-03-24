using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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

        public class TestClass
        {
            public string MyProperty1 { get; set; }
            public string MyProperty2 { get; set; }
        }

        [TestMethod]
        public void Prop_Is_Null_Test()
        {
            var excepted = "<table><thead><tr><th>MyProperty1</th><th>MyProperty2</th></tr></thead><tbody><tr><td>test1</td><td></td></tr></tbody></table>";
            var soucreData = new TestClass[]{
                 new TestClass(){MyProperty1="test1",MyProperty2=null}
            };
            var html = soucreData.ToHtmlTable();
            Assert.AreEqual(excepted, html);
        }

        [TestMethod]
        public void Cache_Test()
        {
            for (int i = 0; i < 2; i++)
            {
                IEnumerable_Test();
            }
        }

        [TestMethod]
        public void DapperDynamicQuery_Test()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Country</th><th>Phone</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>Taiwan</td><td></td></tr></tbody></table>";
            const string _path = "Test.sqlite";
            SQLiteConnection.CreateFile(_path);
            using (var cn = new SQLiteConnection($"Data Source={_path};Version=3;"))
            {
                var sourceData = cn.Query(@"select 'ITWeiHan' Name,25 Age,'Taiwan' Country,null Phone");
                var html = sourceData.ToHtmlTable();
                Assert.AreEqual(html, expected);
            }
        }


        [TestMethod]
        public void DataTableToHtml_Test()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>Taiwan</td></tr></tbody></table>";

            DataTable table = GetTestDataTable();

            var html = table.ToHtmlTable();
            Assert.AreEqual(html, expected);
        }

        private static DataTable GetTestDataTable()
        {
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Age", typeof(int));
            table.Columns.Add("Country", typeof(string));

            table.Rows.Add("ITWeiHan", 25, "Taiwan");
            return table;
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
            var sourceData = new[] { new { Name = "<script>alert('XSS')</script>" } };
            var html = sourceData.ToHtmlTable();
            Assert.AreEqual(expected, html);
        }

        [TestMethod]
        public void NonEncodeModeTest()
        {
            var expected = @"<table><thead><tr><th>Name</th></tr></thead><tbody><tr><td><b>ITWeiHan</b></td></tr></tbody></table>";
            var htmltablesetting = new HtmlTableSetting()
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
            //IEnumrable
            {
                var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Country = "Taiwan" } };

                var html = sourceData.ToArray().ToHtmlTable(tableAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(html, expected);
            }

            //DataTable
            {
                var sourceData = GetTestDataTable();

                var html = sourceData.ToHtmlTable(tableAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(html, expected);
            }
        }

        [TestMethod]
        public void AttributeTrTest()
        {
            var expected = @"<table><thead><tr class=""SomeClass"" ><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr class=""SomeClass"" ><td>ITWeiHan</td><td>25</td><td>Taiwan</td></tr></tbody></table>";
            //IEnumrable
            {
                var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Country = "Taiwan" } };

                var html = sourceData.ToArray().ToHtmlTable(trAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(html, expected);
            }

            //DataTable
            {
                var sourceData = GetTestDataTable();

                var html = sourceData.ToHtmlTable(trAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(html, expected);
            }
        }

        [TestMethod]
        public void AttributeTdTest()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Country</th></tr></thead><tbody><tr><td class=""SomeClass"" >ITWeiHan</td><td class=""SomeClass"" >25</td><td class=""SomeClass"" >Taiwan</td></tr></tbody></table>";

            //IEnumrable
            {
                var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Country = "Taiwan" } };

                var html = sourceData.ToArray().ToHtmlTable(tdAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(html, expected);
            }

            //DataTable
            {
                var sourceData = GetTestDataTable();

                var html = sourceData.ToHtmlTable(tdAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(html, expected);
            }
        }

        [TestMethod]
        public void AttributeHtmlEncodeTest()
        {
            var expected = "<table class=\"SomeClass&quot; onclick=alert(&#39;XSS&#39;) &quot;\" ><thead><tr><th>Name</th></tr></thead><tbody><tr><td>ITWeiHan</td></tr></tbody></table>";
            var sourceData = new[] { new { Name = "ITWeiHan" } };
            var array = sourceData.ToArray().ToHtmlTable(tableAttributes: new { @class = "SomeClass\" onclick=alert('XSS') \"" });
            Assert.AreEqual(array, expected);
        }
    }
}
