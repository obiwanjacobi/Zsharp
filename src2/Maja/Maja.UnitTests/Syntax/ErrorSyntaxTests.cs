using System.Linq;
using FluentAssertions;
using Maja.Compiler.Parser;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class ErrorSyntaxTests
{
    [Fact]
    public void SyntaxError_MissingIdentifier()
    {
        const string code =
            "fn: (): " + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code, throwOnError: false);
        result.HasError.Should().BeTrue();
        var fn = result.Members.First().As<FunctionDeclarationSyntax>();
        fn.ReturnType!.Name.HasTrailingTokens.Should().BeTrue();
        var err = fn.ReturnType!.Name.TrailingTokens.Single().As<ErrorToken>();
        err.Text.Should().Contain("missing Identifier");
        err.TokenTypeId.Should().Be(MajaLexer.Identifier);
    }

    [Fact]
    public void SyntaxError_MissingColon()
    {
        const string code =
            "fn (): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code, throwOnError: false);
        result.HasError.Should().BeTrue();
        // Parser cannot make anything of this: all error tokens
        result.Children.Should().AllSatisfy(c => c.HasError.Should().BeTrue());
    }
}