﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;
using Zsharp.Semantics;

namespace UnitTests.Semantics
{
    [TestClass]
    public class ResolveSymbolsTests
    {
        private static AstFile ParseFile(string code)
        {
            var file = Build.File(code);
            var resolveSymbols = new ResolveSymbols();
            resolveSymbols.Apply(file);
            return file;
        }

        [TestMethod]
        public void TopVariableInferDef()
        {
            const string code =
                "v = 42" + Tokens.NewLine
                ;

            var file = ParseFile(code);

            var a = file.CodeBlock.ItemAt<AstAssignment>(0);
            var v = a.Variable as AstVariableDefinition;
            v.Should().NotBeNull();
            v.Parent.Should().Be(a);

            var sym = file.CodeBlock.Symbols.Find(v);
            sym.Definition.Should().NotBeNull();
        }
    }
}
