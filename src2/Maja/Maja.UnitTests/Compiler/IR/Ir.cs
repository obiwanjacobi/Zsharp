using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Maja.Compiler.External;
using Maja.Compiler.IR;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.UnitTests.Compiler.IR;

internal static class Ir
{
    public static IrProgram Build(string code, bool allowError = false, [CallerMemberName] string source = "")
        => Build(code, new NullModuleLoader(), allowError, source);

    public static IrProgram Build(string code, IExternalModuleLoader moduleLoader, bool allowError = false, [CallerMemberName] string source = "")
    {
        var tree = SyntaxTree.Parse(code, source);
        if (!allowError)
        {
            tree.Diagnostics.Should().BeEmpty();
        }
        var program = IrBuilder.Program(tree, moduleLoader);
        if (!allowError)
        {
            program.Diagnostics.Should().BeEmpty();
            program.Syntax.Should().Be(tree.Root);
        }
        return program;
    }
}

internal class NullModuleLoader : IExternalModuleLoader
{
    public bool TryLookupModule(SymbolName name, [NotNullWhen(true)] out ExternalModule? module)
    {
        module = new ExternalModule(name, Enumerable.Empty<FunctionSymbol>(), Enumerable.Empty<TypeSymbol>());
        return true;
    }

    public List<ExternalModule> LookupNamespace(SymbolNamespace @namespace) => new List<ExternalModule>();
}