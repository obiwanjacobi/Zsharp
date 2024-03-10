using Maja.Compiler.Eval;
using Maja.Compiler.IR;

namespace Maja.UnitTests.Compiler.Eval;

public class TypeTests
{
    [Fact]
    public void TypeInit()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            "x := MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable<EvalTypeInstance>("Defmod.x", out var value).Should().BeTrue();
        value!.GetFieldValue<IrConstant>("fld1").ToI32().Should().Be(42);
    }

    [Fact]
    public void TypeInit_BaseType()
    {
        const string code =
            "BaseType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            "MyType : BaseType" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol +
            "x := MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol +
            Tokens.Indent1 + "fld2 = \"42\"" + Tokens.Eol
            ;

        var result = Eval.Run(code);
        result.TryLookupVariable<EvalTypeInstance>("Defmod.x", out var value).Should().BeTrue();
        value!.GetFieldValue<IrConstant>("fld1").ToI32().Should().Be(42);
        value!.GetFieldValue<IrConstant>("fld2").ToStr().Should().Be("42");
    }
}
