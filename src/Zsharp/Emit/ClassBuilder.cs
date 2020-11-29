using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using Zsharp.AST;
using Zsharp.Semantics;

namespace Zsharp.Emit
{
    public sealed class ClassBuilder : IDisposable
    {
        private readonly EmitContext _context;
        private readonly TypeDefinition _typeDefinition;

        private ClassBuilder(EmitContext context, TypeDefinition typeDefinition)
        {
            _context = context;
            _typeDefinition = typeDefinition;
            ModuleInitializer = CreateModuleInitializer();
        }

        public static ClassBuilder Create(EmitContext context, AstModulePublic module)
        {
            var typeDef = CreateType(
                context.Module, context.Module.Name, module.Name, ToTypeAttributes(module));
            context.Module.Types.Add(typeDef);
            return new ClassBuilder(context, typeDef);
        }

        public FieldDefinition AddField(string name, TypeReference typeReference)
        {
            var fieldAttrs = FieldAttributes.Private | FieldAttributes.Static;
            var fieldDef = new FieldDefinition(name, fieldAttrs, typeReference);
            _typeDefinition.Fields.Add(fieldDef);
            return fieldDef;
        }

        public bool HasField(string name)
        {
            return _typeDefinition.Fields.Any(f => f.Name == name);
        }

        public FieldDefinition GetField(string name)
        {
            var field = _typeDefinition.Fields.Find(name);
            if (field == null)
                throw new ArgumentException(
                    $"Variable backup Field '{name}' was not found.", nameof(name));
            return field;
        }

        public MethodDefinition ModuleInitializer { get; }

        public void Dispose()
        {
            // ignore initializer if it's empty or just a Ret.
            if (ModuleInitializer.Body.Instructions.Count > 0 &&
                ModuleInitializer.Body.Instructions[0].OpCode != OpCodes.Ret)
            {
                _typeDefinition.Methods.Add(ModuleInitializer);
            }
        }

        public MethodDefinition AddFunction(AstFunctionDefinition function)
        {
            var methodDef = new MethodDefinition(
                function.Identifier.CanonicalName,
                ToMethodAttibutes(function),
                _context.ToTypeReference(function.TypeReference));

            foreach (var p in function.Parameters)
            {
                methodDef.Parameters.Add(new ParameterDefinition(
                    p.Identifier.CanonicalName,
                    ToParameterAttributes(p),
                    _context.ToTypeReference(p.TypeReference)
                    ));
            }

            _typeDefinition.Methods.Add(methodDef);
            return methodDef;
        }

        public TypeDefinition AddTypeStruct(AstTypeDefinitionStruct structType)
        {
            var typeDef = CreateType(String.Empty, structType.Identifier.CanonicalName, ToTypeAttributes(structType), EmitContext.DotNet.ValueType);

            foreach (var field in structType.Fields)
            {
                var typeRef = _context.ToTypeReference(field.TypeReference);
                var fieldAttrs = FieldAttributes.Public;
                var fieldDef = new FieldDefinition(field.Identifier.CanonicalName, fieldAttrs, typeRef);
                typeDef.Fields.Add(fieldDef);
            }

            _typeDefinition.NestedTypes.Add(typeDef);
            return typeDef;
        }

        public TypeDefinition AddTypeEnum(AstTypeDefinitionEnum enumType)
        {
            var typeDef = CreateType(String.Empty, enumType.Identifier.CanonicalName, ToTypeAttributes(enumType), EmitContext.DotNet.EnumType);
            var typeRef = _context.ToTypeReference(enumType.BaseType!);

            var fieldAttrs = FieldAttributes.Public | FieldAttributes.SpecialName | FieldAttributes.RTSpecialName;
            var fieldDef = new FieldDefinition("value__", fieldAttrs, typeRef);
            typeDef.Fields.Add(fieldDef);

            foreach (var field in enumType.Fields)
            {
                fieldAttrs = FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal;
                fieldDef = new FieldDefinition(field.Identifier.CanonicalName, fieldAttrs, typeDef)
                {
                    Constant = field.Expression.ConstantValue()
                };
                typeDef.Fields.Add(fieldDef);
            }

            _typeDefinition.NestedTypes.Add(typeDef);
            return typeDef;
        }

        private TypeDefinition CreateType(string ns, string typeName, TypeAttributes typeAttributes, TypeReference? baseType = null)
        {
            return CreateType(_context.Module, ns, typeName, typeAttributes, baseType);
        }

        private static TypeDefinition CreateType(ModuleDefinition moduleDefinition,
            string ns, string typeName, TypeAttributes typeAttributes, TypeReference? baseType = null)
        {
            var typeDef = new TypeDefinition(ns, typeName, typeAttributes);
            if (baseType != null)
            {
                typeDef.BaseType = moduleDefinition.ImportReference(baseType);
            }
            else
            {
                typeDef.BaseType = moduleDefinition.ImportReference(EmitContext.DotNet.ObjectType);
            }
            return typeDef;
        }

        private MethodDefinition CreateModuleInitializer()
        {
            var methodDef = new MethodDefinition(
                ".cctor",
                MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig |
                    MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                _context.Module.TypeSystem.Void);
            return methodDef;
        }

        private static TypeAttributes ToTypeAttributes(AstModulePublic module)
        {
            var attrs = TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed;
            attrs |= module.HasExports ? TypeAttributes.Public : TypeAttributes.NotPublic;
            return attrs;
        }

        private static TypeAttributes ToTypeAttributes(AstTypeDefinitionStruct typeDefinition)
        {
            var attrs = TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.SequentialLayout | TypeAttributes.BeforeFieldInit;
            attrs |= typeDefinition.Symbol.SymbolLocality == AstSymbolLocality.Exported
                ? TypeAttributes.NestedPublic
                : TypeAttributes.NestedPrivate;
            return attrs;
        }

        private static TypeAttributes ToTypeAttributes(AstTypeDefinitionEnum typeDefinition)
        {
            var attrs = TypeAttributes.Class | TypeAttributes.Sealed;
            attrs |= typeDefinition.Symbol.SymbolLocality == AstSymbolLocality.Exported
                ? TypeAttributes.NestedPublic
                : TypeAttributes.NestedPrivate;
            return attrs;
        }

        private MethodAttributes ToMethodAttibutes(IAstSymbolEntrySite function)
        {
            var attrs = MethodAttributes.Static | MethodAttributes.HideBySig;

            attrs |= (function.Symbol.SymbolLocality == AstSymbolLocality.Exported)
                ? MethodAttributes.Public : MethodAttributes.Private;

            return attrs;
        }

        private static ParameterAttributes ToParameterAttributes(AstFunctionParameter p)
        {
            var attrs = ParameterAttributes.In;

            if (p.TypeReference.IsOptional)
                attrs |= ParameterAttributes.Optional;

            return attrs;
        }
    }
}
