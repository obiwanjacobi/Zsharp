using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zsharp.UnitTests.EmitCS
{
    [TestClass]
    public class EmitFunctionTests
    {
        [TestMethod]
        public void Function()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "Function");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("private static void fn()")
                .And.Contain("return ;");
        }

        [TestMethod]
        public void FunctionCallParameter()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: I32)" + Tokens.NewLine +
                Tokens.Indent1 + "fn(p + 1)" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "FunctionCallParameter");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("private static void fn(System.Int32 p)")
                .And.Contain("fn(p + 1);");
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

            // TODO: the namespace of the imported System.Console.WriteLine is wrong.

            var moduleLoader = Emit.CreateModuleLoader();
            var emitCode = Emit.Run(code, "ExternalFunctionCallParameterAlias_Run", moduleLoader);
            var csCode = emitCode.ToString();
            csCode.Should().Contain("public static void Main()")
                .And.Contain("System.Console.WriteLine(\"Hello Z# World\");");

            Emit.InvokeStatic("ExternalFunctionCallParameterAlias_Run");
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
            var emitCode = Emit.Run(code, "ExternalFunctionCallParameter_Run", moduleLoader);
            var csCode = emitCode.ToString();
            csCode.Should().Contain("public static void Main()")
                .And.Contain("System.Console.WriteLine(\"Hello Z# World\");");

            Emit.InvokeStatic("ExternalFunctionCallParameter_Run");
        }

        [TestMethod]
        public void FunctionCallParameterReturn()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: I32): I32" + Tokens.NewLine +
                Tokens.Indent1 + "return p + 1" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "FunctionCallParameterReturn");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("private static System.Int32 fn(System.Int32 p)")
                .And.Contain("return p + 1;");
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

            var moduleLoader = Emit.CreateModuleLoader();
            var emitCode = Emit.Run(code, "FunctionCallResult_Run", moduleLoader);
            var csCode = emitCode.ToString();
            csCode.Should().Contain("private static System.String fn()")
                .And.Contain("System.Console.WriteLine(fn());");

            Emit.InvokeStatic("FunctionCallResult_Run");
        }
    }
}
