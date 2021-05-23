using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Zsharp.EmitIL;

namespace UnitTests.EmitIL
{
    [TestClass]
    public class EmitConversionTestsIL
    {
        [TestMethod]
        public void Conversion()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: U8): U16" + Tokens.NewLine +
                Tokens.Indent1 + "return U16(p)" + Tokens.NewLine
                ;

            var emit = Emit.Create(code, Emit.CreateModuleLoader());

            var moduleClass = emit.Context.Module.Types.Find("test");
            var fn = moduleClass.Methods.First();
            fn.Name.Should().Be("fn");
            fn.Body.Instructions.Should().HaveCount(3);

            emit.SaveAs("Conversion.dll");
        }
    }
}
