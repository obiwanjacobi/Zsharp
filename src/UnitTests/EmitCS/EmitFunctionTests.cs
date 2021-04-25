using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Zsharp.AST;
using Zsharp.EmitCS;
using Emit = UnitTests.EmitCS.Emit;

namespace UnitTests.EmitCs
{
    [TestClass]
    public class EmitFunctionTests
    {
        private static EmitCode RunEmit(string code, string testName, IAstModuleLoader moduleLoader = null)
        {
            var emit = Emit.Create(code, moduleLoader);

            emit.SaveAs($@".\{testName}\{testName}.cs");

            Console.WriteLine(emit.ToString());

            var targetPath = Emit.Build(testName);

            File.Exists(targetPath).Should().BeTrue();
            return emit;
        }

        [TestMethod]
        public void Function()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            RunEmit(code, "Function");
        }

        [TestMethod]
        public void FunctionCallParameter()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: I32)" + Tokens.NewLine +
                Tokens.Indent1 + "fn(p + 1)" + Tokens.NewLine
                ;

            RunEmit(code, "FunctionCallParameter");
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

            // TODO: the name space of the imported System.Console.WriteLine is wrong.

            var moduleLoader = Emit.CreateModuleLoader();
            RunEmit(code, "ExternalFunctionCallParameterAlias_Run", moduleLoader);

            Emit.InvokeStatic("ExternalFunctionCallParameterAlias_Run", "EmitCodeTests", "Main");
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

            var moduleLoader = Emit.CreateModuleLoader();
            RunEmit(code, "ExternalFunctionCallParameter_Run", moduleLoader);

            Emit.InvokeStatic("ExternalFunctionCallParameter_Run", "EmitCodeTests", "Main");
        }

        [TestMethod]
        public void FunctionCallParameterReturn()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: U8): U8" + Tokens.NewLine +
                Tokens.Indent1 + "return p + 1" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);


            emit.SaveAs("FunctionCallParameterReturn.dll");
        }

        [TestMethod]
        public void FunctionCallResult_Run()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "import System.Console" + Tokens.NewLine +
                "fn: (): Str" + Tokens.NewLine +
                Tokens.Indent1 + "return \"Hello Z# World\"" + Tokens.NewLine +
                "export Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "WriteLine(fn())" + Tokens.NewLine
                ;

            var emit = Emit.Create(code, Emit.CreateModuleLoader());

            emit.SaveAs("FunctionCallResult_Run.dll");

            Emit.InvokeStatic("FunctionCallResult_Run", "test", "Main");
        }
    }
}
