using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Smoke
{
    [TestClass]
    public class SmokeCommentTests
    {
        [TestMethod]
        public void StartOfLine()
        {
            const string code = "// comment";

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void AfterInstruction()
        {
            const string code = "v: U8 = 42        // comment";

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void ThreeSlashes()
        {
            const string code = "/// comment";

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void WhitespaceAfter()
        {
            const string code = "// comment    \t    \n";

            Parser.ParseForError(code)
                .Should().BeNull();
        }
    }
}
