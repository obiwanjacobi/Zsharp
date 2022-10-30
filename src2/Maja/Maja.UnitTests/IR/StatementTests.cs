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
    }
}
