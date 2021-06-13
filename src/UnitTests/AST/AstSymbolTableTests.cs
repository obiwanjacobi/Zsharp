﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.AST;

namespace UnitTests.AST
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
            symbols.Entries.Any(e => e is null).Should().BeFalse();

            var v = symbols.FindEntry("v", AstSymbolKind.Variable);
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
            symbols.Entries.Any(e => e is null).Should().BeFalse();

            var v = symbols.FindEntry("v", AstSymbolKind.Variable);
            v.SymbolKind.Should().Be(AstSymbolKind.Variable);
            v.References.Should().HaveCount(1);
        }

        [TestMethod]
        public void ImportModuleName()
        {
            const string code =
                "import System.Console" + Tokens.NewLine
                ;

            var file = Build.File(code, Compile.CreateModuleLoader());
            var symbols = file.Symbols;
            symbols.Entries.Any(e => e is null).Should().BeFalse();

            var mod = symbols.FindEntries(AstSymbolKind.Module).Single();
            mod.SymbolName.Should().Be("System.Console");
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
            symbols.Entries.Any(e => e is null).Should().BeFalse();

            var fn = symbols.FindEntry("fn", AstSymbolKind.Function);
            fn.SymbolKind.Should().Be(AstSymbolKind.Function);
            fn.SymbolLocality.Should().Be(AstSymbolLocality.Exported);

            // the export entry is removed
            fn = symbols.FindEntry("fn", AstSymbolKind.Unknown);
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
            var fnSymbol = symbols.FindEntry("print", AstSymbolKind.Function);
            fnSymbol.SymbolKind.Should().Be(AstSymbolKind.Function);

            // the import entry should be module
            fnSymbol = symbols.FindEntry("external", AstSymbolKind.Module);
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
            symbols.Entries.Any(e => e is null).Should().BeFalse();

            symbols.Entries.Should().HaveCount(2);
            var fn = symbols.FindEntry("fn", AstSymbolKind.Function);
            fn.SymbolKind.Should().Be(AstSymbolKind.Function);

            symbols = fn.DefinitionAs<AstFunctionDefinitionImpl>()!.Symbols;
            symbols.Entries.Should().HaveCount(1);
            symbols.FindEntry("v", AstSymbolKind.Variable).Should().NotBeNull();
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

            symbols.Entries.Should().HaveCount(3);
            var fn = symbols.FindEntry("fn", AstSymbolKind.Function);
            fn.SymbolKind.Should().Be(AstSymbolKind.Function);

            fn = symbols.FindEntry("fn2", AstSymbolKind.Function);
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
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Entries.Should().HaveCount(2);

            var p = symbols.FindEntry("p", AstSymbolKind.Variable);
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
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Entries.Should().HaveCount(2);

            var p = symbols.FindEntry("self", AstSymbolKind.Variable);
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
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;
            symbols.Entries.Should().HaveCount(1);

            symbols.FindEntry("p", AstSymbolKind.Variable).Should().NotBeNull();
        }

        [TestMethod]
        public void LocalVariableName()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "v = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;
            var v = symbols.FindEntry("v", AstSymbolKind.Variable);
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
            var fn = file.CodeBlock.ItemAt<AstFunctionDefinitionImpl>(0);
            var symbols = fn.CodeBlock.Symbols;

            var v = symbols.FindEntry("v", AstSymbolKind.Variable);
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
            var entry = symbols.FindEntry("Myenum.Zero", AstSymbolKind.Field);
            entry.SymbolKind.Should().Be(AstSymbolKind.Field);
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
            var entry = symbols.FindEntry("Mystruct.Fld1", AstSymbolKind.Field);
            entry.SymbolKind.Should().Be(AstSymbolKind.Field);
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
            var entry = symbols.FindEntry("Myenum.Zero", AstSymbolKind.Field);
            entry.SymbolKind.Should().Be(AstSymbolKind.Field);
            entry.Definition.Should().NotBeNull();
            // reference not resolved yet
            var assign = file.CodeBlock.ItemAt<AstAssignment>(1);
            assign.Expression.RHS.FieldReference.Symbol.References.Should().HaveCount(1);
        }
    }
}
