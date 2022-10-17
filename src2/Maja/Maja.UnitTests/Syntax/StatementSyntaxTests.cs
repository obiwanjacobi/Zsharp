using System.Linq;
using FluentAssertions;
using Maja.Compiler.Syntax;
using Xunit;

namespace Maja.UnitTests.Syntax;

public class StatementSyntaxTests
{
    [Fact]
    public void If()
    {
        const string code =
            "fn: ()" + Tokens.EOL +
            Tokens.INDENT1 + "if true" + Tokens.EOL +
            Tokens.INDENT2 + "ret" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDelcarationSyntax>();
        fn.CodeBlock.Statements.Should().HaveCount(1);
        var statIf = fn.CodeBlock.Statements.First().As<StatementIfSyntax>();
        statIf.Expression.Text.Should().Be("true");
        statIf.CodeBlock.Statements.Should().HaveCount(1);
    }

    [Fact]
    public void IfElse()
    {
        const string code =
            "fn: ()" + Tokens.EOL +
            Tokens.INDENT1 + "if true" + Tokens.EOL +
            Tokens.INDENT2 + "ret" + Tokens.EOL +
            Tokens.INDENT1 + "else" + Tokens.EOL +
            Tokens.INDENT2 + "ret" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDelcarationSyntax>();
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
            "fn: ()" + Tokens.EOL +
            Tokens.INDENT1 + "if true" + Tokens.EOL +
            Tokens.INDENT2 + "ret" + Tokens.EOL +
            Tokens.INDENT1 + "elif false" + Tokens.EOL +
            Tokens.INDENT2 + "ret" + Tokens.EOL
            ;

        var result = Syntax.Parse(code);
        result.Members.Should().HaveCount(1);
        var fn = result.Members.First().As<FunctionDelcarationSyntax>();
        fn.CodeBlock.Statements.Should().HaveCount(1);
        var statIf = fn.CodeBlock.Statements.First().As<StatementIfSyntax>();
        statIf.Expression.Text.Should().Be("true");
        statIf.CodeBlock.Statements.Should().HaveCount(1);

        statIf.ElseIf!.Expression.Text.Should().Be("false");
        statIf.ElseIf!.CodeBlock.Statements.Should().HaveCount(1);
    }
}