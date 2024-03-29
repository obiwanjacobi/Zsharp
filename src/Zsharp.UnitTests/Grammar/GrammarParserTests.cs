using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Zsharp.UnitTests.Grammar
{
    [TestClass]
    public class GrammarParserTests
    {
        [TestMethod]
        public void File_Empty()
        {
            const string code = "";

            var file = Parser.ParseFile(code);

            file.Should().NotBeNull();
        }


        [TestMethod]
        public void File_Module()
        {
            const string code = "module mymodule"
                 + Tokens.NewLine
                ;

            var file = Parser.ParseFile(code);

            file.header().First()
                .module_statement()
                .statement_module().Should().NotBeNull();
        }

        [TestMethod]
        public void File_Function_Empty()
        {
            const string code = "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Parser.ParseFile(code);

            file.source().First()
                .definition_top()
                .function_def().Should().NotBeNull();
        }

    }
}
