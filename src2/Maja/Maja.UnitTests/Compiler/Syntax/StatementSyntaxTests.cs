using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Compiler.Syntax;

public class StatementSyntaxTests
{
    private readonly ITestOutputHelper _output;

    public StatementSyntaxTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void If()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "if true" + Tokens.Eol +
            Tokens.Indent2 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDeclarationSyntax>();
        fn.CodeBlock.Statements.Should().HaveCount(1);
        var statIf = fn.CodeBlock.Statements.First().As<StatementIfSyntax>();
        statIf.Expression.Text.Should().Be("true");
        statIf.CodeBlock.Statements.Should().HaveCount(1);

        //Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void GlobalIf()
    {
        const string code =
            "if true" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Statements.Should().HaveCount(1);
        var statIf = result.Statements.First().As<StatementIfSyntax>();
        statIf.Expression.Text.Should().Be("true");
        statIf.CodeBlock.Statements.Should().HaveCount(1);

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void IfElse()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "if true" + Tokens.Eol +
            Tokens.Indent2 + "ret" + Tokens.Eol +
            Tokens.Indent1 + "else" + Tokens.Eol +
            Tokens.Indent2 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDeclarationSyntax>();
        fn.CodeBlock.Statements.Should().HaveCount(1);
        var statIf = fn.CodeBlock.Statements.First().As<StatementIfSyntax>();
        statIf.Expression.Text.Should().Be("true");
        statIf.CodeBlock.Statements.Should().HaveCount(1);

        statIf.Else!.CodeBlock.Statements.Should().HaveCount(1);

        //Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void IfElseIf()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "if true" + Tokens.Eol +
            Tokens.Indent2 + "ret" + Tokens.Eol +
            Tokens.Indent1 + "elif false" + Tokens.Eol +
            Tokens.Indent2 + "ret" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDeclarationSyntax>();
        fn.CodeBlock.Statements.Should().HaveCount(1);
        var statIf = fn.CodeBlock.Statements.First().As<StatementIfSyntax>();
        statIf.Expression.Text.Should().Be("true");
        statIf.CodeBlock.Statements.Should().HaveCount(1);

        statIf.ElseIf!.Expression.Text.Should().Be("false");
        statIf.ElseIf!.CodeBlock.Statements.Should().HaveCount(1);

        //Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void Loop()
    {
        const string code =
            "loop" + Tokens.Eol +
            Tokens.Indent1 + "x = x + 1" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Statements.Should().HaveCount(1);
        var loop = result.Statements.First().As<StatementLoopSyntax>();

        loop.Expression.Should().BeNull();

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void Loop_Count()
    {
        const string code =
            "loop 100" + Tokens.Eol +
            Tokens.Indent1 + "x = x + 1" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Statements.Should().HaveCount(1);
        var loop = result.Statements.First().As<StatementLoopSyntax>();

        loop.Expression.Should().NotBeNull();
        loop.Expression.As<ExpressionLiteralSyntax>().Should().NotBeNull();

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void Loop_Range()
    {
        const string code =
            "loop [0..100]" + Tokens.Eol +
            Tokens.Indent1 + "x = x + 1" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Statements.Should().HaveCount(1);
        var loop = result.Statements.First().As<StatementLoopSyntax>();

        loop.Expression.Should().NotBeNull();
        loop.Expression.As<ExpressionRangeSyntax>().Should().NotBeNull();

        Syntax.RoundTrip(code, _output);
    }

    [Fact]
    public void Loop_While()
    {
        const string code =
            "loop x < 100" + Tokens.Eol +
            Tokens.Indent1 + "x = x + 1" + Tokens.Eol
            ;

        var result = Syntax.Parse(code);
        result.Statements.Should().HaveCount(1);
        var loop = result.Statements.First().As<StatementLoopSyntax>();

        loop.Expression.Should().NotBeNull();
        loop.Expression.As<ExpressionBinarySyntax>()
            .Operator.OperatorCategory.Should().Be(ExpressionOperatorCategory.Comparison);

        Syntax.RoundTrip(code, _output);
    }
}