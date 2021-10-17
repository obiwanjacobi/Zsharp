using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.EmitCS
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

            var emitCode = Emit.Run(code, "TypeEnum");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("private enum Myenum")
                .And.Contain("None = 0");
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

            var emitCode = Emit.Run(code, "TypeEnumExport");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("public enum Count")
                .And.Contain("Zero")
                .And.Contain("One")
                .And.Contain("Two")
                .And.Contain("Three");
        }

        [TestMethod]
        public void TypeEnumBaseType()
        {
            const string code =
                "module TypeEnumBaseType" + Tokens.NewLine +
                "MyEnum: U8" + Tokens.NewLine +
                Tokens.Indent1 + "None = 0" + Tokens.NewLine
                ;

            var emitCode = Emit.Run(code, "TypeEnumBaseType");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("private enum Myenum : System.Byte");
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

            var emitCode = Emit.Run(code, "TypeStructExport");
            var csCode = emitCode.ToString();
            csCode.Should().Contain("public partial record Mystruct")
                .And.Contain("System.UInt32 Id")
                .And.Contain("System.String Name");
        }
    }
}
