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

        public static ClassBuilder Create(EmitContext context, AstModuleImpl module)
        {
            var moduleClass = new CSharp.Class(module.Identifier.SymbolName.CanonicalName.FullName, ClassKeyword.Class)
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
            var field = new CSharp.Field(variable.Identifier.SymbolName.CanonicalName.FullName, variable.TypeReference.ToCode())
            {
                AccessModifiers = AccessModifiers.Private,
                FieldModifiers = FieldModifiers.Static,
            };

            _moduleClass.AddField(field);
        }

        public CSharp.Enum AddEnum(AstTypeDefinitionEnum enumDef)
        {
            var enumType = new CSharp.Enum(enumDef.Identifier.SymbolName.CanonicalName.FullName)
            {
                AccessModifiers = enumDef.Symbol.SymbolLocality == AstSymbolLocality.Exported
                    ? AccessModifiers.Public : AccessModifiers.Private,
                BaseTypeName = enumDef.BaseType.ToCode(),
            };

            foreach (var field in enumDef.Fields)
            {
                var option = new CSharp.EnumOption(field.Identifier.SymbolName.CanonicalName.FullName)
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
            var recordType = new CSharp.Class(structDef.Identifier.SymbolName.CanonicalName.FullName, ClassKeyword.Record)
            {
                AccessModifiers = structDef.Symbol.SymbolLocality == AstSymbolLocality.Exported
                    ? AccessModifiers.Public : AccessModifiers.Private,
                ClassModifiers = ClassModifiers.None,
            };

            if (structDef.HasBaseType)
                recordType.BaseTypeName = structDef.BaseType.ToCode();

            foreach (var field in structDef.Fields)
            {
                var property = new CSharp.Property(field.Identifier.SymbolName.CanonicalName.FullName, field.TypeReference.ToCode())
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
            var method = new CSharp.Method(function.Identifier.SymbolName.CanonicalName.FullName, function.FunctionType.TypeReference.ToCode())
            {
                AccessModifiers = function.Symbol.SymbolLocality == AstSymbolLocality.Exported
                    ? AccessModifiers.Public : AccessModifiers.Private,
                MethodModifiers = MethodModifiers.Static,
            };

            foreach (var parameter in function.FunctionType.Parameters)
            {
                method.AddParameter(
                    new CSharp.Parameter(
                        parameter.Identifier.SymbolName.CanonicalName.FullName,
                        parameter.TypeReference.ToCode()
                    )
                );
            }

            _moduleClass.AddMethod(method);

            return method;
        }
    }
}
