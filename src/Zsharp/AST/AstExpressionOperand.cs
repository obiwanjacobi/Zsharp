using Antlr4.Runtime;
using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstExpressionOperand : AstNode, IAstTypeReferenceSite
    {
        private readonly Literal_boolContext? _litBoolCtx;
        private readonly Function_callContext? _callCtx;

        public AstExpressionOperand(AstExpression expr)
            : base(AstNodeType.Operand)
        {
            Expression = expr;
            bool success = expr.SetParent(this);
            Ast.Guard(success, "SetParent failed.");
        }

        public AstExpressionOperand(AstNumeric num)
            : base(AstNodeType.Operand)
        {
            Numeric = num;
            bool success = num.SetParent(this);
            Ast.Guard(success, "SetParent failed.");
        }

        public AstExpressionOperand(AstVariableReference variable)
            : base(AstNodeType.Operand)
        {
            VariableReference = variable;
            bool success = variable.SetParent(this);
            Ast.Guard(success, "SetParent failed.");
        }

        public AstExpressionOperand(Literal_boolContext ctx)
            : base(AstNodeType.Operand)
        {
            _litBoolCtx = ctx;
        }

        public AstExpressionOperand(Function_callContext ctx)
            : base(AstNodeType.Operand)
        {
            _callCtx = ctx;
        }

        public ParserRuleContext? Context
        {
            get
            {
                if (_litBoolCtx != null)
                    return _litBoolCtx;
                if (_callCtx != null)
                    return _callCtx;
                return null;
            }
        }

        public AstExpression? Expression { get; }

        public AstNumeric? Numeric { get; }

        public AstVariableReference? VariableReference { get; }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool SetTypeReference(AstTypeReference typeRef)
        {
            return Ast.SafeSet(ref _typeRef, typeRef);
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitExpressionOperand(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            Expression?.Accept(visitor);
            Numeric?.Accept(visitor);
            VariableReference?.Accept(visitor);
        }
    }
}