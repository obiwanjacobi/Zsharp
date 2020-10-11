using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.AST
{
    [TestClass]
    public class AstCodeTests
    {
        [TestMethod]
        public void Code1()
        {
            const string code =
                "// comment" + Tokens.NewLine +
                "export MyFunction" + Tokens.NewLine +
                Tokens.NewLine +
                "MyFunction: (a: U8): Bool" + Tokens.NewLine +
                Tokens.Indent1 + "if a = 42" + Tokens.NewLine +
                Tokens.Indent2 + "return true" + Tokens.NewLine +
                Tokens.Indent1 + "return false" + Tokens.NewLine
                ;

            Build.File(code).Should().NotBeNull();
        }
    }
}
