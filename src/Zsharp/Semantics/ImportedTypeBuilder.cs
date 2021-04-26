﻿using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class ImportedTypeBuilder
    {
        private readonly List<AstFunctionDefinitionExternal> _functions = new();
        private readonly Dictionary<string, string> _aliases = new();
        private readonly ExternalTypeRepository _typeRepository;

        public ImportedTypeBuilder(ExternalTypeRepository typeRepository)
        {
            _typeRepository = typeRepository;
            ModuleName = String.Empty;
        }

        public void Build(TypeDefinition typeDefinition)
        {
            ModuleName = typeDefinition.FullName;

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

        public string ModuleName { get; private set; }

        public AstTypeDefinitionExternal? StructType { get; private set; }

        public IEnumerable<AstFunctionDefinitionExternal> Functions => _functions;

        public IEnumerable<KeyValuePair<string, string>> Aliases => _aliases;

        private void BuildEnum(TypeDefinition typeDefinition)
        {

        }

        private void BuildStruct(TypeDefinition typeDefinition)
        {

        }

        private void BuildClass(TypeDefinition typeDefinition)
        {
            if (IsInstanceType(typeDefinition))
            {
                StructType = new AstTypeDefinitionExternal(
                    typeDefinition.Namespace,
                    typeDefinition.Name,
                    _typeRepository.GetTypeReference(typeDefinition.BaseType));
            }

            foreach (var methodGroup in typeDefinition.Methods
                .Where(m => m.IsPublic).GroupBy(m => m.Name))
            {
                foreach (var method in methodGroup)
                {
                    var function = CreateFunction(method);
                    _functions.Add(function);

                    if (method.IsStatic)
                    {
                        // TODO: get_/set_ property methods name handling
                        _aliases.TryAdd(function.Identifier.CanonicalName,
                            $"{typeDefinition.Name}{function.Identifier.CanonicalName}");
                    }
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

        private bool IsInstanceType(TypeDefinition typeDefinition)
        {
            return
                !(typeDefinition.IsAbstract && typeDefinition.IsSealed) &&
                typeDefinition.GetConstructors().Any() &&
                typeDefinition.Methods.Any(m => !m.IsStatic && m.IsPublic);
        }

        private AstFunctionDefinitionExternal CreateFunction(MethodDefinition method)
        {
            var function = new AstFunctionDefinitionExternal(method, !method.IsStatic);
            // TODO: get_/set_ and .ctor handling
            function.SetIdentifier(new AstIdentifier(method.Name, AstIdentifierType.Function));

            // TODO: special Void handling
            var typeRef = _typeRepository.GetTypeReference(method.ReturnType);
            function.SetTypeReference(typeRef);

            AstFunctionParameterDefinition funcParam;
            if (!method.IsStatic)
            {
                funcParam = CreateParameter(AstIdentifierIntrinsic.Self, method.DeclaringType);
                function.TryAddParameter(funcParam);
            }

            foreach (var p in method.Parameters)
            {
                funcParam = CreateParameter(new AstIdentifier(p.Name, AstIdentifierType.Parameter), p.ParameterType);
                function.TryAddParameter(funcParam);
            }

            return function;
        }

        private AstFunctionParameterDefinition CreateParameter(AstIdentifier identifier, TypeReference type)
        {
            var funcParam = new AstFunctionParameterDefinition(identifier);
            var typeRef = _typeRepository.GetTypeReference(type);
            funcParam.SetTypeReference(typeRef);
            return funcParam;
        }
    }
}
