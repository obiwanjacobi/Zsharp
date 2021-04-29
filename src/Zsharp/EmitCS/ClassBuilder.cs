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
            var field = new CSharp.Field()
            {
                AccessModifiers = AccessModifiers.Private,
                FieldModifiers = FieldModifiers.Static,
                Name = variable.Identifier!.CanonicalName,
                TypeName = variable.TypeReference.ToCode(),
            };

            _moduleClass.AddField(field);
        }

        public CSharp.Enum AddEnum(AstTypeDefinitionEnum enumDef)
        {
            var enumType = new CSharp.Enum()
            {
                AccessModifiers = enumDef.Symbol!.SymbolLocality == AstSymbolLocality.Exported
                    ? AccessModifiers.Public : AccessModifiers.Private,
                BaseTypeName = enumDef.BaseType.ToCode(),
                Name = enumDef.Identifier!.CanonicalName,
            };

            foreach (var field in enumDef.Fields)
            {
                var option = new CSharp.EnumOption()
                {
                    Name = field.Identifier!.CanonicalName,
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
                var property = new CSharp.Property
                {
                    AccessModifiers = AccessModifiers.Public,
                    Name = field.Identifier!.CanonicalName,
                    TypeName = field.TypeReference.ToCode(),
                };
                recordType.AddProperty(property);
            }

            _moduleClass.AddClass(recordType);

            return recordType;
        }

        public CSharp.Method AddFunction(AstFunctionDefinition function)
        {
            var method = new CSharp.Method(function.Identifier!.CanonicalName)
            {
                AccessModifiers = function.Symbol!.SymbolLocality == AstSymbolLocality.Exported
                    ? AccessModifiers.Public : AccessModifiers.Private,
                MethodModifiers = MethodModifiers.Static,
                TypeName = function.TypeReference.ToCode(),
            };

            foreach (var parameter in function.Parameters)
            {
                method.AddParameter(new CSharp.Parameter
                {
                    Name = parameter.Identifier!.CanonicalName,
                    TypeName = parameter.TypeReference.ToCode()
                });
            }

            _moduleClass.AddMethod(method);

            return method;
        }
    }
}
