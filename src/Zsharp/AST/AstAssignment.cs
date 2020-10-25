using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstAssignment : AstCodeBlockItem, IAstExpressionSite
    {
        public AstAssignment(Variable_assign_autoContext context)
            : base(AstNodeType.Assignment)
        { }

        public AstAssignment(Variable_def_typed_initContext context)
            : base(AstNodeType.Assignment)
        { }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public bool TrySetExpression(AstExpression expression)
        {
            return this.SafeSetParent(ref _expression, expression);
        }

        public void SetExpression(AstExpression expression)
        {
            if (!TrySetExpression(expression))
                throw new InvalidOperationException(
                    "Expression is already set or null.");
        }

        private AstVariable? _variable;
        public AstVariable? Variable => _variable;

        public bool TrySetVariable(AstVariable variable)
        {
            if (this.SafeSetParent(ref _variable, variable))
            {
                variable.Indent = Indent;
                return true;
            }
            return false;
        }

        public void SetVariable(AstVariable variable)
        {
            if (!TrySetVariable(variable))
                throw new InvalidOperationException(
                    "Variable is already set or null.");
        }

        // override variable reference with definition
        public void SetVariableDefinition(AstVariableDefinition variableDefinition)
        {
            Ast.Guard(_variable is AstVariableReference, "Unexpected Variable Type on Assign.");

            _variable = null;
            if (!this.SafeSetParent(ref _variable, variableDefinition))
            {
                throw new InvalidOperationException(
                    "SetParent failed in AstAssignment.SetVariableDefinition.");
            }
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitAssignment(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            Expression?.Accept(visitor);
            Variable?.Accept(visitor);
        }
    }
}