using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using HtmlTableHelper;


namespace BenchmarkTest
{
    public  static class Program
    {
        public static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(Program).Assembly).Run(args, new Config());
            Console.Read();
        }
    }

    public class BenchmarkBase
    {
        private static List<MyClass> Data = Enumerable.Range(1,100).Select(s=>new MyClass() { MyProperty1 = 123, MyProperty2 = "test" }).ToList();

        [Benchmark()]
        public void ToHtmlTable() => Data.ToHtmlTable();
        
    }

    public class MyClass
    {
        public int MyProperty1 { get; set; }
        public string MyProperty2 { get; set; }
    }
}
