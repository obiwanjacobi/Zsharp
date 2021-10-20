using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zsharp.Compiler.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Compile.File(@"CodeFiles\CodeFile1.zs", "TestMethod1");
        }
    }
}
