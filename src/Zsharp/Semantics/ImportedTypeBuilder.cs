using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class ImportedTypeBuilder
    {
        private readonly List<AstFunctionExternal> _functions = new List<AstFunctionExternal>();
        private readonly Dictionary<string, string> _aliases = new Dictionary<string, string>();
        private readonly ExternalTypeRepository _typeRepository;

        public ImportedTypeBuilder(ExternalTypeRepository typeRepository)
        {
            _typeRepository = typeRepository;
            Namespace = String.Empty;
        }

        public void Build(TypeDefinition typeDefinition)
        {
            Namespace = typeDefinition.Namespace;

            if (typeDefinition.IsEnum)
            {
                BuildEnum(typeDefinition);
            }
            else if (typeDefinition.IsValueType)
            {
                BuildStruct(typeDefinition);
            }
            else if (typeDefinition.IsClass)
            {
                BuildClass(typeDefinition);
            }
            else
                throw new NotImplementedException(
                    $"No implementation for Type {typeDefinition.FullName}");
        }

        public AstTypeDefinitionExternal? StructType { get; private set; }

        public IEnumerable<AstFunctionExternal> Functions => _functions;

        public IEnumerable<KeyValuePair<string, string>> Aliases => _aliases;

        public string Namespace { get; internal set; }

        private void BuildEnum(TypeDefinition typeDefinition)
        {

        }

        private void BuildStruct(TypeDefinition typeDefinition)
        {

        }

        private void BuildClass(TypeDefinition typeDefinition)
        {
            if (IsStructType(typeDefinition))
            {
                StructType = new AstTypeDefinitionExternal(typeDefinition.Name,
                    _typeRepository.GetTypeReference(typeDefinition.BaseType));
            }
            else
            {
                // full name for static classes
                Namespace = typeDefinition.FullName;
            }

            foreach (var methodGroup in typeDefinition.Methods
                .Where(m => m.IsPublic).GroupBy(m => m.Name))
            {
                int overloadIndex = 0;
                foreach (var method in methodGroup)
                {
                    var function = CreateFunction(method, overloadIndex);
                    _functions.Add(function);

                    if (method.IsStatic)
                    {
                        // TODO: get_/set_ property methods name handling
                        _aliases.TryAdd(function.Identifier.Name,
                            $"{typeDefinition.Name}{function.Identifier.Name}");
                    }

                    overloadIndex++;
                }
            }
        }

        // copy over the types to the module
        public void AddTo(AstModuleExternal module)
        {
            if (StructType != null)
            {
                module.AddTypeDefinition(StructType);
            }
            foreach (var function in Functions)
            {
                module.AddFunction(function);
            }
            foreach (var alias in Aliases)
            {
                module.AddAlias(alias.Key, alias.Value);
            }
        }

        private bool IsStructType(TypeDefinition typeDefinition)
        {
            return
                !(typeDefinition.IsAbstract && typeDefinition.IsSealed) &&
                typeDefinition.GetConstructors().Any() &&
                typeDefinition.Methods.Any(m => !m.IsStatic && m.IsPublic);
        }

        private AstFunctionExternal CreateFunction(MethodDefinition method, int overloadIndex)
        {
            var function = new AstFunctionExternal(method);
            var functionName = overloadIndex > 0 ? $"{method.Name}{overloadIndex + 1}" : method.Name;
            function.SetIdentifier(new AstIdentifierExternal(functionName, AstIdentifierType.Function));

            // TODO: special Void handling
            var typeRef = _typeRepository.GetTypeReference(method.ReturnType);
            function.SetTypeReference(typeRef);

            AstFunctionParameter funcParam;
            if (!method.IsStatic)
            {
                funcParam = CreateParameter(AstIdentifierIntrinsic.Self, method.DeclaringType);
                function.TryAddParameter(funcParam);
            }

            foreach (var p in method.Parameters)
            {
                funcParam = CreateParameter(new AstIdentifierExternal(p.Name, AstIdentifierType.Parameter), p.ParameterType);
                function.TryAddParameter(funcParam);
            }

            return function;
        }

        private AstFunctionParameter CreateParameter(AstIdentifier identifier, TypeReference type)
        {
            var funcParam = new AstFunctionParameter();
            funcParam.SetIdentifier(identifier);
            var typeRef = _typeRepository.GetTypeReference(type);
            funcParam.SetTypeReference(typeRef);
            return funcParam;
        }
    }
}
