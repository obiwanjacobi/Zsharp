using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;
using Zsharp.External.Metadata;

namespace Zsharp.External
{
    public class ExternalModuleLoader : IAstModuleLoader
    {
        private readonly Dictionary<string, AstModuleExternal> _modules = new();
        private readonly ExternalTypeRepository _typeRepository = new();
        private readonly AssemblyManager _assemblies;

        public ExternalModuleLoader(AssemblyManager assemblies)
        {
            _assemblies = assemblies;
        }

        private void CreateExternalModules(IEnumerable<AssemblyMetadata> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                CreateExternalModules(assembly);
            }
        }

        private void CreateExternalModules(AssemblyMetadata assembly)
        {
            foreach (var type in assembly.GetPublicTypes())
            {
                AddType(type);
            }
        }

        private void AddType(TypeMetadata type)
        {
            var builder = new ImportedTypeBuilder(_typeRepository);
            builder.Build(type);

            var module = GetModule(builder.ModuleName);
            builder.AddTo(module);
        }

        private AstModuleExternal GetModule(string moduleName)
        {
            if (!_modules.TryGetValue(moduleName, out AstModuleExternal? module))
            {
                module = new AstModuleExternal(moduleName, SymbolTable);
                _modules.Add(moduleName, module);
            }

            return module;
        }

        private AstSymbolTable? _symbolTable;
        public AstSymbolTable SymbolTable => _symbolTable ?? throw new InternalErrorException("Initialize() was not called.");

        public void Initialize(AstSymbolTable symbolTable)
        {
            _symbolTable = symbolTable ?? throw new ArgumentNullException(nameof(symbolTable));
            CreateExternalModules(_assemblies.Assemblies);
        }

        public AstModuleExternal? LoadExact(string fullModuleName)
        {
            _modules.TryGetValue(fullModuleName, out AstModuleExternal? module);
            return module;
        }

        public IEnumerable<AstModuleExternal> LoadNamespace(string moduleNamespace)
            => _modules.Values
                .Where(m => m.Identifier.SymbolName.CanonicalName.Namespace == moduleNamespace)
                .ToList();

        public IEnumerable<AstModuleExternal> LoadAll(string partialModuleName)
            => _modules.Keys
                .Where(k => k.StartsWith(partialModuleName))
                .Select(k => _modules[k])
                .ToList();
    }
}
