﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstVariableTests
    {
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
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(1);
            var br = fn.CodeBlock.ItemAt<AstBranchExpression>(0);
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
            var fn = file.CodeBlock.ItemAt<AstFunctionReference>(3);
            fn.Should().NotBeNull();
            fn.Parameters.First().Identifier
                .Should().Be(AstIdentifierIntrinsic.Self);
        }
    }
}