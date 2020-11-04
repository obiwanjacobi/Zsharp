namespace Zsharp.AST
{
    public class AstVisitor
    {
        public virtual void Visit(AstNode node)
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

        public virtual void VisitFunctionParameter(AstFunctionParameter parameter)
        {
            VisitChildren(parameter);
        }

        public virtual void VisitModulePublic(AstModulePublic module)
        {
            VisitChildren(module);
        }

        public virtual void VisitNumeric(AstNumeric numeric)
        {
            VisitChildren(numeric);
        }

        public virtual void VisitTypeReference(AstTypeReference type)
        {
            VisitChildren(type);
        }

        public virtual void VisitTypeDefinition(AstTypeDefinition type)
        {
            VisitChildren(type);
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