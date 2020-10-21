using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableReference : AstVariable
    {
        private readonly Variable_refContext? _refCtx;
        private readonly Variable_assign_autoContext? _assignCtx;

        public AstVariableReference(Variable_refContext context)
        {
            _refCtx = context;
        }
        public AstVariableReference(Variable_assign_autoContext context)
        {
            _assignCtx = context;
        }

        private AstVariableDefinition? _varDef;
        public AstVariableDefinition? VariableDefinition => _varDef;

        public bool SetVariableDefinition(AstVariableDefinition variableDefinition)
        {
            if (Ast.SafeSet(ref _varDef, variableDefinition))
            {
                // auto/inferred definitions are owned by the first reference.
                variableDefinition.SetParent(this);
                return true;
            }
            return false;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitVariableReference(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            Identifier?.Accept(visitor);
        }
    }
}