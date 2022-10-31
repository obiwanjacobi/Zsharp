using FluentAssertions;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Xunit;

namespace Maja.UnitTests.IR;

public class StatementTests
{
    [Fact]
    public void If()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "if x = 0" + Tokens.Eol +
            Tokens.Indent1 + "x = 42" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var statIf = program.Root.Statements[0].As<IrStatementIf>();
        statIf.Condition.TypeSymbol.Should().Be(TypeSymbol.Bool);
        statIf.CodeBlock.Statements.Should().HaveCount(1);
    }

    [Fact]
    public void IfElse()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "if x = 0" + Tokens.Eol +
            Tokens.Indent1 + "x = 42" + Tokens.Eol +
            "else" + Tokens.Eol +
            Tokens.Indent1 + "x = 101" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var statIf = program.Root.Statements[0].As<IrStatementIf>();
        statIf.Condition.TypeSymbol.Should().Be(TypeSymbol.Bool);
        statIf.CodeBlock.Statements.Should().HaveCount(1);
        statIf.ElseClause!.CodeBlock.Statements.Should().HaveCount(1);
    }

    [Fact]
    public void IfElseIf()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "if x = 0" + Tokens.Eol +
            Tokens.Indent1 + "x = 42" + Tokens.Eol +
            "else if x > 101" + Tokens.Eol +
            Tokens.Indent1 + "x = 0" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var statIf = program.Root.Statements[0].As<IrStatementIf>();
        statIf.Condition.TypeSymbol.Should().Be(TypeSymbol.Bool);
        statIf.CodeBlock.Statements.Should().HaveCount(1);
        statIf.ElseIfClause!.Condition.TypeSymbol.Should().Be(TypeSymbol.Bool);
        statIf.ElseIfClause!.CodeBlock.Statements.Should().HaveCount(1);
    }

    [Fact]
    public void IfElseIfElse()
    {
        const string code =
            "x: U8 = 0" + Tokens.Eol +
            "if x = 0" + Tokens.Eol +
            Tokens.Indent1 + "x = 42" + Tokens.Eol +
            "else if x > 101" + Tokens.Eol +
            Tokens.Indent1 + "x = 0" + Tokens.Eol +
            "else" + Tokens.Eol +
            Tokens.Indent1 + "x = 101" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var statIf = program.Root.Statements[0].As<IrStatementIf>();
        statIf.Condition.TypeSymbol.Should().Be(TypeSymbol.Bool);
        statIf.CodeBlock.Statements.Should().HaveCount(1);
        var statElseIf = statIf.ElseIfClause!;
        statElseIf.ElseClause!.CodeBlock.Statements.Should().HaveCount(1);
    }
}
