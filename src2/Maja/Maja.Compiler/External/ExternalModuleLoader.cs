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
    private readonly List<AssemblyMetadata> _assemblies = new();

    public ExternalModuleLoader(AssemblyManager assemblyManager)
    {
        _assemblyManager = assemblyManager;
    }

    public IEnumerable<string> Assemblies => _assemblies.Select(a => a.Location);

    public bool TryLookupModule(SymbolName name,
        [NotNullWhen(true)] out ExternalModule? module)
    {
        if (TryLookupType(name, out var typeMetadata, out var symbolName))
        {
            AddAssembly(typeMetadata.Assembly);
            module = CreateExternalModule(symbolName, typeMetadata);
            return true;
        }

        module = null;
        return false;
    }

    public List<ExternalModule> LookupNamespace(SymbolNamespace @namespace)
    {
        var modules = new List<ExternalModule>();
        var assemblies = AssembliesForNamespace(@namespace);

        foreach (var assembly in assemblies)
        {
            foreach (var typeMetadata in assembly.GetPublicTypes())
            {
                if (typeMetadata.Namespace == @namespace.OriginalName)
                {
                    var symbolName = new SymbolName(typeMetadata.Namespace, typeMetadata.Name);
                    var module = CreateExternalModule(symbolName, typeMetadata);

                    AddAssembly(typeMetadata.Assembly);
                    modules.Add(module);
                }
            }
        }

        return modules;
    }

    public bool TryLookupOperator(string operatorSymbol, TypeSymbol returnType, IEnumerable<TypeSymbol> parameterTypes, [NotNullWhen(true)] out DeclaredFunctionSymbol? function)
    {
        var selector = new OperatorFunctionSelector(operatorSymbol, returnType, parameterTypes);
        foreach (var assembly in _assemblyManager.Assemblies)
        {
            foreach (var type in assembly.GetPublicTypes())
            {
                var result = type.GetOperatorFunctions()
                    .Where(selector.IsMatch);

                if (result.Any())
                {
                    AddAssembly(type.Assembly);
                    function = _factory.Create(type, result.Single());
                    return true;
                }
            }
        }

        function = null;
        return false;
    }

    private ExternalModule CreateExternalModule(SymbolName symbolName, TypeMetadata typeMetadata)
    {
        var functions = new List<DeclaredFunctionSymbol>();
        var type = _factory.Create(typeMetadata);

        foreach (var method in typeMetadata.GetFunctions())
        {
            var fn = _factory.Create(typeMetadata, method);
            functions.Add(fn);
        }

        var types = new List<TypeSymbol>() { type };
        types.AddRange(typeMetadata.GetNestedTypes()
            .Select(_factory.Create)
            );

        return new ExternalModule(symbolName, functions, types);
    }

    private bool TryLookupType(SymbolName name,
        [NotNullWhen(true)] out TypeMetadata? typeMetadata,
        [NotNullWhen(true)] out SymbolName? symbolName)
    {
        var assembliesWithNamespace = AssembliesForNamespace(name.Namespace);

        foreach (var assembly in assembliesWithNamespace)
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

    private IEnumerable<AssemblyMetadata> AssembliesForNamespace(SymbolNamespace @namespace)
        => _assemblyManager.Assemblies.Where(a => a.GetNamespaces().Contains(@namespace));

    private void AddAssembly(AssemblyMetadata assembly)
    {
        if (_assemblies.Contains(assembly)) return;
        _assemblies.Add(assembly);
    }
}
