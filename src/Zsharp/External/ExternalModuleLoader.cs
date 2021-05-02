﻿using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.External
{
    public class ExternalModuleLoader : IAstModuleLoader
    {
        private readonly Dictionary<string, AstModuleExternal> _modules = new();
        private readonly ExternalTypeRepository _typeRepository = new();

        public ExternalModuleLoader(AssemblyManager assemblies)
        {
            Assemblies = assemblies;
            CreateExternalModules(assemblies.Assemblies);
        }

        private void CreateExternalModules(IEnumerable<AssemblyDefinition> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                CreateExternalModules(assembly);
            }
        }

        private void CreateExternalModules(AssemblyDefinition assembly)
        {
            foreach (var mod in assembly.Modules)
            {
                foreach (var type in mod.Types.Where(t => t.IsPublic))
                {
                    AddType(type);
                }
            }
        }

        private void AddType(TypeDefinition type)
        {
            var builder = new ImportedTypeBuilder(_typeRepository);
            builder.Build(type);

            if (!_modules.TryGetValue(builder.ModuleName, out AstModuleExternal? module))
            {
                module = new AstModuleExternal(type.Namespace, builder.ModuleName);
                _modules.Add(builder.ModuleName, module);
            }

            builder.AddTo(module);
        }

        public AssemblyManager Assemblies { get; }

        public IEnumerable<AstModuleExternal> Modules => _modules.Values;

        public AstModuleExternal? LoadExternal(string moduleName)
        {
            _modules.TryGetValue(moduleName, out AstModuleExternal? module);
            return module;
        }
    }
}