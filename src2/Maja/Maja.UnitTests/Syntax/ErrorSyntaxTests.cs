using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class ErrorSyntaxTests
{
    [Fact]
    public void SyntaxError()
    {
        const string code =
            "fn: (): " + Tokens.Eol
            ;

        var result = Syntax.Parse(code, throwOnError: false);
        var fn = result.Members.First().As<FunctionDelcarationSyntax>();
        fn.ReturnType!.Name.HasTrailingTokens.Should().BeTrue();
        var err = fn.ReturnType!.Name.TrailingTokens.Single().As<ErrorToken>();
        err.Text.Should().Contain("missing Identifier");
    }
}