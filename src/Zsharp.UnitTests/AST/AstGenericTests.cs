using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    [TestClass]
    public class AstGenericTests
    {
        [TestMethod]
        public void GenericTypeDefinition()
        {
            const string code =
                "MyStruct<T>" + Tokens.NewLine +
                Tokens.Indent1 + "Id: T" + Tokens.NewLine +
                Tokens.Indent1 + "Name: Str" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var s = file.CodeBlock.LineAt<AstTypeDefinitionStruct>(0);
            s.Symbol.Definition.Should().Be(s);
            s.BaseType.Should().BeNull();
            s.IsGeneric.Should().BeTrue();
            var tp = s.GenericParameters.First();
            tp.Identifier.Name.Should().Be("T");

            var f = s.Fields.First();
            f.Identifier.Name.Should().Be("Id");
            f.TypeReference.Should().NotBeNull();
            f.TypeReference.IsGenericParameter.Should().BeTrue();
            f.Symbol.Definition.Should().Be(f);
        }

        [TestMethod]
        public void GenericFunctionDefinition()
        {
            const string code =
                "fn: <T>(c: U16): T" + Tokens.NewLine +
                Tokens.Indent1 + "return c.T()" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            fn.IsGeneric.Should().BeTrue();
            var tp = fn.GenericParameters.First();
            tp.Identifier.Name.Should().Be("T");
        }
    }
}
