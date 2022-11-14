using FluentAssertions;
using Maja.Compiler.External;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Xunit;

namespace Maja.UnitTests.IR;

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
        program.Root.Should().NotBeNull();
        program.Root.Imports.Should().HaveCount(1);
        program.Root.Statements.Should().HaveCount(1);
        var stat = program.Root.Statements[0].As<IrStatementExpression>();
        var invoke = stat.Expression.As<IrExpressionInvocation>();
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
            "use System.ConsoleColor" + Tokens.Eol +
            "c: ConsoleColor" + Tokens.Eol
            ;

        var program = Ir.Build(code, builder.ToModuleLoader());
        program.Root.Should().NotBeNull();
        program.Root.Imports.Should().HaveCount(1);
        program.Root.Members.Should().HaveCount(1);
        var v = program.Root.Members[0].As<IrVariableDeclaration>();
        v.Type.IsExternal.Should().BeTrue();
        var colors = v.Type.As<DeclaredTypeSymbol>();
        colors.Enums.Should().HaveCount(16);
    }
}