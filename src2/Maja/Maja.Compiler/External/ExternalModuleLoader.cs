using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.External.Metadata;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal sealed class ExternalModuleLoader : IExternalModuleLoader
{
    private readonly AssemblyManager _assemblyManager;
    private readonly ExternalTypeFactory _factory = new();

    public ExternalModuleLoader(AssemblyManager assemblyManager)
    {
        _assemblyManager = assemblyManager;
    }

    public bool TryLookupModule(SymbolName name,
        [NotNullWhen(true)] out ExternalModule? module)
    {
        if (TryLookupType(name, out var typeMetadata, out var symbolName))
        {
            module = CreateExternalModule(symbolName, typeMetadata);
            return true;
        }

        module = null;
        return false;
    }

    private ExternalModule CreateExternalModule(SymbolName symbolName, TypeMetadata typeMetadata)
    {
        var functions = new List<FunctionSymbol>();
        var type = _factory.Create(typeMetadata);

        foreach (var method in typeMetadata.GetFunctions())
        {
            var parameters = new List<ParameterSymbol>();
            if (!method.IsStatic)
            {
                parameters.Add(new ParameterSymbol(ParameterSymbol.Self, type));
            }

            parameters.AddRange(method.Parameters
                .Select(p => new ParameterSymbol(p.Name,
                    _factory.Create(p.ParameterType)))
                );

            var rt = _factory.Create(method.ReturnType);
            var name = new SymbolName(typeMetadata.FullName, method.Name);
            var fn = new FunctionSymbol(name, parameters, rt);
            functions.Add(fn);
        }

        var types = new List<TypeSymbol>() { type };
        types.AddRange(typeMetadata.GetNestedTypes()
            .Select(t => _factory.Create(t))
            );

        return new ExternalModule(symbolName, functions, types);
    }

    private bool TryLookupType(SymbolName name,
        [NotNullWhen(true)] out TypeMetadata? typeMetadata,
        [NotNullWhen(true)] out SymbolName? symbolName)
    {
        foreach (var assembly in _assemblyManager.Assemblies)
        {
            foreach (var type in assembly.GetPublicTypes())
            {
                symbolName = new SymbolName(type.Namespace, type.Name);
                if (name.Equals(symbolName))
                {
                    typeMetadata = type;
                    return true;
                }
            }
        }

        symbolName = null;
        typeMetadata = null;
        return false;
    }
}
