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
        fn.Identifier.Name.Should().Be("fn");
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
        fn.Identifier.Name.Should().Be("fn");
        fn.Parameters.Should().HaveCount(1);
        var param = fn.Parameters.First().As<ParameterSyntax>();
        param.Name.Name.Should().Be("p");
        param.Type.Name.Name.Should().Be("U8");
    }
}