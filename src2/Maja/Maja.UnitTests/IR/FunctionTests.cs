using FluentAssertions;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Xunit;

namespace Maja.UnitTests.IR;

public class FunctionTests
{
    [Fact]
    public void FuncDeclVoid()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        var fn = program.Root.Members[0].As<IrFunctionDeclaration>();
        fn.Body.Statements.Should().HaveCount(1);
        fn.Body.Declarations.Should().BeEmpty();
        fn.Parameters.Should().BeEmpty();
        fn.ReturnType.Should().BeNull();
        // scope
        fn.Scope.Symbols.Should().BeEmpty();
        // symbol
        fn.Symbol.Name.Should().Be("fn");
        fn.Symbol.Parameters.Should().BeEmpty();
        fn.Symbol.ReturnType.Should().Be(TypeSymbol.Void);
    }

    [Fact]
    public void FuncDecl()
    {
        const string code =
            "fn: (p1: U8, p2: Str): Bool" + Tokens.Eol +
            Tokens.Indent1 + "ret false" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Members.Should().HaveCount(1);
        var fn = program.Root.Members[0].As<IrFunctionDeclaration>();
        fn.Body.Statements.Should().HaveCount(1);
        fn.Body.Declarations.Should().BeEmpty();
        fn.Parameters.Should().HaveCount(2);
        fn.ReturnType!.Symbol.Should().Be(TypeSymbol.Bool);
        // scope
        fn.Scope.Symbols.Should().HaveCount(2);
        // symbol
        fn.Symbol.Name.Should().Be("fn");
        fn.Symbol.Parameters.Should().HaveCount(2);
        fn.Symbol.ReturnType.Should().Be(TypeSymbol.Bool);
    }
}
