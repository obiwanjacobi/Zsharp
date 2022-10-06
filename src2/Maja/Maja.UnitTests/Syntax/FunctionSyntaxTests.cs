using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class FunctionSyntaxTests
{
    [Fact]
    public void Fn()
    {
        const string code =
            "fn: ()" + Tokens.EOL +
            Tokens.INDENT1 + "ret" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDelcarationSyntax>();
        fn.Identifier.Value.Should().Be("fn");
        fn.CodeBlock.Statements.Should().HaveCount(1);
        var ret = fn.CodeBlock.Statements.First().As<StatementReturnSyntax>();
        ret.Should().NotBeNull();
    }

    [Fact]
    public void FnParams()
    {
        const string code =
            "fn: (p: U8)" + Tokens.EOL +
            Tokens.INDENT1 + "ret" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDelcarationSyntax>();
        fn.Identifier.Value.Should().Be("fn");
        fn.Parameters.Should().HaveCount(1);
        var param = fn.Parameters.First().As<ParameterSyntax>();
        param.Name.Value.Should().Be("p");
        param.Type.Name.Value.Should().Be("U8");
    }

    [Fact]
    public void FnRetVal()
    {
        const string code =
            "fn: (): U8" + Tokens.EOL +
            Tokens.INDENT1 + "ret 42" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDelcarationSyntax>();
        fn.Identifier.Value.Should().Be("fn");
        fn.ReturnType!.Name.Value.Should().Be("U8");
        var ret = fn.CodeBlock.Statements.First().As<StatementReturnSyntax>();
        ret.Expression.Should().NotBeNull();
        ret.Expression.As<ExpressionLiteralSyntax>().Value.Should().Be("42");
    }
}