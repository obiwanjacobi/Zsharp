using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableDefinition : AstVariable
    {
        private readonly Variable_def_typedContext? _typedCtx;
        private readonly Variable_def_typed_initContext? _typedInitCtx;

        public AstVariableDefinition(Variable_def_typedContext context)
        {
            _typedCtx = context;
        }

        public AstVariableDefinition(Variable_def_typed_initContext context)
        {
            _typedInitCtx = context;
        }

        public AstVariableDefinition(AstTypeReference? typeReference)
            : base(typeReference)
        { }

        public override void Accept(AstVisitor visitor)
        {
            if (ParentAs<AstCodeBlock>() != null)
            {
                base.Accept(visitor);
            }
            visitor.VisitVariableDefinition(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            Identifier?.Accept(visitor);
            TypeReference?.Accept(visitor);
        }
    }
}