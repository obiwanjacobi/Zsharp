using System.Linq;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.UnitTests.Compiler.IR;

public class ExpressionTests
{
    [Theory]
    [InlineData("42", 42, "I8")]
    [InlineData("42", 42, "U8")]
    [InlineData("255", 255, "U8")]
    [InlineData("256", 256, "I16")]
    [InlineData("256", 256, "U16")]
    [InlineData("65535", 65535, "U16")]
    [InlineData("65535", 65535, "I32")]
    [InlineData("4294967295", 4294967295, "I64")]
    [InlineData("42.42", 42.42, "F16")]
    [InlineData("42.42", 42.42, "F32")]
    [InlineData("42.42", 42.42, "F64")]
    [InlineData("42.42", 42.42, "F96")]
    public void ParseNumber(string text, object value, string type)
    {
        var types = IrNumber.ParseNumber(text, out var actualValue);
        actualValue.Should().Be(value);
        types.Where(t => t.Name.Value == type).Should().HaveCount(1);
    }

    [Fact]
    public void ArithmeticLiterals()
    {
        const string code =
            "x := 12 + 30" + Tokens.Eol
            ;

        var expectedTypeSymbol = TypeSymbol.I64;

        var program = Ir.Build(code);
        program.Module.Should().NotBeNull();
        program.Module.Declarations.Should().HaveCount(1);

        var v = program.Module.Declarations[0].As<IrDeclarationVariable>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Symbol.Name.Namespace.OriginalName.Should().Be(IrBuilder.DefaultModuleName);
        v.TypeSymbol.Should().Be(expectedTypeSymbol);

        v.Initializer!.TypeSymbol.Should().Be(expectedTypeSymbol);
        v.Initializer!.ConstantValue.Should().NotBeNull();
        v.Initializer!.ConstantValue!.Value.Should().Be(42);

        var sub = v.Initializer.As<IrExpressionBinary>();
        sub.TypeSymbol.Should().Be(expectedTypeSymbol);
        sub.Left.TypeSymbol.Should().Be(expectedTypeSymbol);
        sub.Right.TypeSymbol.Should().Be(expectedTypeSymbol);
    }

    [Fact]
    public void ArithmeticLiterals2()
    {
        const string code =
            "x := (12 + 30) * 2" + Tokens.Eol
            ;

        var expectedTypeSymbol = TypeSymbol.I64;

        var program = Ir.Build(code);
        program.Module.Should().NotBeNull();
        program.Module.Declarations.Should().HaveCount(1);

        var v = program.Module.Declarations[0].As<IrDeclarationVariable>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Symbol.Name.Namespace.OriginalName.Should().Be(IrBuilder.DefaultModuleName);
        v.TypeSymbol.Should().Be(expectedTypeSymbol);

        v.Initializer!.TypeSymbol.Should().Be(expectedTypeSymbol);
        v.Initializer!.ConstantValue.Should().NotBeNull();
        v.Initializer!.ConstantValue!.Value.Should().Be(84);

        var sub = v.Initializer.As<IrExpressionBinary>();
        sub.TypeSymbol.Should().Be(expectedTypeSymbol);
        sub.Left.TypeSymbol.Should().Be(expectedTypeSymbol);
        sub.Right.TypeSymbol.Should().Be(expectedTypeSymbol);

        var sub2 = sub.Left.As<IrExpressionBinary>();
        sub2.TypeSymbol.Should().Be(expectedTypeSymbol);
        sub2.Left.TypeSymbol.Should().Be(expectedTypeSymbol);
        sub2.Right.TypeSymbol.Should().Be(expectedTypeSymbol);
    }

    [Fact]
    public void ExpressionInvocationTypeBinaryOperator()
    {
        const string code =
            "fn: (p: U8): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "y: U8 = 42" + Tokens.Eol +
            "x := fn(y + 42)" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Should().NotBeNull();
        program.Module.Declarations.Should().HaveCount(3);
        var v = program.Module.Declarations[2].As<IrDeclarationVariable>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Symbol.Name.Namespace.OriginalName.Should().Be(IrBuilder.DefaultModuleName);
        v.TypeSymbol.Should().Be(TypeSymbol.U8);
        v.Initializer!.TypeSymbol.Should().Be(TypeSymbol.U8);

        var invok = v.Initializer!.As<IrExpressionInvocation>();
        var exprBin = invok.Arguments.First().Expression.As<IrExpressionBinary>();
        exprBin.Left.TypeSymbol.Should().Be(TypeSymbol.U8);
        exprBin.Right.TypeSymbol.Should().Be(TypeSymbol.U8);
    }

    [Fact]
    public void ExpressionInvocationNestedTypeBinaryOperator()
    {
        const string code =
            "fn: (p: U8): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "y: U8 = 42" + Tokens.Eol +
            "x := fn(fn(y) + 42)" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Should().NotBeNull();
        program.Module.Declarations.Should().HaveCount(3);
        var v = program.Module.Declarations[2].As<IrDeclarationVariable>();
        v.Symbol.Name.Value.Should().Be("x");
        v.TypeSymbol.Should().Be(TypeSymbol.U8);
        v.Initializer!.TypeSymbol.Should().Be(TypeSymbol.U8);

        var invok = v.Initializer!.As<IrExpressionInvocation>();
        var exprBin = invok.Arguments.First().Expression.As<IrExpressionBinary>();
        exprBin.Left.TypeSymbol.Should().Be(TypeSymbol.U8);
        exprBin.Right.TypeSymbol.Should().Be(TypeSymbol.U8);
    }

    [Fact]
    public void ExpressionMemberAccessField()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol +
            "x := MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1 = 42" + Tokens.Eol +
            Tokens.Indent1 + "fld2 = \"42\"" + Tokens.Eol +
            "y := x.fld1" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Should().NotBeNull();
        program.Module.Declarations.Should().HaveCount(3);
        var v = program.Module.Declarations[2].As<IrDeclarationVariable>();
        v.Initializer!.TypeSymbol.Name.Value.Should().Be("U8");
        v.TypeSymbol.Name.Value.Should().Be("U8");
        var xs = v.Initializer.As<IrExpressionMemberAccess>();
        xs.Expression.As<IrExpressionIdentifier>().Symbol.Name.Value.Should().Be("x");
        xs.Members.Last().Name.Value.Should().Be("fld1");
    }

    [Fact]
    public void ExpressionMemberAccessFunction()
    {
        const string code =
            "MyType" + Tokens.Eol +
            Tokens.Indent1 + "fld1: U8" + Tokens.Eol +
            Tokens.Indent1 + "fld2: Str" + Tokens.Eol +
            "fn: (): MyType" + Tokens.Eol +
            Tokens.Indent1 + "ret MyType" + Tokens.Eol +
            Tokens.Indent2 + "fld1 = 42" + Tokens.Eol +
            Tokens.Indent2 + "fld2 = \"42\"" + Tokens.Eol +
            "y := fn().fld1" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Module.Should().NotBeNull();
        program.Module.Declarations.Should().HaveCount(3);
        var v = program.Module.Declarations[2].As<IrDeclarationVariable>();
        v.Initializer!.TypeSymbol.Name.Value.Should().Be("U8");
        v.TypeSymbol.Name.Value.Should().Be("U8");
        var xs = v.Initializer.As<IrExpressionMemberAccess>();
        xs.Expression.As<IrExpressionInvocation>().Symbol.Name.Value.Should().Be("fn");
        xs.Members.Last().Name.Value.Should().Be("fld1");
    }
}
