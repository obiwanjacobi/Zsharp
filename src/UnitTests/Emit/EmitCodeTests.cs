using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private static void InvokeStatic(string assemblyName, string typeName, string methodName)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var path = Path.GetDirectoryName(thisAssembly.Location);

            var assembly = Assembly.LoadFile(Path.Combine(path, assemblyName + ".dll"));
            var type = assembly.ExportedTypes.Single(t => t.Name == typeName);
            var method = type.GetMethods().Single(m => m.Name == methodName);
            method.Invoke(null, null);
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
        public void ExternalFunctionCallParameterAlias_Run()
        {
            const string code =
                "module EmitCodeTests" + Tokens.NewLine +
                "import Print = System.Console.WriteLine" + Tokens.NewLine +
                "export Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "Print(\"Hello Z# World\")" + Tokens.NewLine
                ;

            var moduleLoader = CreateModuleLoader();
            var emit = CreateEmitCode(code, moduleLoader);
            emit.Should().NotBeNull();

            emit.SaveAs("ExternalFunctionCallParameterAlias_Run.dll");

            InvokeStatic("ExternalFunctionCallParameterAlias_Run", "EmitCodeTests", "Main");
        }

        [TestMethod]
        public void ExternalFunctionCallParameter_Run()
        {
            const string code =
                "module EmitCodeTests" + Tokens.NewLine +
                "import System.Console" + Tokens.NewLine +
                "export Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "WriteLine(\"Hello Z# World\")" + Tokens.NewLine
                ;

            var moduleLoader = CreateModuleLoader();
            var emit = CreateEmitCode(code, moduleLoader);
            emit.Should().NotBeNull();

            emit.SaveAs("ExternalFunctionCallParameter_Run.dll");

            InvokeStatic("ExternalFunctionCallParameter_Run", "EmitCodeTests", "Main");
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

        [TestMethod]
        public void BranchIfTrue()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "if true" + Tokens.NewLine +
                Tokens.Indent2 + "return" + Tokens.NewLine
                ;

            var emit = CreateEmitCode(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var body = moduleClass.Methods.First().Body;
            body.Instructions.Should().HaveCount(5);

            emit.SaveAs("BranchIfTrue.dll");
        }
    }
}
