using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp;
using Zsharp.AST;
using Zsharp.Emit;
using Zsharp.Semantics;

namespace UnitTests.Emit
{
    [TestClass]
    public class EmitCodeTests
    {
        private static AssemblyManager LoadTestAssemblies()
        {
            var assemblies = new AssemblyManager();
            assemblies.LoadAssembly(@"C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\3.1.0\ref\netcoreapp3.1\System.Console.dll");
            return assemblies;
        }

        private static ExternalModuleLoader CreateModuleLoader()
        {
            var assemblies = LoadTestAssemblies();
            var loader = new ExternalModuleLoader(assemblies);
            loader.Modules.Should().HaveCount(2);
            return loader;
        }

        private static EmitCode CreateEmitCode(string code, IAstModuleLoader moduleLoader = null)
        {
            var compiler = new Compiler(moduleLoader ?? new ModuleLoader());
            var errors = compiler.Compile("UnitTests", "EmitCodeTests", code);
            errors.Should().BeEmpty();

            var module = compiler.Context.Modules.Modules.First();
            var emit = new EmitCode("EmitCodeTest");
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
        public void FunctionCallParameter()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "fn(p + 1)" + Tokens.NewLine
                ;

            var emit = CreateEmitCode(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var fn = moduleClass.Methods.First();
            fn.Name.Should().Be("fn");
            fn.Body.Instructions.Should().HaveCount(5);
        }

        [TestMethod]
        public void ExternalFunctionCallParameter()
        {
            const string code =
                "module ExternalFunctionCallParameter" + Tokens.NewLine +
                "import Print = System.Console.WriteLineString" + Tokens.NewLine +
                "Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "Print(\"Hello World\")" + Tokens.NewLine
                ;

            var moduleLoader = CreateModuleLoader();
            var emit = CreateEmitCode(code, moduleLoader);
            emit.Should().NotBeNull();

            emit.SaveAs("ExternalFunctionCallParameter.dll");
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
            // ldc 42, stloc 'a', ret
            body.Instructions.Should().HaveCount(3);
        }

        [TestMethod]
        public void VariableAssignment_ExpressionConstants()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "a = 42 + 101" + Tokens.NewLine
                ;

            var emit = CreateEmitCode(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            // ldc 42, ldc 101, add, stloc 'a', ret
            body.Instructions.Should().HaveCount(5);
        }

        [TestMethod]
        public void VariableAssignment_ExpressionVariableRef()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "x: U8" + Tokens.NewLine +
                Tokens.Indent1 + "a = x + 1" + Tokens.NewLine
                ;

            var emit = CreateEmitCode(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(5);
        }
    }
}
