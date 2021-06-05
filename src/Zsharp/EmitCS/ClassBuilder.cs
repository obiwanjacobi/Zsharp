using Zsharp.AST;

namespace Zsharp.EmitCS
{
    internal sealed class ClassBuilder
    {
        private readonly EmitContext _context;
        private readonly CSharp.Class _moduleClass;

        private ClassBuilder(EmitContext context, CSharp.Class moduleClass)
        {
            _context = context;
            _moduleClass = moduleClass;
        }

        public static ClassBuilder Create(EmitContext context, AstModulePublic module)
        {
            var moduleClass = new CSharp.Class(module.Identifier!.CanonicalName, ClassKeyword.Class)
            {
                AccessModifiers = module.HasExports
                    ? AccessModifiers.Public
                    : AccessModifiers.Internal,
                ClassModifiers = ClassModifiers.Static,
            };
            context.Namespace.AddClass(moduleClass);

            return new ClassBuilder(context, moduleClass);
        }

        public CSharp.Class ModuleClass => _moduleClass;

        public void AddField(AstVariableDefinition variable)
        {
            var field = new CSharp.Field(variable.Identifier!.CanonicalName, variable.TypeReference.ToCode())
            {
                AccessModifiers = AccessModifiers.Private,
                FieldModifiers = FieldModifiers.Static,
            };

            _moduleClass.AddField(field);
        }

        public CSharp.Enum AddEnum(AstTypeDefinitionEnum enumDef)
        {
            var enumType = new CSharp.Enum(enumDef.Identifier!.CanonicalName)
            {
                AccessModifiers = enumDef.Symbol!.SymbolLocality == AstSymbolLocality.Exported
                    ? AccessModifiers.Public : AccessModifiers.Private,
                BaseTypeName = enumDef.BaseType.ToCode(),
            };

            foreach (var field in enumDef.Fields)
            {
                var option = new CSharp.EnumOption(field.Identifier!.CanonicalName)
                {
                    Value = field.Expression.ToCode()
                };

                enumType.AddOption(option);
            }

            _moduleClass.AddEnum(enumType);

            return enumType;
        }

        public CSharp.Class AddStruct(AstTypeDefinitionStruct structDef)
        {
            var recordType = new CSharp.Class(structDef.Identifier!.CanonicalName, ClassKeyword.Record)
            {
                AccessModifiers = structDef.Symbol!.SymbolLocality == AstSymbolLocality.Exported
                    ? AccessModifiers.Public : AccessModifiers.Private,
                ClassModifiers = ClassModifiers.None,
                BaseTypeName = structDef.BaseType.ToCode(),
            };

            foreach (var field in structDef.Fields)
            {
                var property = new CSharp.Property(field.Identifier!.CanonicalName, field.TypeReference.ToCode())
                {
                    AccessModifiers = AccessModifiers.Public,
                };
                recordType.AddProperty(property);
            }

            _moduleClass.AddClass(recordType);

            return recordType;
        }

        public CSharp.Method AddFunction(AstFunctionDefinition function)
        {
            var method = new CSharp.Method(function.Identifier!.CanonicalName, function.FunctionType.TypeReference.ToCode())
            {
                AccessModifiers = function.Symbol!.SymbolLocality == AstSymbolLocality.Exported
                    ? AccessModifiers.Public : AccessModifiers.Private,
                MethodModifiers = MethodModifiers.Static,
            };

            foreach (var parameter in function.FunctionType.Parameters)
            {
                method.AddParameter(
                    new CSharp.Parameter(
                        parameter.Identifier!.CanonicalName,
                        parameter.TypeReference.ToCode()
                    )
                );
            }

            _moduleClass.AddMethod(method);

            return method;
        }
    }
}
