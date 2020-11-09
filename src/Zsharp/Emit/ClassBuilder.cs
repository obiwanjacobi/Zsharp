using Mono.Cecil;
using System;
using System.Linq;
using Zsharp.AST;

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
            var moduleDefinition = context.Module;
            var typeDef = new TypeDefinition(moduleDefinition.Name, module.Name, ToTypeAttributes(module))
            {
                BaseType = moduleDefinition.ImportReference(typeof(Object))
            };
            moduleDefinition.Types.Add(typeDef);
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
            if (ModuleInitializer.Body.Instructions.Count > 0)
            {
                _typeDefinition.Methods.Add(ModuleInitializer);
            }
        }

        public MethodDefinition AddFunction(AstFunctionDefinition function)
        {
            var methodDef = new MethodDefinition(
                function.Identifier.Name,
                ToMethodAttibutes(function),
                _context.ToTypeReference(function.TypeReference));

            foreach (var p in function.Parameters)
            {
                methodDef.Parameters.Add(new ParameterDefinition(
                    p.Identifier.Name,
                    ToParameterAttributes(p),
                    _context.ToTypeReference(p.TypeReference)
                    ));
            }

            _typeDefinition.Methods.Add(methodDef);
            return methodDef;
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
