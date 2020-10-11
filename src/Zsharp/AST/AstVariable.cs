using static ZsharpParser;

namespace Zsharp.AST
{
    public abstract class AstVariable : AstCodeBlockItem, IAstIdentifierSite
    {
        protected AstVariable()
            : base(AstNodeType.Variable)
        { }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool SetIdentifier(AstIdentifier identifier)
        {
            return this.SafeSetParent(ref _identifier, identifier);
        }
    }

    public class AstVariableDefinition : AstVariable, IAstTypeReferenceSite
    {
        private Variable_def_typedContext? _typedCtx;
        private Variable_def_typed_initContext? _typedInitCtx;
        private Variable_assign_autoContext? _assignCtx;

        public AstVariableDefinition(Variable_def_typedContext ctx)
        {
            _typedCtx = ctx;
        }

        public AstVariableDefinition(Variable_def_typed_initContext ctx)
        {
            _typedInitCtx = ctx;
        }

        public AstVariableDefinition(Variable_assign_autoContext ctx)
        {
            _assignCtx = ctx;
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

    public class AstVariableReference : AstVariable
    {
        private Variable_refContext? _refCtx;
        private Variable_assign_autoContext? _assignCtx;

        public AstVariableReference(Variable_refContext ctx)
        {
            _refCtx = ctx;
        }
        public AstVariableReference(Variable_assign_autoContext ctx)
        {
            _assignCtx = ctx;
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