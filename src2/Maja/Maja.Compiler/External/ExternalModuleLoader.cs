using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Maja.Compiler.External.Metadata;
using Maja.Compiler.Symbol;

namespace Maja.Compiler.External;

internal sealed class ExternalModuleLoader : IExternalModuleLoader
{
    private readonly AssemblyManager _assemblyManager;

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

        var type = CreateExternalType(typeMetadata.Namespace, typeMetadata.Name);

        foreach (var method in typeMetadata.GetPublicMethods())
        {
            var parameters = new List<ParameterSymbol>();
            if (!method.IsStatic)
            {
                parameters.Add(new ParameterSymbol(ParameterSymbol.Self, type));
            }
            foreach (var param in method.Parameters)
            {
                var pt = CreateExternalType(param.ParameterType.Namespace, param.ParameterType.Name);
                var p = new ParameterSymbol(param.Name, pt);

                parameters.Add(p);
            }

            var rt = CreateExternalType(method.ReturnType.Namespace, method.ReturnType.Name);
            var name = new SymbolName(typeMetadata.FullName, method.Name);
            var fn = new FunctionSymbol(name, parameters, rt);
            functions.Add(fn);
        }

        var types = new List<TypeSymbol>
        {
            type
        };
        foreach (var nType in typeMetadata.GetNestedTypes())
        {
            var nt = CreateExternalType(nType.Namespace, nType.Name);
            types.Add(nt);
        }

        return new ExternalModule(symbolName, functions, types);
    }

    private TypeSymbol CreateExternalType(string ns, string name)
    {
        // TODO: cache created types + lookup
        return MajaTypeMapper.MapToMajaType(ns, name);
    }

    private bool TryLookupType(SymbolName name,
        [NotNullWhen(true)] out TypeMetadata? typeMetadata,
        [NotNullWhen(true)] out SymbolName? symbolName)
    {
        foreach (var assembly in _assemblyManager.Assemblies)
        {
            foreach (var type in assembly.GetPublicTypes())
            {
                symbolName = SymbolName.Parse(type.Namespace, type.Name);
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
