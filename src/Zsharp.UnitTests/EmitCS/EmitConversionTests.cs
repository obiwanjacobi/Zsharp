using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zsharp.UnitTests.EmitCS
{
    [TestClass]
    public class EmitConversionTests
    {
        [TestMethod]
        public void Conversion()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "fn: (p: U8): U16" + Tokens.NewLine +
                Tokens.Indent1 + "return U16(p)" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "Conversion", Emit.CreateModuleLoader());
            var csCode = emitCode.ToString();
            csCode.Should().Contain("return Zsharp.Runtime.Conversion.U16(p);");
        }
    }
}
