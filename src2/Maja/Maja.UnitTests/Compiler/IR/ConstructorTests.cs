using System.Linq;
using Maja.Compiler.External;
using Maja.Compiler.IR;

namespace Maja.UnitTests.Compiler.IR;

public class ConstructorTests
{
    [Fact]
    public void ArrayOfT()
    {
        var builder = new AssemblyManagerBuilder();
        builder.AddMaja();

        const string code =
            "use Maja" + Tokens.Eol +
            "arr := Array<U8>(42)" + Tokens.Eol
            ;

        var program = Ir.Build(code, builder.ToModuleLoader());
        var varDecl = program.Module.Declarations.ElementAt(0).As<IrDeclarationVariable>();
        varDecl.Should().NotBeNull();
        varDecl.Symbol.Name.Namespace.OriginalName.Should().Be(IrBuilder.DefaultModuleName);
        var invok = varDecl.Initializer.As<IrExpressionInvocation>();
        invok.Arguments.Count().Should().Be(1);
        invok.TypeArguments.Count().Should().Be(1);
        invok.Symbol.Name.Value.Should().Be("Array");
    }
}
