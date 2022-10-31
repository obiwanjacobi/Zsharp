using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

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

        var result = Syntax.Parse(code);
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
}
