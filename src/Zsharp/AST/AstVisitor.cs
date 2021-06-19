namespace Zsharp.AST
{
    public class AstVisitor
    {
        public void Visit(AstNode node)
        {
            node.Accept(this);
        }

        public virtual void VisitAssignment(AstAssignment assign)
        {
            assign.VisitChildren(this);
        }

        public virtual void VisitBranch(AstBranch branch)
        {
            branch.VisitChildren(this);
        }

        public virtual void VisitBranchExpression(AstBranchExpression branch)
        {
            branch.VisitChildren(this);
        }

        public virtual void VisitBranchConditional(AstBranchConditional branch)
        {
            branch.VisitChildren(this);
        }

        public virtual void VisitCodeBlock(AstCodeBlock codeBlock)
        {
            codeBlock.VisitChildren(this);
        }

        public virtual void VisitExpression(AstExpression expression)
        {
            expression.VisitChildren(this);
        }

        public virtual void VisitExpressionOperand(AstExpressionOperand operand)
        {
            operand.VisitChildren(this);
        }

        public virtual void VisitFile(AstFile file)
        {
            file.VisitChildren(this);
        }

        public virtual void VisitFunctionDefinition(AstFunctionDefinition function)
        {
            function.VisitChildren(this);
        }

        public virtual void VisitFunctionReference(AstFunctionReference function)
        {
            function.VisitChildren(this);
        }

        public virtual void VisitFunctionParameterDefinition(AstFunctionParameterDefinition parameter)
        {
            parameter.VisitChildren(this);
        }

        public virtual void VisitFunctionParameterReference(AstFunctionParameterReference parameter)
        {
            parameter.VisitChildren(this);
        }

        public virtual void VisitModuleExternal(AstModuleExternal module)
        {
            module.VisitChildren(this);
        }

        public virtual void VisitModulePublic(AstModulePublic module)
        {
            module.VisitChildren(this);
        }

        public virtual void VisitLiteralNumeric(AstLiteralNumeric numeric)
        {
            numeric.VisitChildren(this);
        }

        public virtual void VisitLiteralBoolean(AstLiteralBoolean literalBool)
        {
            literalBool.VisitChildren(this);
        }

        public virtual void VisitLiteralString(AstLiteralString literalString)
        {
            literalString.VisitChildren(this);
        }

        public virtual void VisitTemplateInstanceFunction(AstTemplateInstanceFunction templateFunction)
        {
            templateFunction.VisitChildren(this);
        }

        public virtual void VisitTypeFieldDefinition(AstTypeFieldDefinition field)
        {
            field.VisitChildren(this);
        }

        public virtual void VisitTypeFieldInitialization(AstTypeFieldInitialization field)
        {
            field.VisitChildren(this);
        }

        public virtual void VisitTypeReferenceType(AstTypeReferenceType type)
        {
            type.VisitChildren(this);
        }

        public virtual void VisitTypeDefinitionFunction(AstTypeDefinitionFunction function)
        {
            function.VisitChildren(this);
        }

        public virtual void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
        {
            enumType.VisitChildren(this);
        }

        public virtual void VisitTypeDefinitionEnumOption(AstTypeDefinitionEnumOption enumOption)
        {
            enumOption.VisitChildren(this);
        }

        public virtual void VisitTypeReferenceFunction(AstTypeReferenceFunction function)
        {
            function.VisitChildren(this);
        }

        public virtual void VisitTypeFieldReferenceEnumOption(AstTypeFieldReferenceEnumOption enumOption)
        {
            enumOption.VisitChildren(this);
        }

        public virtual void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
        {
            structType.VisitChildren(this);
        }

        public virtual void VisitTypeDefinitionStructField(AstTypeDefinitionStructField structField)
        {
            structField.VisitChildren(this);
        }

        public virtual void VisitTypeFieldReferenceStructField(AstTypeFieldReferenceStructField structField)
        {
            structField.VisitChildren(this);
        }

        public virtual void VisitTemplateInstanceStruct(AstTemplateInstanceStruct structTemplate)
        {
            structTemplate.VisitChildren(this);
        }

        public virtual void VisitTemplateInstanceType(AstTemplateInstanceType typeTemplate)
        {
            typeTemplate.VisitChildren(this);
        }

        public virtual void VisitTemplateParameterDefinition(AstTemplateParameterDefinition templateParameter)
        {
            templateParameter.VisitChildren(this);
        }

        public virtual void VisitTemplateParameterReference(AstTemplateParameterReference templateParameter)
        {
            templateParameter.VisitChildren(this);
        }

        public virtual void VisitGenericParameterDefinition(AstGenericParameterDefinition genericParameter)
        {
            genericParameter.VisitChildren(this);
        }

        public virtual void VisitGenericParameterReference(AstGenericParameterReference genericParameter)
        {
            genericParameter.VisitChildren(this);
        }

        public virtual void VisitVariableDefinition(AstVariableDefinition variable)
        {
            variable.VisitChildren(this);
        }

        public virtual void VisitVariableReference(AstVariableReference variable)
        {
            variable.VisitChildren(this);
        }
    }
}