using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Semantics
{
    [TestClass]
    public class ResolveArrayTest
    {
        [TestMethod]
        public void ConstructorCapacity()
        {
            const string code =
                "arr = Array<U8>(10)" + Tokens.NewLine
                ;

            var file = Compile.File(code);

        }
    }
}
