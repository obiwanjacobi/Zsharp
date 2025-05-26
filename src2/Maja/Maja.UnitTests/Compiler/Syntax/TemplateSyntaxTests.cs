using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Compiler.Syntax;

public class TemplateSyntaxTests
{
    private readonly ITestOutputHelper _output;

    public TemplateSyntaxTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void FnTypeParams_Template()
    {
        const string code =
            "fn: <#T>(p: T)" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<DeclarationFunctionSyntax>();
        fn.Identifier.Text.Should().Be("fn");
        fn.TypeParameters.Should().HaveCount(1);
        var param = fn.TypeParameters.First().As<TypeParameterTemplateSyntax>();
        param.Text.Should().Be("#T");
        param.Type.Text.Should().Be("T");

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void FnTypeParamsWithDefault_Template()
    {
        const string code =
            "fn: <#T = Str>(p: T)" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<DeclarationFunctionSyntax>();
        fn.Identifier.Text.Should().Be("fn");
        fn.TypeParameters.Should().HaveCount(1);
        var param = fn.TypeParameters.First().As<TypeParameterTemplateSyntax>();
        param.Type.Text.Should().Be("T");
        param.DefaultType!.Text.Should().Be("Str");

        Syntax.RoundTrip(code, _output);
    }
}