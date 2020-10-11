using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Smoke
{
    [TestClass]
    public class SmokeEnumTests
    {
        [TestMethod]
        public void DefaultOptionsOnOneLine()
        {
            const string code = "MyEnum" + Tokens.NewLine
                + Tokens.Indent1 + "opt1, opt2, opt3" + Tokens.NewLine;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void DefaultOptions()
        {
            const string code = "MyEnum" + Tokens.NewLine
                + Tokens.Indent1 + "opt1" + Tokens.NewLine
                + Tokens.Indent1 + "opt2" + Tokens.NewLine
                + Tokens.Indent1 + "opt3" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void ExplicitOptions()
        {
            const string code = "MyEnum" + Tokens.NewLine
                + Tokens.Indent1 + "opt1 = 1" + Tokens.NewLine
                + Tokens.Indent1 + "opt2 = 2" + Tokens.NewLine
                + Tokens.Indent1 + "opt3 = 3" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void StringOptions()
        {
            const string code = "MyEnum: Str" + Tokens.NewLine
                + Tokens.Indent1 + "opt1 = \"1\"" + Tokens.NewLine
                + Tokens.Indent1 + "opt2 = \"2\"" + Tokens.NewLine
                + Tokens.Indent1 + "opt3 = \"3\"" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void OmmittedOptions_err()
        {
            const string code = "MyEnum" + Tokens.NewLine;

            Parser.ParseForError(code)
                .Should().NotBeNullOrEmpty();
        }
    }
}
