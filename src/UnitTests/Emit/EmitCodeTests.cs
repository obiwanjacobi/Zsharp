using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.Emit;
using Zsharp.Semantics;

namespace UnitTests.Emit
{
    [TestClass]
    public class EmitCodeTests
    {
        private static EmitCode CreateEmitCode(string code)
        {
            var module = Build.Module(code);

            var resolver = new ResolveTypes();
            resolver.Apply(module);

            var emit = new EmitCode("UnitTest");
            emit.Visit(module);
            return emit;
        }

        [TestMethod]
        public void AssemblyModule()
        {
            const string code =
                "module test" + Tokens.NewLine
                ;

            var emit = CreateEmitCode(code);

            emit.Context.Assembly.Should().NotBeNull();
            emit.Context.Module.Should().NotBeNull();
        }

        [TestMethod]
        public void ModuleClass()
        {
            const string code =
                "module test" + Tokens.NewLine
                ;

            var emit = CreateEmitCode(code);

            emit.Context.Module.Types.Find("test").Should().NotBeNull();
        }

        [TestMethod]
        public void Function()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var emit = CreateEmitCode(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var fn = moduleClass.Methods.First();
            fn.Name.Should().Be("fn");
            fn.Body.Instructions.Should().HaveCount(1);
        }

        [TestMethod]
        public void VariableAssignment_Constant()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "a = 42" + Tokens.NewLine
                ;

            var emit = CreateEmitCode(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(1);
        }
    }
}
