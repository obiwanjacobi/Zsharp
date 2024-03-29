﻿using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTemplateParameterDefinition : AstTemplateParameter,
        IAstIdentifierSite
    {
        internal AstTemplateParameterDefinition(ParserRuleContext context)
            : base(context)
        { }

        public AstTemplateParameterDefinition(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
        }

        protected AstTemplateParameterDefinition()
        { }

        public virtual bool IsIntrinsic => false;

        public bool HasIdentifier => _identifier != null;

        private AstIdentifier? _identifier;
        public AstIdentifier Identifier
            => _identifier ?? throw new InternalErrorException("No Identifier was set.");

        public bool TrySetIdentifier(AstIdentifier identifier)
        {
            Ast.Guard(identifier.IdentifierKind == AstIdentifierKind.TemplateParameter, "Identifier must be of kind TemplateParameter");
            return Ast.SafeSet(ref _identifier, identifier);
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterDefinition(this);
    }
}