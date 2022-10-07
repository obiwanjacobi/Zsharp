using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class ExpressionSyntaxTests
{
    [Fact]
    public void ArithmeticPlusLiterals()
    {
        const string code =
            "x := 42 + 101" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var expr = v.Expression!.As<ExpressionSyntax>();
        expr.Children[0].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("42");
        expr.Children[1].As<ExpressionOperatorSyntax>().Value.Should().Be("+");
        expr.Children[2].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("101");
    }

    [Fact]
    public void ArithmeticMultipleLiterals()
    {
        const string code =
            "x := 42 + 101 / 2 + 2112" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var expr = v.Expression!.As<ExpressionSyntax>();
        expr.Children[0].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("42");
        expr.Children[1].As<ExpressionOperatorSyntax>().Value.Should().Be("+");
        expr.Children[2].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("101");
        expr.Children[3].As<ExpressionOperatorSyntax>().Value.Should().Be("/");
        expr.Children[4].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("2");
        expr.Children[5].As<ExpressionOperatorSyntax>().Value.Should().Be("+");
        expr.Children[6].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("2112");
    }

    [Fact]
    public void ArithmeticPrecedence()
    {
        const string code =
            "x := 42 + (101 / 2) + 2112" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        var expr = v.Expression!.As<ExpressionSyntax>();
        expr.Children[0].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("42");
        expr.Children[1].As<ExpressionOperatorSyntax>().Value.Should().Be("+");
            var prec = expr.Children[2].As<ExpressionSyntax>();
            prec.Precedence.Should().BeTrue();
            prec.Children[0].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("101");
            prec.Children[1].As<ExpressionOperatorSyntax>().Value.Should().Be("/");
            prec.Children[2].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("2");
        expr.Children[3].As<ExpressionOperatorSyntax>().Value.Should().Be("+");
        expr.Children[4].As<ExpressionLiteralSyntax>().LiteralNumber!.Value.Should().Be("2112");
    }
}
