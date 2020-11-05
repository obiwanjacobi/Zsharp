using Mono.Cecil;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.Emit
{
    public class ClassBuilder
    {
        private readonly EmitContext _context;
        private readonly TypeDefinition _typeDefinition;

        private ClassBuilder(EmitContext context, TypeDefinition typeDefinition)
        {
            _context = context;
            _typeDefinition = typeDefinition;
        }

        public static ClassBuilder Create(EmitContext context, AstModulePublic module)
        {
            var moduleDefinition = context.Module;
            var typeDef = new TypeDefinition(moduleDefinition.Name, module.Name, ToTypeAttributes(module));
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

        private static TypeAttributes ToTypeAttributes(AstModulePublic module)
        {
            var attrs = TypeAttributes.Class;
            attrs |= module.HasExports ? TypeAttributes.Public : TypeAttributes.NotPublic;
            return attrs;
        }

        private MethodAttributes ToMethodAttibutes(AstFunction function)
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
