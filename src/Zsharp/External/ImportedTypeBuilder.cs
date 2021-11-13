using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;
using Zsharp.External.Metadata;

namespace Zsharp.External
{
    public class ImportedTypeBuilder
    {
        private readonly List<AstFunctionDefinitionExternal> _functions = new();
        private readonly Dictionary<AstNode, string> _aliases = new();
        private readonly ExternalTypeRepository _typeRepository;

        public ImportedTypeBuilder(ExternalTypeRepository typeRepository)
        {
            _typeRepository = typeRepository;
            ModuleName = String.Empty;
        }

        public void Build(TypeMetadata typeDefinition)
        {
            ModuleName = AstSymbolName.ToCanonical(typeDefinition.FullName);

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
            else if (typeDefinition.IsInterface)
            {
                BuildInterface(typeDefinition);
            }
            else
                throw new NotImplementedException(
                    $"No implementation for Type {typeDefinition.FullName}");
        }

        public string ModuleName { get; private set; }

        public AstTypeDefinitionExternal? StructType { get; private set; }

        private void BuildInterface(TypeMetadata typeDefinition)
        {

        }

        private void BuildEnum(TypeMetadata typeDefinition)
        {

        }

        private void BuildStruct(TypeMetadata typeDefinition)
        {

        }

        private void BuildClass(TypeMetadata typeDefinition)
        {
            if (IsInstanceType(typeDefinition))
            {
                AstTypeReference? baseType = null;

                if (typeDefinition.HasBaseType)
                {
                    baseType = _typeRepository.GetTypeReference(typeDefinition.GetBaseType());
                }

                StructType = new AstTypeDefinitionExternal(
                    typeDefinition.FullName,
                    baseType);

                if (typeDefinition.ContainsGenericParameter)
                {
                    AddGenericParametersTo(StructType, typeDefinition.GenericParameters);
                }
            }

            foreach (var methodGroup in typeDefinition.GetPublicMethods().GroupBy(m => m.Name))
            {
                foreach (var method in methodGroup)
                {
                    var function = CreateFunction(method);
                    _functions.Add(function);

                    if (method.IsStatic)
                    {
                        var aliasName = AstName.ParseFullName($"{typeDefinition.Name}{function.Identifier.SymbolName.CanonicalName.Name}");
                        _aliases.TryAdd(function, aliasName.FullName);
                    }
                }
            }
        }

        // copy the types over to the module
        public void AddTo(AstModuleExternal module)
        {
            if (StructType is not null)
            {
                module.AddTypeDefinition(StructType);
            }
            foreach (var function in _functions)
            {
                module.AddFunction(function);
            }
            foreach (var alias in _aliases)
            {
                module.AddAlias(alias.Key, alias.Value);
            }
        }

        private bool IsInstanceType(TypeMetadata typeDefinition)
        {
            return
                !(typeDefinition.IsAbstract && typeDefinition.IsSealed) &&
                typeDefinition.HasConstructors &&
                typeDefinition.GetPublicMethods().Any(m => !m.IsStatic);
        }

        private AstFunctionDefinitionExternal CreateFunction(MethodMetadata method)
        {
            var declType = method.GetDeclaringType();
            var function = new AstFunctionDefinitionExternal(method, !method.IsStatic);
            var methodName = method.Name == ".ctor" ? declType.Name : method.Name;
            var name = AstName.FromExternal(declType.FullName, methodName);
            function.SetIdentifier(new AstIdentifier(name, AstIdentifierKind.Function));

            var typeRef = _typeRepository.GetTypeReference(method.ReturnType);
            function.FunctionType.SetTypeReference(typeRef);

            AstFunctionParameterDefinition funcParam;
            if (!method.IsStatic)
            {
                funcParam = CreateParameter(AstIdentifierIntrinsic.Self, declType);
                function.FunctionType.TryAddParameter(funcParam);
            }

            foreach (var p in method.Parameters)
            {
                funcParam = CreateParameter(new AstIdentifier(p.Name, AstIdentifierKind.Parameter), p.ParameterType);
                function.FunctionType.TryAddParameter(funcParam);
            }

            AddGenericParametersTo(function, method.GenericParameters);

            return function;
        }

        private AstFunctionParameterDefinition CreateParameter(AstIdentifier identifier, TypeMetadata type)
        {
            var funcParam = new AstFunctionParameterDefinition(identifier);
            var typeRef = _typeRepository.GetTypeReference(type);
            funcParam.SetTypeReference(typeRef);
            return funcParam;
        }

        private void AddGenericParametersTo(IAstGenericSite<AstGenericParameterDefinition> site, IEnumerable<GenericParameterMetadata> genericParameters)
        {
            foreach (var p in genericParameters)
            {
                var paramName = AstName.ParseFullName(p.Name, AstNameKind.External);
                var identifier = new AstIdentifier(paramName, AstIdentifierKind.TemplateParameter);
                var genericParam = new AstGenericParameterDefinition(identifier);
                site.AddGenericParameter(genericParam);

                foreach (var constraintType in p.GetConstraintTypes())
                {
                    var typeRef = _typeRepository.GetTypeReference(constraintType);
                    genericParam.AddConstraintType(typeRef);
                }
            }
        }
    }
}
