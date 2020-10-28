﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.AST;
using Zsharp.Semantics;

namespace UnitTests.AST
{
    [TestClass]
    public partial class AstVisitorTests
    {
        const string code =
            "// comment" + Tokens.NewLine +
            "export MyFunction" + Tokens.NewLine +
            "x = 42" + Tokens.NewLine +
            "MyFunction: (p: U8): Bool" + Tokens.NewLine +
            Tokens.Indent1 + "if p = 42" + Tokens.NewLine +
            Tokens.Indent2 + "return true" + Tokens.NewLine +
            Tokens.Indent1 + "return p <> 0" + Tokens.NewLine
            ;

        [TestMethod]
        public void NodesHaveParents()
        {
            var file = Build.File(code);
            file.Should().NotBeNull();
            var checker = new AstParentChecker();
            checker.Visit(file);
        }

        [TestMethod]
        public void NodesHaveVariables()
        {
            var file = Build.File(code);
            file.Should().NotBeNull();
            var checker = new AstVariableChecker();
            checker.Visit(file);
        }

        [TestMethod]
        public void NodesHaveTypes()
        {
            var file = Build.File(code);
            file.Should().NotBeNull();

            var errors = new AstErrorSite();

            var symbolResolver = new ResolveSymbols(errors);
            symbolResolver.Visit(file);

            var typeResolver = new ResolveTypes(errors);
            typeResolver.Visit(file);

            errors.HasErrors.Should().BeFalse();

            var checker = new AstTypeChecker();
            checker.Visit(file);
        }
    }
}
