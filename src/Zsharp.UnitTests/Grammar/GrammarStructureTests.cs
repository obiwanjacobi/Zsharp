using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zsharp.UnitTests.Grammar
{
    [TestClass]
    public class GrammarStructureTests
    {
        [TestMethod]
        public void DefaultWithFields()
        {
            const string code =
                "MyStruct" + Tokens.NewLine +
                Tokens.Indent1 + "fld1: U8" + Tokens.NewLine +
                Tokens.Indent1 + "fld2: U16" + Tokens.NewLine +
                Tokens.Indent1 + "fld3: Str" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void DerivedWithFields()
        {
            const string code =
                "MyStruct: BaseStruct" + Tokens.NewLine +
                Tokens.Indent1 + "fld1: U8" + Tokens.NewLine +
                Tokens.Indent1 + "fld2: U16" + Tokens.NewLine +
                Tokens.Indent1 + "fld3: Str" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }

        [TestMethod]
        public void DefaultWithTypeParam1()
        {
            const string code =
                "MyStruct<T>" + Tokens.NewLine +
                Tokens.Indent1 + "fld1: T" + Tokens.NewLine +
                Tokens.Indent1 + "fld2: U16" + Tokens.NewLine +
                Tokens.Indent1 + "fld3: Str" + Tokens.NewLine
                ;

            Parser.ParseForError(code)
                .Should().BeNull();
        }
    }
}
