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
            "x: U8" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationTypedSyntax>();
        v.Name.Text.Should().Be("x");
        v.Type!.Name.Text.Should().Be("U8");
    }

    [Fact]
    public void VarTypeArgs()
    {
        const string code =
            "x: Bit<U8, 4>" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationTypedSyntax>();
        v.Name.Text.Should().Be("x");
        v.Type!.Name.Text.Should().Be("Bit");
        v.Type!.Arguments.Should().HaveCount(2);
        var arg = v.Type!.Arguments.First().As<TypeArgumentSyntax>();
        arg.Type.Should().NotBeNull();
        arg.Type!.Name.Text.Should().Be("U8");
        arg = v.Type!.Arguments.Skip(1).First().As<TypeArgumentSyntax>();
        arg.Expression.Should().NotBeNull();
        arg.Expression!.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("4");
    }

    [Fact]
    public void VarAssignmentInfer()
    {
        const string code =
            "x := 42" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var v = result.Members.First().As<VariableDeclarationInferredSyntax>();
        v.Name.Text.Should().Be("x");
        v.Expression.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("42");
    }
}