using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp;
using Zsharp.AST;

namespace UnitTests.AST
{
    [TestClass]
    public class AstBuilderTests
    {
        [TestMethod]
        public void BuildFile_empty()
        {
            const string code = "";

            var file = Build.File(code);
            file.Should().NotBeNull();
            file.NodeType.Should().Be(AstNodeType.File);
        }

        [TestMethod]
        public void BuildFile_Comment()
        {
            const string code =
                "// comment" + Tokens.NewLine
                ;

            var file = Build.File(code);
            file.Should().NotBeNull();
            file.NodeType.Should().Be(AstNodeType.File);
        }

        [TestMethod]
        public void BuildFile_Module()
        {
            const string code =
                "module mymod" + Tokens.NewLine
                ;

            var builder = new AstBuilder(new CompilerContext());
            builder.Build(Parser.ParseFile(code));
            var mod = builder.Modules.FirstOrDefault();
            mod.Should().NotBeNull();
            mod.Name.Should().Be("mymod");
        }

        [TestMethod]
        public void BuildFile_Import1()
        {
            const string code =
                "import mymod" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var import = file.Imports.FirstOrDefault();
            import.Should().NotBeNull();
            import.module_name().GetText().Should().Be("mymod");
        }

        [TestMethod]
        public void BuildFile_Import2()
        {
            const string code =
                "import mymod1" + Tokens.NewLine +
                "import mymod2" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var import = file.Imports.Skip(1).FirstOrDefault();
            import.Should().NotBeNull();
            import.module_name().GetText().Should().Be("mymod2");
        }

        [TestMethod]
        public void BuildFile_Export1()
        {
            const string code =
                "export myfn" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var export = file.Exports.FirstOrDefault();
            export.Should().NotBeNull();
            export.identifier_func().GetText().Should().Be("myfn");
        }

        [TestMethod]
        public void BuildFile_Export2()
        {
            const string code =
                "export myfn1" + Tokens.NewLine +
                "export myfn2" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var export = file.Exports.Skip(1).FirstOrDefault();
            export.Should().NotBeNull();
            export.identifier_func().GetText().Should().Be("myfn2");
        }

        [TestMethod]
        public void BuildFile_Function()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var func = file.Functions.FirstOrDefault();
            func.Should().NotBeNull();
            func.Identifier.Name.Should().Be("fn");
            func.CodeBlock.Should().NotBeNull();
        }

        [TestMethod]
        public void BuildFile_Assignment()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "c = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var func = file.Functions.FirstOrDefault();
            func.Should().NotBeNull();
            func.Identifier.Name.Should().Be("fn");
            func.CodeBlock.ItemAt<AstAssignment>(0)
                .Should().NotBeNull();
        }
    }
}
