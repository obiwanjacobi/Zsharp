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
            VisitChildren(assign);
        }

        public virtual void VisitBranch(AstBranch branch)
        {
            VisitChildren(branch);
        }

        public virtual void VisitBranchExpression(AstBranchExpression branch)
        {
            VisitChildren(branch);
        }

        public virtual void VisitBranchConditional(AstBranchConditional branch)
        {
            VisitChildren(branch);
        }

        public virtual void VisitCodeBlock(AstCodeBlock codeBlock)
        {
            VisitChildren(codeBlock);
        }

        public virtual void VisitExpression(AstExpression expression)
        {
            VisitChildren(expression);
        }

        public virtual void VisitExpressionOperand(AstExpressionOperand operand)
        {
            VisitChildren(operand);
        }

        public virtual void VisitFile(AstFile file)
        {
            VisitChildren(file);
        }

        public virtual void VisitFunctionDefinition(AstFunctionDefinition function)
        {
            VisitChildren(function);
        }

        public virtual void VisitFunctionReference(AstFunctionReference function)
        {
            VisitChildren(function);
        }

        public virtual void VisitFunctionParameterDefinition(AstFunctionParameterDefinition parameter)
        {
            VisitChildren(parameter);
        }

        public virtual void VisitFunctionParameterReference(AstFunctionParameterReference parameter)
        {
            VisitChildren(parameter);
        }

        public virtual void VisitModuleExternal(AstModuleExternal module)
        {
            VisitChildren(module);
        }

        public virtual void VisitModulePublic(AstModulePublic module)
        {
            VisitChildren(module);
        }

        public virtual void VisitLiteralNumeric(AstLiteralNumeric numeric)
        {
            VisitChildren(numeric);
        }

        public virtual void VisitLiteralBoolean(AstLiteralBoolean literalBool)
        {
            VisitChildren(literalBool);
        }

        public virtual void VisitLiteralString(AstLiteralString literalString)
        {
            VisitChildren(literalString);
        }

        public virtual void VisitTemplateInstanceFunction(AstTemplateInstanceFunction templateFunction)
        {
            VisitChildren(templateFunction);
        }

        public virtual void VisitTypeFieldDefinition(AstTypeFieldDefinition field)
        {
            VisitChildren(field);
        }

        public virtual void VisitTypeFieldInitialization(AstTypeFieldInitialization field)
        {
            VisitChildren(field);
        }

        public virtual void VisitTypeReference(AstTypeReference type)
        {
            VisitChildren(type);
        }

        public virtual void VisitTypeDefinitionFunction(AstTypeDefinitionFunction function)
        {
            VisitChildren(function);
        }

        public virtual void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
        {
            VisitChildren(enumType);
        }

        public virtual void VisitTypeDefinitionEnumOption(AstTypeDefinitionEnumOption enumOption)
        {
            VisitChildren(enumOption);
        }

        public virtual void VisitTypeFieldReferenceEnumOption(AstTypeFieldReferenceEnumOption enumOption)
        {
            VisitChildren(enumOption);
        }

        public virtual void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
        {
            VisitChildren(structType);
        }

        public virtual void VisitTypeDefinitionStructField(AstTypeDefinitionStructField structField)
        {
            VisitChildren(structField);
        }

        public virtual void VisitTypeFieldReferenceStructField(AstTypeFieldReferenceStructField structField)
        {
            VisitChildren(structField);
        }

        public virtual void VisitTemplateInstanceStruct(AstTemplateInstanceStruct structTemplate)
        {
            VisitChildren(structTemplate);
        }

        public virtual void VisitTemplateInstanceType(AstTemplateInstanceType typeTemplate)
        {
            VisitChildren(typeTemplate);
        }

        public virtual void VisitTemplateParameterDefinition(AstTemplateParameterDefinition templateParameter)
        {
            VisitChildren(templateParameter);
        }

        public virtual void VisitTemplateParameterReference(AstTemplateParameterReference templateParameter)
        {
            VisitChildren(templateParameter);
        }

        public virtual void VisitVariableDefinition(AstVariableDefinition variable)
        {
            VisitChildren(variable);
        }

        public virtual void VisitVariableReference(AstVariableReference variable)
        {
            VisitChildren(variable);
        }

        protected void VisitChildren(AstNode node)
        {
            node.VisitChildren(this);
        }
    }
}