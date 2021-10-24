using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.UnitTests.AST
{
    [TestClass]
    public partial class AstSymbolTableTests
    {
        [TestMethod]
        public void TopVariableDefInit()
        {
            const string code =
                "v: U8 = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var symbols = file.Symbols;
            symbols.Symbols.Any(e => e is null).Should().BeFalse();

            var v = symbols.FindSymbol("v", AstSymbolKind.Variable);
            v.SymbolKind.Should().Be(AstSymbolKind.Variable);
            var def = v.DefinitionAs<AstVariableDefinition>();
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
            symbols.Symbols.Any(e => e is null).Should().BeFalse();

            var v = symbols.FindSymbol("v", AstSymbolKind.Variable);
            v.SymbolKind.Should().Be(AstSymbolKind.Variable);
            v.References.Should().HaveCount(1);
        }

        [TestMethod]
        public void ImportRuntimeGenericFunction()
        {
            const string code =
                "" + Tokens.NewLine
                ;

            var file = Build.File(code, Compile.CreateModuleLoader());
            var symbols = file.Symbols;

            var fn = symbols.FindDefinition<AstFunctionDefinitionExternal>("Array`1", AstSymbolKind.Function);
            fn.Should().NotBeNull();
        }

        [TestMethod]
        public void ImportRuntimeGenericType()
        {
            const string code =
                "" + Tokens.NewLine
                ;

            var file = Build.File(code, Compile.CreateModuleLoader());
            var symbols = file.Symbols;

            var type = symbols.FindDefinition<AstTypeDefinitionExternal>("Opt`1", AstSymbolKind.Type);
            type.Should().NotBeNull();
        }

        [TestMethod]
        public void ExportFunctionName()
        {
            const string code =
                "export fn" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var symbols = file.Symbols;
            symbols.Symbols.Any(e => e is null).Should().BeFalse();

            var fn = symbols.FindSymbol("fn", AstSymbolKind.Function);
            fn.SymbolKind.Should().Be(AstSymbolKind.Function);
            fn.SymbolLocality.Should().Be(AstSymbolLocality.Exported);

            // the export symbol is removed
            fn = symbols.FindSymbol("fn", AstSymbolKind.Unknown);
            fn.SymbolKind.Should().NotBe(AstSymbolKind.Unknown);
        }

        [TestMethod]
        public void ImportFunctionName()
        {
            const string code =
                "import external" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "print()" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.Functions.First();

            var symbols = fn.Symbols;
            var fnSymbol = symbols.FindSymbol("print", AstSymbolKind.Function);
            fnSymbol.SymbolKind.Should().Be(AstSymbolKind.Function);

            // the import symbol should be module
            fnSymbol = symbols.FindSymbol("external", AstSymbolKind.Module);
            fnSymbol.Should().NotBeNull();
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
            symbols.Symbols.Any(e => e is null).Should().BeFalse();

            symbols.Symbols.Should().HaveCount(2);
            var fn = symbols.FindSymbol("fn", AstSymbolKind.Function);
            fn.SymbolKind.Should().Be(AstSymbolKind.Function);

            symbols = fn.DefinitionAs<AstFunctionDefinitionImpl>()!.Symbols;
            symbols.Symbols.Should().HaveCount(1);
            symbols.FindSymbol("v", AstSymbolKind.Variable).Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionMultipleTop()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn2()" + Tokens.NewLine +
                "fn2: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var symbols = file.Symbols;

            symbols.Symbols.Should().HaveCount(3);
            var fn = symbols.FindSymbol("fn", AstSymbolKind.Function);
            fn.SymbolKind.Should().Be(AstSymbolKind.Function);

            fn = symbols.FindSymbol("fn2", AstSymbolKind.Function);
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
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Symbols.Should().HaveCount(2);

            var p = symbols.FindSymbol("p", AstSymbolKind.Variable);
            p.SymbolKind.Should().Be(AstSymbolKind.Variable);
            p.Definition.Should().NotBeNull();
        }

        [TestMethod]
        public void FunctionParameterSelf()
        {
            const string code =
                "fn: (self: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Symbols.Should().HaveCount(2);

            var p = symbols.FindSymbol("self", AstSymbolKind.Variable);
            p.DefinitionAs<AstFunctionParameter>().Identifier.Name.Should().Be("self");
        }

        [TestMethod]
        public void FunctionParameterAssignment()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "p = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Symbols.Should().HaveCount(1);

            symbols.FindSymbol("p", AstSymbolKind.Variable).Should().NotBeNull();
        }

        [TestMethod]
        public void LocalVariableName()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;
            var v = symbols.FindSymbol("v", AstSymbolKind.Variable);
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
            var fn = file.CodeBlock.LineAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;

            var v = symbols.FindSymbol("v", AstSymbolKind.Variable);
            v.SymbolKind.Should().Be(AstSymbolKind.Variable);
            v.DefinitionAs<AstVariableDefinition>().Identifier.IdentifierKind
                .Should().Be(AstIdentifierKind.Variable);
            v.ReferencesAs<AstVariableReference>().First().Identifier.IdentifierKind
                .Should().Be(AstIdentifierKind.Variable);
        }


        [TestMethod]
        public void EnumDefOptionDotName()
        {
            const string code =
                "MyEnum" + Tokens.NewLine +
                Tokens.Indent1 + "Zero" + Tokens.NewLine +
                Tokens.Indent1 + "One" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var symbols = file.Symbols;
            var symbol = symbols.FindSymbol("Myenum.Zero", AstSymbolKind.Field);
            symbol.SymbolKind.Should().Be(AstSymbolKind.Field);
        }

        [TestMethod]
        public void StructDefFieldDotName()
        {
            const string code =
                "MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Fld1: U8" + Tokens.NewLine +
                Tokens.Indent1 + "Fld2: Str" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var symbols = file.Symbols;
            var symbol = symbols.FindSymbol("Mystruct.Fld1", AstSymbolKind.Field);
            symbol.SymbolKind.Should().Be(AstSymbolKind.Field);
        }

        [TestMethod]
        public void EnumUseOption()
        {
            const string code =
                "MyEnum" + Tokens.NewLine +
                Tokens.Indent1 + "Zero" + Tokens.NewLine +
                Tokens.Indent1 + "One" + Tokens.NewLine +
                "v = MyEnum.Zero" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var symbols = file.Symbols;
            // gives symbol of enum option definition (dot-name)
            var symbol = symbols.FindSymbol("Myenum.Zero", AstSymbolKind.Field);
            symbol.SymbolKind.Should().Be(AstSymbolKind.Field);
            symbol.Definition.Should().NotBeNull();
            // reference not resolved yet
            var assign = file.CodeBlock.LineAt<AstAssignment>(1);
            assign.Expression.RHS.FieldReference.Symbol.References.Should().HaveCount(1);
        }
    }
}
