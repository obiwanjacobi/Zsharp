﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Smoke
{
    [TestClass]
    public class SmokeFunctionTest
    {
        [TestMethod]
        public void Export()
        {
            const string code =
                "export fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }


        [TestMethod]
        public void Void()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void VoidParam()
        {
            const string code =
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void VoidParam2()
        {
            const string code =
                "fn: (p: U8, s: Str)" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void RetVal()
        {
            const string code =
                "fn: (): Bool" + Tokens.NewLine +
                Tokens.Indent1 + "return true" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void RetValParam()
        {
            const string code =
                "fn: (p: U8): Bool" + Tokens.NewLine +
                Tokens.Indent1 + "return true" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void RetValParam2()
        {
            const string code =
                "fn: (p: U8, s: Str): Bool" + Tokens.NewLine +
                Tokens.Indent1 + "return true" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void Call()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "call()" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }
    }
}
