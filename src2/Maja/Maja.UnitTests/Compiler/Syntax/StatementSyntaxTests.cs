using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Compiler.Syntax;

public class StatementSyntaxTests
{
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
    }
}