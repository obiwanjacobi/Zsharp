﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zsharp.UnitTests.Grammar
{
    [TestClass]
    public class GrammarVariableTests
    {
        [TestMethod]
        public void GlobalAutoVariable()
        {
            const string code =
                "g = 42" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void GlobalTypedVariable()
        {
            const string code =
                "g: U8" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void GlobalTypedVariableInit()
        {
            const string code =
                "g: U8 = 42" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void LocalTypedVariable()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "l: U8" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void LocalTypedVariableInit()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "l: U8 = 42" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void GlobalEnumVariableInit()
        {
            const string code =
                "v = MyEnum.None" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }
    }
}
