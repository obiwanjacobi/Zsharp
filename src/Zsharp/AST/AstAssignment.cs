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

        public bool SetExpression(AstExpression expression)
        {
            return this.SafeSetParent(ref _expression, expression);
        }

        private AstVariable? _variable;
        public AstVariable? Variable => _variable;

        public bool SetVariable(AstVariable variable)
        {
            if (this.SafeSetParent(ref _variable, variable))
            {
                variable.Indent = Indent;
                return true;
            }
            return false;
        }

        // override variable reference with definition
        public void SetVariableDefinition(AstVariableDefinition variableDefinition)
        {
            Ast.Guard(_variable is AstVariableReference, "Unexpected Variable on Assign.");

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
            Variable?.Accept(visitor);
            Expression?.Accept(visitor);
        }
    }
}