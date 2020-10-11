using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstSymbolTableTests
    {
        [TestMethod]
        public void TopVariableDefInit()
        {
            const string code =
                "v: U8 = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var symbols = file.Symbols;
            symbols.Entries.Any(e => e == null).Should().BeFalse();

            var v = symbols.GetEntry("v", AstSymbolKind.Variable);
            v.SymbolKind.Should().Be(AstSymbolKind.Variable);
            var def = v.GetDefinition<AstVariableDefinition>();
            def.TypeReference.Identifier.Name.Should().Be("U8");
        }


        [TestMethod]
        public void TopVariableName()
        {
            const string code =
                "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var symbols = file.Symbols;
            symbols.Entries.Any(e => e == null).Should().BeFalse();

            var v = symbols.GetEntry("v", AstSymbolKind.Variable);
            v.SymbolKind.Should().Be(AstSymbolKind.Variable);
            var def = v.GetDefinition<AstVariableDefinition>();
            def.TypeReference.Should().BeNull();
        }

        [TestMethod]
        public void FunctionName()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var symbols = file.Symbols;
            symbols.Entries.Any(e => e == null).Should().BeFalse();

            var fn = symbols.GetEntry("fn", AstSymbolKind.Function);
            fn.SymbolKind.Should().Be(AstSymbolKind.Function);
        }

        [TestMethod]
        public void FunctionParameterName()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunction>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Entries.Any(e => e == null).Should().BeFalse();

            var p = symbols.GetEntry("p", AstSymbolKind.Parameter);
            p.SymbolKind.Should().Be(AstSymbolKind.Parameter);
        }

        [TestMethod]
        public void FunctionParameterSelf()
        {
            const string code =
                "fn: (self: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunction>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Entries.Any(e => e == null).Should().BeFalse();

            // TODO: no definition is set!?
            //var p = symbols.GetEntry("self", AstSymbolKind.Parameter);
            //p.GetDefinition<AstIdentifier>().Name.Should().Be("self");
        }

        [TestMethod]
        public void FunctionParameterAssignment()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "p = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunction>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Entries.Any(e => e == null).Should().BeFalse();

            symbols.GetEntry("p", AstSymbolKind.Parameter).Should().NotBeNull();
            symbols.GetEntry("p", AstSymbolKind.Variable).Should().NotBeNull();
        }

        [TestMethod]
        public void LocalVariableName()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunction>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Entries.Any(e => e == null).Should().BeFalse();

            var v = symbols.GetEntry("v", AstSymbolKind.Variable);
            v.SymbolKind.Should().Be(AstSymbolKind.Variable);
        }

        [TestMethod]
        public void LocalVariableDefAndUse()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "v: U8" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunction>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Entries.Any(e => e == null).Should().BeFalse();

            var v = symbols.GetEntry("v", AstSymbolKind.Variable);
            v.SymbolKind.Should().Be(AstSymbolKind.Variable);
            v.GetDefinition<AstVariable>().Identifier.IdentifierType
                .Should().Be(AstIdentifierType.Variable);
            v.ReferencesOf<AstVariableReference>().First().Identifier.IdentifierType
                .Should().Be(AstIdentifierType.Variable);
        }
    }
}
