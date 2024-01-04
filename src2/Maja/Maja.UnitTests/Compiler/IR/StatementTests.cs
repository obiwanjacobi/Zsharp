using Maja.Compiler.Diagnostics;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.UnitTests.Compiler.IR;

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
        program.Root.Declarations.Should().HaveCount(1);
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
        program.Root.Declarations.Should().HaveCount(1);
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
        program.Root.Declarations.Should().HaveCount(1);
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
        program.Root.Declarations.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var statIf = program.Root.Statements[0].As<IrStatementIf>();
        statIf.Condition.TypeSymbol.Should().Be(TypeSymbol.Bool);
        statIf.CodeBlock.Statements.Should().HaveCount(1);
        var statElseIf = statIf.ElseIfClause!;
        statElseIf.ElseClause!.CodeBlock.Statements.Should().HaveCount(1);
    }

    [Fact]
    public void AssignmentDiscardInvocation()
    {
        const string code =
            "fn: (): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret 42" + Tokens.Eol +
            "_ = fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var stat = program.Root.Statements[0].As<IrStatementAssignment>();
        stat.Symbol.Should().BeOfType<DiscardSymbol>();
        stat.Expression.As<IrExpressionInvocation>().Symbol.Name.Value.Should().Be("fn");
    }

    [Fact]
    public void AssignmentDiscard_Error()
    {
        const string code =
            "_ = 42" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Invalid Assignment. Only the result of an invocation can be assigned to the discard '_'.");
    }
}
