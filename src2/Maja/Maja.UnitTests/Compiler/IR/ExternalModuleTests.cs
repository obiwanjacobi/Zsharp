using Maja.Compiler.External;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;

namespace Maja.UnitTests.Compiler.IR;

public class ExternalModuleTests
{
    [Fact]
    public void Console()
    {
        var builder = new AssemblyManagerBuilder();
        builder.AddMsCoreLib();
        builder.AddSystemConsole();

        const string code =
            "use System.Console" + Tokens.Eol +
            "WriteLine(\"Hello World!\")" + Tokens.Eol
            ;

        var program = Ir.Build(code, builder.ToModuleLoader());
        program.Module.Should().NotBeNull();
        program.Module.Imports.Should().HaveCount(1);
        program.Module.Statements.Should().HaveCount(1);
        var stat = program.Module.Statements[0].As<IrStatementExpression>();
        var invoke = stat.Expression.As<IrExpressionInvocation>();
        invoke.Symbol.Name.Namespace.OriginalName.Should().Be("System.Console");
        invoke.Arguments.Should().HaveCount(1);
        invoke.Arguments[0].Expression.TypeSymbol.Should().Be(TypeSymbol.Str);
        invoke.Arguments[0].Expression.ConstantValue!.Value.Should().Be("Hello World!");
        invoke.TypeSymbol.Should().Be(TypeSymbol.Void);
    }

    [Fact]
    public void ConsoleColorEnum()
    {
        var builder = new AssemblyManagerBuilder();
        builder.AddMsCoreLib();
        builder.AddSystemConsole();

        const string code =
            "use System" + Tokens.Eol +
            "c: ConsoleColor" + Tokens.Eol
            ;

        var program = Ir.Build(code, builder.ToModuleLoader());
        program.Module.Should().NotBeNull();
        program.Module.Imports.Should().HaveCount(1);
        program.Module.Declarations.Should().HaveCount(1);
        var v = program.Module.Declarations[0].As<IrDeclarationVariable>();
        v.TypeSymbol.IsExternal.Should().BeTrue();
        var colors = v.TypeSymbol.As<DeclaredTypeSymbol>();
        colors.Enums.Should().HaveCount(16);
    }
}
