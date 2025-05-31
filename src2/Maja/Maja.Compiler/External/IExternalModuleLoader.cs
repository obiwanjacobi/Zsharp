using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal interface IExternalModuleLoader
{
    // returns assemblies that have been 'hit' during compilation
    IEnumerable<string> Assemblies { get; }

    bool TryLookupModule(SymbolName name, [NotNullWhen(true)] out ExternalModule? module);
    List<ExternalModule> LookupNamespace(SymbolNamespace @namespace);

    bool TryLookupOperator(string operatorSymbol, TypeSymbol returnType, IEnumerable<TypeSymbol> parameterTypes,
        [NotNullWhen(true)] out DeclaredFunctionSymbol? function);
}
