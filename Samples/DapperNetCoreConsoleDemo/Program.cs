using System;
using System.Data.SQLite;
using System.IO;
using Dapper;
using HtmlTableHelper;

namespace ITWeiHan.DapperNetCoreConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            const string _path = "Test.sqlite";
            SQLiteConnection.CreateFile(_path);
            using (var cn = new SQLiteConnection($"Data Source={_path};Version=3;"))
            {
                var sourceData = cn.Query(@"select 'ITWeiHan' Name,25 Age,'Taiwan' Country");
                var tablehtml = sourceData.ToHtmlTable();
                Console.WriteLine(tablehtml);
            }
            Console.ReadKey();
        }
    }
}
