using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstFunctionTests
    {
        [TestMethod]
        public void FunctionCallParameters()
        {
            const string code =
                "fn1: (p: U8): U8" + Tokens.NewLine +
                Tokens.Indent1 + "return p" + Tokens.NewLine +
                "fn2: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn1(42)" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn2 = file.CodeBlock.ItemAt<AstFunctionDefinition>(1);

        }
    }
}
