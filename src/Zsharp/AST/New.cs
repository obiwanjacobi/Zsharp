using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public static class New
    {
        public static AstAssignment AstAssignment(Variable_assign_structContext context)
            => new(context);
        public static AstAssignment AstAssignment(Variable_assign_valueContext context)
            => new(context);

        public static AstBranch AstBranch(Statement_breakContext context)
            => new(context, AstBranchKind.ExitLoop);
        public static AstBranch AstBranch(Statement_continueContext context)
            => new(context, AstBranchKind.ExitIteration);

        public static AstBranchConditional AstBranchConditional(Statement_ifContext context)
            => new(context);
        public static AstBranchConditional AstBranchConditional(Statement_elseContext context)
            => new(context);
        public static AstBranchConditional AstBranchConditional(Statement_elseifContext context)
            => new(context);

        public static AstBranchExpression AstBranchExpression(Statement_returnContext context)
            => new(context, AstBranchKind.ExitFunction);
        public static AstBranchExpression AstBranchExpression(Statement_loop_infiniteContext context)
        {
            var br = new AstBranchExpression(context, AstBranchKind.Loop);

            // loop true
            var trueExpression = new AstExpression(context);
            trueExpression.Add(
                new AstExpressionOperand(
                    new AstLiteralBoolean(true)));

            br.SetExpression(trueExpression);
            return br;
        }
        public static AstBranchExpression AstBranchExpression(Statement_loop_iterationContext context)
            => new(context, AstBranchKind.Loop);
        public static AstBranchExpression AstBranchExpression(Statement_loop_whileContext context)
            => new(context, AstBranchKind.Loop);

        public static AstExpressionRange AstExpressionRange(RangeContext context)
            => new(context);

        public static AstFile AstFile(string scopeName, AstSymbolTable parentTable, FileContext context)
            => new(scopeName, parentTable, context);

        public static AstFunctionDefinitionImpl AstFunctionDefinitionImpl(Function_defContext context)
            => new(context);

        public static AstFunctionParameterDefinition AstFunctionParameterDefinition(Function_parameterContext context)
            => new(context, isSelf: false);
        public static AstFunctionParameterDefinition AstFunctionParameterDefinition(Function_parameter_selfContext context)
            => new(context, isSelf: true);

        public static AstFunctionParameterReference AstFunctionParameterReference(Function_param_useContext context)
            => new(context);

        public static AstFunctionReference AstFunctionReference(Function_callContext context)
            => new(context, enforceReturnValueUse: context.Parent is not Function_call_retval_unusedContext);

        public static AstGenericParameterDefinition AstGenericParameterDefinition(Template_param_anyContext context)
            => new(context);
        public static AstGenericParameterReference AstGenericParameterReference(Template_param_useContext context)
            => new(context);

        public static AstIdentifier AstIdentifier(Identifier_typeContext context)
            => new(context, AstIdentifierKind.Type);
        public static AstIdentifier AstIdentifier(Identifier_varContext context)
            => new(context, AstIdentifierKind.Variable);
        public static AstIdentifier AstIdentifier(Identifier_paramContext context)
            => new(context, AstIdentifierKind.Parameter);
        public static AstIdentifier AstIdentifier(Identifier_funcContext context)
            => new(context, AstIdentifierKind.Function);
        public static AstIdentifier AstIdentifier(Identifier_fieldContext context)
            => new(context, AstIdentifierKind.Field);
        public static AstIdentifier AstIdentifier(Identifier_enumoptionContext context)
            => new(context, AstIdentifierKind.EnumOption);
        public static AstIdentifier AstIdentifier(Enum_option_useContext context)
            => new(context, AstIdentifierKind.EnumOption);
        public static AstIdentifier AstIdentifier(Identifier_template_paramContext context)
            => new(context, AstIdentifierKind.TemplateParameter);

        public static AstLiteralBoolean AstLiteralBoolean(Literal_boolContext context)
            => new(context, context.TRUE() is not null);

        public static AstTemplateParameterDefinition AstTemplateParameterDefinition(Template_param_anyContext context)
            => new(context);

        public static AstTemplateParameterReference AstTemplateParameterReference(Template_param_useContext context)
            => new(context);

        public static AstTypeDefinitionEnum AstTypeDefinitionEnum(Enum_defContext context, AstSymbolTable parentTable)
            => new(context, parentTable);

        public static AstTypeDefinitionEnumOption AstTypeDefinitionEnumOption(Enum_option_defContext context)
            => new(context);

        public static AstTypeDefinitionStruct AstTypeDefinitionStruct(Struct_defContext context, AstSymbolTable parentTable)
            => new(context, parentTable);

        public static AstTypeDefinitionStructField AstTypeDefinitionStructField(Struct_field_defContext context)
            => new(context);

        public static AstTypeFieldInitialization AstTypeFieldInitialization(Struct_field_initContext context)
            => new(context);

        public static AstTypeFieldReferenceEnumOption AstTypeFieldReferenceEnumOption(Enum_option_useContext context)
            => new(context);

        public static AstTypeFieldReferenceStructField AstTypeFieldReferenceStructField(Variable_field_refContext context)
            => new(context);

        public static AstTypeReferenceType AstTypeReferenceType(Type_refContext context)
            => new(context);

        public static AstVariableDefinition AstVariableDefinition(Variable_def_typedContext context)
            => new(context);
        public static AstVariableDefinition AstVariableDefinition(Variable_assign_valueContext context)
            => new(context);

        public static AstVariableReference AstVariableReference(Variable_refContext context)
            => new(context);
        public static AstVariableReference AstVariableReference(Variable_assign_structContext context)
            => new(context);
        public static AstVariableReference AstVariableReference(Variable_assign_valueContext context)
            => new(context);
    }
}
