using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstVariableDefinition : AstVariable, IAstTypeReferenceSite
    {
        private readonly Variable_def_typedContext? _typedCtx;
        private readonly Variable_def_typed_initContext? _typedInitCtx;
        private readonly Variable_assign_autoContext? _assignCtx;

        public AstVariableDefinition(Variable_def_typedContext context)
        {
            _typedCtx = context;
        }

        public AstVariableDefinition(Variable_def_typed_initContext context)
        {
            _typedInitCtx = context;
        }

        public AstVariableDefinition(Variable_assign_autoContext context)
        {
            _assignCtx = context;
        }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool SetTypeReference(AstTypeReference typeReference)
        {
            return this.SafeSetParent(ref _typeRef, typeReference);
        }

        public override void Accept(AstVisitor visitor)
        {
            if (GetParent<AstCodeBlock>() != null)
                base.Accept(visitor);
            visitor.VisitVariableDefinition(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            Identifier?.Accept(visitor);
            TypeReference?.Accept(visitor);
        }
    }
}