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
                "module TypeEnum" + Tokens.NewLine +
                "MyEnum" + Tokens.NewLine +
                Tokens.Indent1 + "None = 0" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("TypeEnum");
            var typeEnum = moduleClass.NestedTypes.Find("Myenum");
            typeEnum.Fields.Should().HaveCount(1 + 1);

            emit.SaveAs("TypeEnum.dll");
        }

        [TestMethod]
        public void TypeEnumExport()
        {
            const string code =
                "module TypeEnumExport" + Tokens.NewLine +
                "export Count" + Tokens.NewLine +
                Tokens.Indent1 + "Zero" + Tokens.NewLine +
                Tokens.Indent1 + "One" + Tokens.NewLine +
                Tokens.Indent1 + "Two" + Tokens.NewLine +
                Tokens.Indent1 + "Three" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("TypeEnumExport");
            var typeEnum = moduleClass.NestedTypes.Find("Count");
            typeEnum.Fields.Should().HaveCount(4 + 1);

            emit.SaveAs("TypeEnumExport.dll");
        }

        [TestMethod]
        public void TypeEnumBaseType()
        {
            const string code =
                "module TypeEnumBaseType" + Tokens.NewLine +
                "MyEnum: U8" + Tokens.NewLine +
                Tokens.Indent1 + "None = 0" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("TypeEnumBaseType");
            var typeEnum = moduleClass.NestedTypes.Find("Myenum");
            typeEnum.Fields.Should().HaveCount(1 + 1);

            emit.SaveAs("TypeEnumBaseType.dll");
        }

        [TestMethod]
        public void TypeStructExport()
        {
            const string code =
                "module TypeStructExport" + Tokens.NewLine +
                "export MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "Id: U32" + Tokens.NewLine +
                Tokens.Indent1 + "Name: Str" + Tokens.NewLine
                ;

            var emit = Emit.Create(code);

            var moduleClass = emit.Context.Module.Types.Find("TypeStructExport");
            var typeStruct = moduleClass.NestedTypes.Find("Mystruct");
            typeStruct.Fields.Should().HaveCount(2);

            emit.SaveAs("TypeStructExport.dll");
        }
    }
}
