using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class VariableSyntaxTests
{
    [Fact]
    public void VarType()
    {
        const string code =
            "x: U8" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        v.Name.Value.Should().Be("x");
        v.Type!.Name.Value.Should().Be("U8");
    }

    [Fact]
    public void VarAssignmentInfer()
    {
        const string code =
            "x := 42" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationSyntax>();
        v.Name.Value.Should().Be("x");
        v.Expression.As<ExpressionLiteralSyntax>().Value.Should().Be("42");
    }
}