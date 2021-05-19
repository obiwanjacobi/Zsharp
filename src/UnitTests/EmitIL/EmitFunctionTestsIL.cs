using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.EmitIL;

namespace UnitTests.EmitIL
{
    [TestClass]
    public class EmitFunctionTestsIL
    {
        [TestMethod]
        public void Function()
        {
            const string code =
                "module Function" + Tokens.NewLine +
                "fn: ()" + Tokens.NewLine +
                Tokens.Indent1 + "return" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("Function");
            var fn = moduleClass.Methods.First();
            fn.Name.Should().Be("fn");
            fn.Body.Instructions.Should().HaveCount(1);

            emit.SaveAs("Function.dll");
        }

        [TestMethod]
        public void FunctionCallParameter()
        {
            const string code =
                "module FunctionCallParameter" + Tokens.NewLine +
                "fn: (p: U8)" + Tokens.NewLine +
                Tokens.Indent1 + "fn(p + 1)" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("Functioncallparameter");
            var fn = moduleClass.Methods.First();
            fn.Name.Should().Be("fn");
            fn.Body.Instructions.Should().HaveCount(5);

            emit.SaveAs("FunctionCallParameter.dll");
        }

        [TestMethod]
        public void ExternalFunctionCallParameterAlias_Run()
        {
            const string code =
                "module ExternalFunctionCallParameterAlias_Run" + Tokens.NewLine +
                "import Print = System.Console.WriteLine" + Tokens.NewLine +
                "export Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "Print(\"Hello Z# World\")" + Tokens.NewLine
                ;

            var moduleLoader = Emit.CreateModuleLoader();
            var emit = Emit.Create(code, moduleLoader);
            emit.Should().NotBeNull();

            emit.SaveAs("ExternalFunctionCallParameterAlias_Run.dll");

            Emit.InvokeStatic("ExternalFunctionCallParameterAlias_Run");
        }

        [TestMethod]
        public void ExternalFunctionCallParameter_Run()
        {
            const string code =
                "module ExternalFunctionCallParameter_Run" + Tokens.NewLine +
                "import System.Console" + Tokens.NewLine +
                "export Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "WriteLine(\"Hello Z# World\")" + Tokens.NewLine
                ;

            var moduleLoader = Emit.CreateModuleLoader();
            var emit = Emit.Create(code, moduleLoader);
            emit.Should().NotBeNull();

            emit.SaveAs("ExternalFunctionCallParameter_Run.dll");

            Emit.InvokeStatic("ExternalFunctionCallParameter_Run");
        }

        [TestMethod]
        public void FunctionCallParameterReturn()
        {
            const string code =
                "module FunctionCallParameterReturn" + Tokens.NewLine +
                "fn: (p: U8): U8" + Tokens.NewLine +
                Tokens.Indent1 + "return p + 1" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("Functioncallparameterreturn");
            var fn = moduleClass.Methods.First();
            fn.Name.Should().Be("fn");
            fn.Body.Instructions.Should().HaveCount(4);

            emit.SaveAs("FunctionCallParameterReturn.dll");
        }

        [TestMethod]
        public void FunctionCallResult_Run()
        {
            const string code =
                "module FunctionCallResult_Run" + Tokens.NewLine +
                "import System.Console" + Tokens.NewLine +
                "fn: (): Str" + Tokens.NewLine +
                Tokens.Indent1 + "return \"Hello Z# World\"" + Tokens.NewLine +
                "export Main: ()" + Tokens.NewLine +
                Tokens.Indent1 + "WriteLine(fn())" + Tokens.NewLine
                ;

            var emit = Emit.Create(code, Emit.CreateModuleLoader());
            emit.SaveAs("FunctionCallResult_Run.dll");

            Emit.InvokeStatic("FunctionCallResult_Run");
        }
    }
}
