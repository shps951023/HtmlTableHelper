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
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Gender</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>M</td></tr></tbody></table>";
            var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Gender = "M" } };

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
        public void CaptionTest()
        {
            var excepted = "<table><caption id=\"CaptionId\" >This is Caption</caption><thead><tr><th>MyProperty1</th><th>MyProperty2</th></tr></thead><tbody><tr><td>test1</td><td>123</td></tr></tbody></table>";
            var soucreData = new []{
                 new {MyProperty1="test1",MyProperty2=123}
            };
            var html = soucreData.CreateBuilder()
                .SetCaption("This is Caption", new { id = "CaptionId" })
                .ToHtmlTable();
            Assert.AreEqual(excepted, html);
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
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Gender</th><th>Phone</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>M</td><td></td></tr></tbody></table>";
            const string _path = "Test.sqlite";
            SQLiteConnection.CreateFile(_path);
            using (var cn = new SQLiteConnection($"Data Source={_path};Version=3;"))
            {
                var sourceData = cn.Query(@"select 'ITWeiHan' Name,25 Age,'M' Gender,null Phone");
                var html = sourceData.ToHtmlTable();
                Assert.AreEqual(expected, html);
            }
        }


        [TestMethod]
        public void DataTableToHtml_Test()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Gender</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>M</td></tr></tbody></table>";

            DataTable table = GetTestDataTable();

            var html = table.ToHtmlTable();
            Assert.AreEqual(expected, html);
        }

        private static DataTable GetTestDataTable()
        {
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Age", typeof(int));
            table.Columns.Add("Gender", typeof(string));

            table.Rows.Add("ITWeiHan", 25, "M");
            return table;
        }

        [TestMethod]
        public void DictinaryToHtml_Test()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Gender</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>M</td></tr></tbody></table>";
            var sourceData = new[] {
               new Dictionary<string, object> (){
                     {"Name" , "ITWeiHan" }
                    ,{"Age",25}
                    ,{"Gender","M"}
               }
            };
            var html = sourceData.ToHtmlTable();
            Assert.AreEqual(expected, html);
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
            var expected = @"<table class=""SomeClass"" ><thead><tr><th>Name</th><th>Age</th><th>Gender</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>M</td></tr></tbody></table>";
            //IEnumrable
            {
                var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Gender = "M" } };

                var html = sourceData.ToArray().ToHtmlTable(tableAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(expected, html);
            }

            //DataTable
            {
                var sourceData = GetTestDataTable();

                var html = sourceData.ToHtmlTable(tableAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(expected, html);
            }
        }

        [TestMethod]
        public void AttributeTrTest()
        {
            var expected = @"<table><thead><tr class=""SomeClass"" ><th>Name</th><th>Age</th><th>Gender</th></tr></thead><tbody><tr class=""SomeClass"" ><td>ITWeiHan</td><td>25</td><td>M</td></tr></tbody></table>";
            //IEnumrable
            {
                var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Gender = "M" } };

                var html = sourceData.ToArray().ToHtmlTable(trAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(expected, html);
            }

            //DataTable
            {
                var sourceData = GetTestDataTable();

                var html = sourceData.ToHtmlTable(trAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(expected, html);
            }
        }

        [TestMethod]
        public void AttributeThTest()
        {
            var expected = @"<table><thead><tr><th class=""SomeClass"" >Name</th><th class=""SomeClass"" >Age</th><th class=""SomeClass"" >Gender</th></tr></thead><tbody><tr><td>ITWeiHan</td><td>25</td><td>M</td></tr></tbody></table>";
            //IEnumrable
            {
                var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Gender = "M" } };

                var html = sourceData.ToHtmlTable(thAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(expected,html);
            }

            //DataTable
            {
                var sourceData = GetTestDataTable();

                var html = sourceData.ToHtmlTable(thAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(expected,html);
            }
        }

        [TestMethod]
        public void AttributeTdTest()
        {
            var expected = @"<table><thead><tr><th>Name</th><th>Age</th><th>Gender</th></tr></thead><tbody><tr><td class=""SomeClass"" >ITWeiHan</td><td class=""SomeClass"" >25</td><td class=""SomeClass"" >M</td></tr></tbody></table>";

            //IEnumrable
            {
                var sourceData = new[] { new { Name = "ITWeiHan", Age = 25, Gender = "M" } };

                var html = sourceData.ToArray().ToHtmlTable(tdAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(expected, html);
            }

            //DataTable
            {
                var sourceData = GetTestDataTable();

                var html = sourceData.ToHtmlTable(tdAttributes: new { @class = "SomeClass" });
                Assert.AreEqual(expected, html);
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

        /// <summary>
        /// [Issues Found �P Issue #25 ](https://github.com/shps951023/HtmlTableHelper/issues/25)
        /// </summary>
        [TestMethod]
        public void EmptyRow()
        {
            var expected = "<table><thead><tr><th>MyProperty1</th><th>MyProperty2</th></tr></thead><tbody></tbody></table>";
            var sourceData = new TestClass[] {  };
            var result = sourceData.ToHtmlTable();
            Assert.AreEqual(expected,result);
        }
    }
}
