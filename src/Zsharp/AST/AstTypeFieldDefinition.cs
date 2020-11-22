using Antlr4.Runtime;
using System;

namespace Zsharp.AST
{
    public class AstTypeFieldDefinition : AstNode, IAstIdentifierSite
    {
        public AstTypeFieldDefinition()
            : base(AstNodeType.Field)
        { }

        protected AstTypeFieldDefinition(AstNodeType nodeType)
            : base(nodeType)
        { }

        public ParserRuleContext? Context { get; protected set; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier already set or null.");
        }

        public bool TrySetIdentifier(AstIdentifier identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        public override void Accept(AstVisitor visitor)
        {
            // TODO
        }
    }
}
