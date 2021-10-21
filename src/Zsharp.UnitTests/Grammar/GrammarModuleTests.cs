using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zsharp.UnitTests.Grammar
{
    [TestClass]
    public class GrammarModuleTests
    {
        [TestMethod]
        public void Module()
        {
            const string code =
                "module mymod" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void Import()
        {
            const string code =
                "import mymod" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void Export()
        {
            const string code =
                "export myfn" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }
    }
}
