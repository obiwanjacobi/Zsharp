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
        fn.CodeBlock.Children.Should().HaveCount(1);
        var ret = fn.CodeBlock.Children.First().As<StatementReturnSyntax>();
        ret.Should().NotBeNull();
    }
}