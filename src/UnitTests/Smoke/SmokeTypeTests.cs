using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Smoke
{
    [TestClass]
    public class SmokeTypeTests
    {
        [TestMethod]
        public void Bit()
        {
            const string code =
                "b: Bit<4>" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void BitInit()
        {
            const string code =
                "b: Bit<4> = 12" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void BitBig()
        {
            const string code =
                "b: Bit<1024>" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void Ptr()
        {
            const string code =
                "p: Ptr<U8>" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void TypeDef()
        {
            const string code =
                "MyType: Map<Str, U8> _" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void TypeAlias()
        {
            const string code =
                "MyType = Map<Str, U8>" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }
    }
}
