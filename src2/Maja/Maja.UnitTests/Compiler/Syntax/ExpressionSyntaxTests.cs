using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Compiler.Syntax;

public class ExpressionSyntaxTests
{
    [Fact]
    public void ArithmeticLiteralsSingle()
    {
        const string code =
            "x := 42 + 101" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var expr = v.Expression!.As<ExpressionBinarySyntax>();
        expr.Left.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("42");
        expr.Operator.Text.Should().Be("+");
        expr.Operator.OperatorKind.Should().Be(ExpressionOperatorKind.Plus);
        expr.Operator.OperatorCategory.Should().Be(ExpressionOperatorCategory.Arithmetic);
        expr.Operator.OperatorCardinality.Should().Be(ExpressionOperatorCardinality.Binary);
        expr.Right.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("101");
    }

    [Fact]
    public void ArithmeticLiteralsMultiple()
    {
        const string code =
            "x := 42 + 101 / 2 + 2112" + Tokens.Eol
            ;

        var result = Syntax.Parse(code, throwOnError: false);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var lvl0 = v.Expression!.As<ExpressionBinarySyntax>();
        var lvl1 = lvl0.Left.As<ExpressionBinarySyntax>();
        var lvl2 = lvl1.Left.As<ExpressionBinarySyntax>();
        lvl2.Left.As<ExpressionLiteralSyntax>().Text.Should().Be("42");
        lvl2.Operator.Text.Should().Be("+");
        lvl2.Right.As<ExpressionLiteralSyntax>().Text.Should().Be("101");
        lvl1.Operator.Text.Should().Be("/");
        lvl1.Right.As<ExpressionLiteralSyntax>().Text.Should().Be("2");
        lvl0.Operator.Text.Should().Be("+");
        lvl0.Right.As<ExpressionLiteralSyntax>().Text.Should().Be("2112");
        lvl0.IsPrecedenceValid.Should().BeFalse();

        //         +
        //        / \
        //      '/'  2112
        //      / \
        //     +   2
        //    / \
        //  42   101
    }

    [Fact]
    public void ArithmeticLiteralsPrecedence()
    {
        const string code =
            "x := 42 + (101 / 2) + 2112" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var lvl0 = v.Expression!.As<ExpressionBinarySyntax>();
        var lvl1 = lvl0.Left.As<ExpressionBinarySyntax>();
        lvl1.Left.As<ExpressionLiteralSyntax>().Text.Should().Be("42");
        lvl1.Operator.Text.Should().Be("+");
        var lvl2 = lvl1.Right.As<ExpressionBinarySyntax>();
        lvl2.Precedence.Should().BeTrue();
        lvl2.Left.As<ExpressionLiteralSyntax>().Text.Should().Be("101");
        lvl2.Operator.Text.Should().Be("/");
        lvl2.Right.As<ExpressionLiteralSyntax>().Text.Should().Be("2");
        lvl0.Operator.Text.Should().Be("+");
        lvl0.Right.As<ExpressionLiteralSyntax>().Text.Should().Be("2112");
        lvl0.IsPrecedenceValid.Should().BeTrue();

        //          +
        //         / \
        //        +   2112
        //       / \
        //     42  '/' *precedence
        //         / \
        //      101   2
    }

    [Fact]
    public void ComparisonSingle()
    {
        const string code =
            "x := y = 42" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var expr = v.Expression!.As<ExpressionBinarySyntax>();
        expr.Left.As<ExpressionIdentifierSyntax>().Name.Text.Should().Be("y");
        expr.Right.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("42");
        expr.Operator.Text.Should().Be("=");
        expr.Operator.OperatorKind.Should().Be(ExpressionOperatorKind.Equals);
        expr.Operator.OperatorCategory.Should().Be(ExpressionOperatorCategory.Comparison);
        expr.Operator.OperatorCardinality.Should().Be(ExpressionOperatorCardinality.Binary);
    }

    [Fact]
    public void Invocation()
    {
        const string code =
            "fn(42)" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Statements.Should().HaveCount(1);
        var s = result.Statements.First().As<StatementExpressionSyntax>();
        var expr = s.Expression!.As<ExpressionInvocationSyntax>();
        expr.Identifier.Text.Should().Be("fn");
        expr.Arguments.First().ChildNodes[0]
            .As<ExpressionLiteralSyntax>().Text.Should().Be("42");
    }

    [Fact]
    public void InvocationAssign()
    {
        const string code =
            "x := fn()" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var expr = v.Expression!.As<ExpressionInvocationSyntax>();
        expr.Identifier.Text.Should().Be("fn");
        expr.Arguments.Should().BeEmpty();
    }

    [Fact]
    public void InvocationAssignParam()
    {
        const string code =
            "x := fn(42)" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var expr = v.Expression!.As<ExpressionInvocationSyntax>();
        expr.Identifier.Text.Should().Be("fn");
        expr.Arguments.First().ChildNodes[0]
            .As<ExpressionLiteralSyntax>().Text.Should().Be("42");
    }

    [Fact]
    public void InvocationAssignTypeParam()
    {
        const string code =
            "x := fn<U8>()" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var expr = v.Expression!.As<ExpressionInvocationSyntax>();
        expr.Identifier.Text.Should().Be("fn");
        expr.TypeArguments.First().ChildNodes[0]
            .As<TypeSyntax>().Text.Should().Be("U8");
    }

    [Fact]
    public void VariableMemberAccess()
    {
        const string code =
            "y := x.fld1" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.ElementAt(0).As<VariableDeclarationSyntax>();
        v.Name.Text.Should().Be("y");
        var expr = v.Expression.As<ExpressionMemberAccessSyntax>();
        expr.Name.Text.Should().Be("fld1");
        expr.LeftAs<ExpressionIdentifierSyntax>()
            .Name.Text.Should().Be("x");
        
    }

    [Fact]
    public void VariableMemberAccess2()
    {
        const string code =
            "y := x.fld1.fld2" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.ElementAt(0).As<VariableDeclarationSyntax>();
        v.Name.Text.Should().Be("y");
        var expr = v.Expression.As<ExpressionMemberAccessSyntax>();
        
        expr.Name.Text.Should().Be("fld2");
        expr = expr.LeftAs<ExpressionMemberAccessSyntax>();
        expr.Name.Text.Should().Be("fld1");
        expr.LeftAs<ExpressionIdentifierSyntax>()
            .Name.Text.Should().Be("x");
    }

    [Fact]
    public void RangeExpression()
    {
        const string code =
            "x := [0..10]" + Tokens.Eol +
            "y := [..10]" + Tokens.Eol +
            "z := [0..]" + Tokens.Eol +
            "a := [..]" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(4);

        var v = result.Members.ElementAt(0).As<VariableDeclarationSyntax>();
        v.Name.Text.Should().Be("x");
        var rng = v.Expression.As<ExpressionRangeSyntax>();
        rng.Start.Should().NotBeNull();
        rng.End.Should().NotBeNull();

        v = result.Members.ElementAt(1).As<VariableDeclarationSyntax>();
        v.Name.Text.Should().Be("y");
        rng = v.Expression.As<ExpressionRangeSyntax>();
        rng.Start.Should().BeNull();
        rng.End.Should().NotBeNull();

        v = result.Members.ElementAt(2).As<VariableDeclarationSyntax>();
        v.Name.Text.Should().Be("z");
        rng = v.Expression.As<ExpressionRangeSyntax>();
        rng.Start.Should().NotBeNull();
        rng.End.Should().BeNull();

        v = result.Members.ElementAt(3).As<VariableDeclarationSyntax>();
        v.Name.Text.Should().Be("a");
        rng = v.Expression.As<ExpressionRangeSyntax>();
        rng.Start.Should().BeNull();
        rng.End.Should().BeNull();
    }
}
