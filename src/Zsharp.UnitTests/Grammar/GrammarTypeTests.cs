using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zsharp.UnitTests.Grammar
{
    [TestClass]
    public class GrammarTypeTests
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

        [TestMethod]
        public void TypeEnum()
        {
            const string code =
                "MyEnum" + Tokens.NewLine +
                Tokens.Indent1 + "None = 0" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void TypeStruct()
        {
            const string code =
                "MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Id: U32" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }
    }
}
