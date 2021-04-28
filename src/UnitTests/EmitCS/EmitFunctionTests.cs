﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Emit = UnitTests.EmitCS.Emit;

namespace UnitTests.EmitCs
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

            Emit.Run(code, "Function");
        }

        [TestMethod]
        public void FunctionCallParameter()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: I32)" + Tokens.NewLine +
                Tokens.Indent1 + "fn(p + 1)" + Tokens.NewLine
                ;

            Emit.Run(code, "FunctionCallParameter");
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
            Emit.Run(code, "ExternalFunctionCallParameterAlias_Run", moduleLoader);

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
            Emit.Run(code, "ExternalFunctionCallParameter_Run", moduleLoader);

            Emit.InvokeStatic("ExternalFunctionCallParameter_Run", "EmitCodeTests", "Main");
        }

        [TestMethod]
        public void FunctionCallParameterReturn()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: I32): I32" + Tokens.NewLine +
                Tokens.Indent1 + "return p + 1" + Tokens.NewLine
                ;

            Emit.Run(code, "FunctionCallParameterReturn");
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
            Emit.Run(code, "FunctionCallResult_Run", moduleLoader);

            Emit.InvokeStatic("FunctionCallResult_Run", "test", "Main");
        }
    }
}
