using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HtmlTableHelper.Test
{
    [TestClass]
    public class NestedTypeHelperTests
    {
        private static class TestClass
        {
            private static class NestedTestClass
            {
                private static string[] TestMethod(string para1, string para2)
                {
                    return new string[] { para1, para2 };
                }
            }
        }

        [TestMethod]
        public void NestedTypeHelper_CallStaticMethod_Test()
        {
            var excepted = new string[]{"para1", "para2"};
            var paras = NestedTypeHelper.CallStaticNestedTypeMethod(
                type: typeof(TestClass), nestedClassName: "NestedTestClass", methodName: "TestMethod"
                , parameters: excepted
            ) as string[];

            Assert.AreEqual(paras[0], excepted[0]);
            Assert.AreEqual(paras[1], excepted[1]);
        }

        [TestMethod]
        public void NestedTypeHelper_CallStaticMethod_WrongClassNameAndMethodName_Test()
        {
            //WrongClassName
            try
            {
                NestedTypeHelper.CallStaticNestedTypeMethod(
                    type: typeof(TestClass), nestedClassName: "xxx", methodName: "TestMethod"
                );
            }
            catch (System.Exception e)
            {
                Assert.AreEqual(e.Message, "nestedClassName xxx's nestedType not found.");
            }
            //WrongMethodName
            try
            {
                NestedTypeHelper.CallStaticNestedTypeMethod(
                    type: typeof(TestClass), nestedClassName: "NestedTestClass", methodName: "xxx"
                );
            }
            catch (System.Exception e)
            {
                Assert.AreEqual(e.Message, "methodName xxx's method not found.");
            }
        }
    }
}
