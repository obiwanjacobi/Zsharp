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

            var context = new CompilerContext(new ModuleLoader());
            var builder = new AstBuilder(context);
            builder.Build(Parser.ParseFile(code), "UnitTests");
            var mod = context.Modules.Modules.First();
            mod.Name.Should().Be("mymod");
        }

        [TestMethod]
        public void BuildFile_Import1()
        {
            const string code =
                "import mymod" + Tokens.NewLine
                ;

            var module = Build.Module(code);

            // TODO
            //var import = module.Imports.First();
            //import.module_name().GetText().Should().Be("mymod");
        }

        [TestMethod]
        public void BuildFile_Import2()
        {
            const string code =
                "import mymod1" + Tokens.NewLine +
                "import mymod2" + Tokens.NewLine
                ;

            var module = Build.Module(code);

            // TODO
            //var import = module.Imports.Skip(1).First();
            //import.module_name().GetText().Should().Be("mymod2");
        }

        [TestMethod]
        public void BuildFile_Export1()
        {
            const string code =
                "export myfn" + Tokens.NewLine
                ;

            var module = Build.Module(code);
            var export = module.Exports.First();
            export.identifier_func().GetText().Should().Be("myfn");
        }

        [TestMethod]
        public void BuildFile_Export2()
        {
            const string code =
                "export myfn1" + Tokens.NewLine +
                "export myfn2" + Tokens.NewLine
                ;

            var module = Build.Module(code);
            var export = module.Exports.Skip(1).First();
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
            var func = file.Functions.First();
            func.Identifier.Name.Should().Be("fn");
            func.CodeBlock.Should().NotBeNull();
        }

        [TestMethod]
        public void BuildFile_FunctionCall()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "fn()" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var func = file.Functions.First().CodeBlock.ItemAt<AstFunctionReference>(0);
            func.Identifier.Name.Should().Be("fn");
        }

        [TestMethod]
        public void BuildFile_Assignment()
        {
            const string code =
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "c = 42" + Tokens.NewLine
                ;

            var file = Build.File(code);
            var func = file.Functions.First();
            func.Identifier.Name.Should().Be("fn");
            func.CodeBlock.ItemAt<AstAssignment>(0)
                .Should().NotBeNull();
        }
    }
}
