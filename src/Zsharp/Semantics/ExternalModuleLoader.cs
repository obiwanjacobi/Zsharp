using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class ExternalModuleLoader : IAstModuleLoader
    {
        private readonly Dictionary<string, AstModuleExternal> _modules = new Dictionary<string, AstModuleExternal>();
        private readonly Dictionary<string, AstTypeReferenceExternal> _typeReferences = new Dictionary<string, AstTypeReferenceExternal>();

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
            if (!_modules.TryGetValue(type.Namespace, out AstModuleExternal? module))
            {
                module = new AstModuleExternal(type.Namespace);
                _modules.Add(type.Namespace, module);
            }

            module.AddTypeDefinition(new AstTypeDefinitionExternal(
                type.Name, GetOrAddTypeReference(type.BaseType)));

            //foreach (var method in type.Methods)
            //{
            //    module.AddFunction(new AstFunctionExternal(method));
            //}
        }

        private AstTypeReferenceExternal GetOrAddTypeReference(TypeReference typeReference)
        {
            if (!_typeReferences.TryGetValue(typeReference.FullName, out AstTypeReferenceExternal? typeRef))
            {
                typeRef = new AstTypeReferenceExternal(typeReference);
                _typeReferences.Add(typeReference.FullName, typeRef);
            }
            return typeRef;
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
