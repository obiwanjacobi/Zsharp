using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Compiler.Syntax;

public class FunctionSyntaxTests
{
    [Fact]
    public void Fn()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDeclarationSyntax>();
        fn.Identifier.Text.Should().Be("fn");
        fn.CodeBlock.Statements.Should().HaveCount(1);
        var ret = fn.CodeBlock.Statements.First().As<StatementReturnSyntax>();
        ret.Should().NotBeNull();
    }

    [Fact]
    public void FnParams()
    {
        const string code =
            "fn: (p: U8)" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDeclarationSyntax>();
        fn.Identifier.Text.Should().Be("fn");
        fn.Parameters.Should().HaveCount(1);
        var param = fn.Parameters.First().As<ParameterSyntax>();
        param.Name.Text.Should().Be("p");
        param.Type.Name.Text.Should().Be("U8");
    }

    [Fact]
    public void FnParamsIndent()
    {
        const string code =
            "fn: (" + Tokens.Eol +
            Tokens.Indent1 + "p1: U8" + Tokens.Eol +
            Tokens.Indent1 + "p2: Str" + Tokens.Eol +
            ")" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDeclarationSyntax>();
        fn.Identifier.Text.Should().Be("fn");
        fn.Parameters.Should().HaveCount(2);
        var param = fn.Parameters.First().As<ParameterSyntax>();
        param.Name.Text.Should().Be("p1");
        param.Type.Name.Text.Should().Be("U8");
        param = fn.Parameters.Skip(1).First().As<ParameterSyntax>();
        param.Name.Text.Should().Be("p2");
        param.Type.Name.Text.Should().Be("Str");
    }

    [Fact]
    public void FnRetVal()
    {
        const string code =
            "fn: (): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret 42" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDeclarationSyntax>();
        fn.Identifier.Text.Should().Be("fn");
        fn.ReturnType!.Name.Text.Should().Be("U8");
        var ret = fn.CodeBlock.Statements.First().As<StatementReturnSyntax>();
        ret.Expression.Should().NotBeNull();
        ret.Expression.As<ExpressionLiteralSyntax>().LiteralNumber!.Text.Should().Be("42");
    }
}