using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.UnitTests.Compiler.IR;

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
        program.Root.Declarations.Should().HaveCount(1);
        var fn = program.Root.Declarations[0].As<IrDeclarationFunction>();
        fn.Body.Statements.Should().HaveCount(1);
        fn.Body.Declarations.Should().BeEmpty();
        fn.Parameters.Should().BeEmpty();
        fn.ReturnType.Symbol.Should().Be(TypeSymbol.Void);
        // scope
        fn.Scope.Symbols.Should().BeEmpty();
        // symbol
        fn.Symbol.Name.Value.Should().Be("fn");
        fn.Symbol.Parameters.Should().BeEmpty();
        fn.Symbol.ReturnType.Should().Be(TypeSymbol.Void);
    }

    [Fact]
    public void FuncDeclVoid_Err_RetValue()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret 42" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);

        program.Diagnostics.Should().NotBeEmpty();
        program.Diagnostics.First().Text.Should().Contain("Void function");
    }

    [Fact]
    public void FuncDeclVoid_Err_RetValue_Descendent()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "if true" + Tokens.Eol +
            Tokens.Indent2 + "ret 42" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);

        program.Diagnostics.Should().NotBeEmpty();
        program.Diagnostics.First().Text.Should().Contain("Void function");
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
        program.Root.Declarations.Should().HaveCount(1);
        var fn = program.Root.Declarations[0].As<IrDeclarationFunction>();
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

    // see todo list
    //[Fact] // fails
    public void DeclarationWithForwardReference()
    {
        const string code =
            "fn: (): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret fn2()" + Tokens.Eol +
            "fn2: (): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret 42" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(2);
    }

    [Fact]
    public void FuncDecl_TypeParams_Generics()
    {
        const string code =
            "fn: <T>(p1: T): Bool" + Tokens.Eol +
            Tokens.Indent1 + "ret false" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        var fn = program.Root.Declarations[0].As<IrDeclarationFunction>();
        fn.Body.Statements.Should().HaveCount(1);
        fn.Body.Declarations.Should().BeEmpty();
        fn.TypeParameters.Should().HaveCount(1);
        fn.Parameters.Should().HaveCount(1);
        fn.ReturnType!.Symbol.Should().Be(TypeSymbol.Bool);
        // scope
        fn.Scope.Symbols.Should().HaveCount(2);
        fn.Scope.TryLookupSymbol("p1", out var p1).Should().BeTrue();
        p1!.Kind.Should().Be(SymbolKind.Variable);
        // symbol
        fn.Symbol.Name.Value.Should().Be("fn");
        fn.Symbol.TypeParameters.Should().HaveCount(1);
        fn.Symbol.Parameters.Should().HaveCount(1);
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
        err.Text.Should().Contain("Function 'Defmod.fn' is already declared.");
    }

    [Fact]
    public void Invocation()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol +
            "fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var fn = program.Root.Statements[0]
            .As<IrStatementExpression>().Expression
            .As<IrExpressionInvocation>();
        fn.Symbol!.Name.Value.Should().Be("fn");
        fn.Arguments.Should().HaveCount(0);
    }

    [Fact]
    public void InvocationAssign()
    {
        const string code =
            "fn: (): U8" + Tokens.Eol +
            Tokens.Indent1 + "ret 42" + Tokens.Eol +
            "x := fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(2);
        var v = program.Root.Declarations[1].As<IrDeclarationVariable>();
        v.Symbol.Name.Value.Should().Be("x");
        v.Symbol.Type.Should().Be(TypeSymbol.U8);
        v.Initializer!.TypeSymbol.Should().Be(TypeSymbol.U8);
        v.Initializer!.ConstantValue.Should().BeNull();
        v.TypeSymbol.Should().Be(TypeSymbol.U8);
    }

    [Fact]
    public void InvocationForwardReference()
    {
        const string code =
            "fn()" + Tokens.Eol +
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var fn = program.Root.Statements[0]
            .As<IrStatementExpression>().Expression
            .As<IrExpressionInvocation>();
        fn.Symbol!.Name.Value.Should().Be("fn");
        fn.Arguments.Should().HaveCount(0);
    }

    [Fact]
    public void InvocationTypeParam_Generics()
    {
        const string code =
            "fn: <T>(p: T): T" + Tokens.Eol +
            Tokens.Indent1 + "ret p" + Tokens.Eol +
            "x := fn<U8>(42)" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(2);
        var v = program.Root.Declarations[1].As<IrDeclarationVariable>();
        v.Symbol.Name.Value.Should().Be("x");
        v.TypeSymbol.Name.Value.Should().Be("U8");
        var invok = v.Initializer.As<IrExpressionInvocation>();
        invok.Arguments.Should().HaveCount(1);
        invok.TypeArguments.Should().HaveCount(1);
    }

    [Fact]
    public void InvocationAssign_ErrorCannotAssignVoid()
    {
        const string code =
            "fn: ()" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol +
            "x := fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Cannot assign Void").And.Contain("x");
    }

    [Fact]
    public void Invocation_ErrorNotFound()
    {
        const string code =
            "x := fn()" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Function reference 'fn' cannot be resolved. Function not found.");
    }

    [Fact]
    public void MatchArgumentType()
    {
        const string code =
            "fn: (p: U8)" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol +
            "fn(42)" + Tokens.Eol
            ;

        var program = Ir.Build(code);
        program.Root.Should().NotBeNull();
        program.Root.Declarations.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var fn = program.Root.Statements[0].As<IrStatementExpression>()
            .Expression.As<IrExpressionInvocation>();
        fn.Symbol!.Name.Value.Should().Be("fn");
        fn.Arguments.Should().HaveCount(1);
        var arg = fn.Arguments[0];
        arg.Expression.ConstantValue.Should().NotBeNull();
        arg.Expression.ConstantValue!.Value.Should().Be(42);
    }

    [Fact]
    public void MatchArgumentType_Error()
    {
        const string code =
            "fn: (p: U8)" + Tokens.Eol +
            Tokens.Indent1 + "ret" + Tokens.Eol +
            "fn(\"42\")" + Tokens.Eol
            ;

        var program = Ir.Build(code, allowError: true);
        program.Diagnostics.Should().HaveCount(1);
        var err = program.Diagnostics[0];
        err.MessageKind.Should().Be(DiagnosticMessageKind.Error);
        err.Text.Should().Contain("Cannot implicitly use Type 'Str' as Type 'U8'.");
    }
}
