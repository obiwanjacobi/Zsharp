using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    [TestClass]
    public class AstVariableTests
    {
        [TestMethod]
        public void DiscardVariableAssign()
        {
            const string code =
                "fn: (): U8" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine +
                "_ = fn()" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionReference>(1);
            fn.EnforceReturnValueUse.Should().BeFalse();
        }

        [TestMethod]
        public void StructFieldAccess()
        {
            const string code =
                "MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Id: U32" + Tokens.NewLine +
                "fn: (s: MyStruct): U32" + Tokens.NewLine +
                Tokens.Indent1 + "return s.Id" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(1);
            var br = fn.CodeBlock.LineAt<AstBranchExpression>(0);
            var fld = br.Expression.RHS.VariableReference;
            fld.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionCallSelf()
        {
            const string code =
                "MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Id: U32" + Tokens.NewLine +
                "fn: (s: MyStruct): U32" + Tokens.NewLine +
                Tokens.Indent1 + "return s.Id" + Tokens.NewLine +
                "s = MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Id = 42" + Tokens.NewLine +
                "s.fn()" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionReference>(3);
            fn.Should().NotBeNull();
            fn.FunctionType.Arguments.First().Identifier
                .Should().Be(AstIdentifierIntrinsic.Self);
        }
    }
}
