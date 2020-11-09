﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.Emit;

namespace UnitTests.Emit
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

            var emit = Emit.Create(code);

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

            var emit = Emit.Create(code);

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

            var moduleLoader = Emit.CreateModuleLoader();
            var emit = Emit.Create(code, moduleLoader);
            emit.Should().NotBeNull();

            emit.SaveAs("ExternalFunctionCallParameterAlias_Run.dll");

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
            var emit = Emit.Create(code, moduleLoader);
            emit.Should().NotBeNull();

            emit.SaveAs("ExternalFunctionCallParameter_Run.dll");

            Emit.InvokeStatic("ExternalFunctionCallParameter_Run", "EmitCodeTests", "Main");
        }
    }
}