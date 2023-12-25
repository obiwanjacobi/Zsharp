using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal interface IExternalModuleLoader
{
    bool TryLookupModule(SymbolName name, [NotNullWhen(true)] out ExternalModule? module);
    List<ExternalModule> LookupNamespace(SymbolNamespace @namespace);
}
