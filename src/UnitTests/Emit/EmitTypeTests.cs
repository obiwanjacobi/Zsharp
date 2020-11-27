using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zsharp.Emit;

namespace UnitTests.Emit
{
    [TestClass]
    public class EmitTypeTests
    {
        [TestMethod]
        public void TypeEnum()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "MyEnum" + Tokens.NewLine +
                Tokens.Indent1 + "None = 0" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var typeEnum = moduleClass.NestedTypes.Find("Myenum");
            typeEnum.Fields.Should().HaveCount(1 + 1);

            emit.SaveAs("TypeEnum.dll");
        }

        [TestMethod]
        public void TypeEnumExport()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "export MyEnum" + Tokens.NewLine +
                Tokens.Indent1 + "None = 0" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var typeEnum = moduleClass.NestedTypes.Find("Myenum");
            typeEnum.Fields.Should().HaveCount(1 + 1);

            emit.SaveAs("TypeEnumExport.dll");
        }

        [TestMethod]
        public void TypeEnumBaseType()
        {
            const string code =
                "module test" + Tokens.NewLine +
                "MyEnum: U8" + Tokens.NewLine +
                Tokens.Indent1 + "None = 0" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("test");
            var typeEnum = moduleClass.NestedTypes.Find("Myenum");
            typeEnum.Fields.Should().HaveCount(1 + 1);

            emit.SaveAs("TypeEnumBaseType.dll");
        }
    }
}
