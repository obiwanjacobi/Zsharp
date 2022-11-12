using FluentAssertions;
using Maja.Compiler.Diagnostics;
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
        fn.Symbol.Name.Value.Should().Be("fn");
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
        fn.Scope.TryLookupSymbol("p1", out var p1).Should().BeTrue();
        fn.Scope.TryLookupSymbol("p2", out var p2).Should().BeTrue();
        p1!.Kind.Should().Be(SymbolKind.Variable);
        p2!.Kind.Should().Be(SymbolKind.Variable);
        // symbol
        fn.Symbol.Name.Value.Should().Be("fn");
        fn.Symbol.Parameters.Should().HaveCount(2);
        fn.Symbol.ReturnType.Should().Be(TypeSymbol.Bool);
    }

    [Fact]
    public void FuncDuplicate_Error()
    {
        const string code =
            "fn: (p1: U8, p2: Str): Bool" + Tokens.Eol +
            Tokens.Indent1 + "ret false" + Tokens.Eol +
            "fn: (p1: U8, p2: Str): Bool" + Tokens.Eol +
            Tokens.Indent1 + "ret false" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Function 'default.fn' is already declared.");
    }
}
