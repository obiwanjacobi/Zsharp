using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace UnitTests.Smoke
{
    [TestClass]
    public class SmokeParserTests
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
            const string code = "module mymodule";

            var file = Parser.ParseFile(code);

            file.source().First()
                .module_statement()
                .statement_module().Should().NotBeNull();
        }

        [TestMethod]
        public void File_Function_Empty()
        {
            const string code = "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return";

            var file = Parser.ParseFile(code);

            file.source().First()
                .definition_top()
                .function_def().Should().NotBeNull();
        }

    }
}
