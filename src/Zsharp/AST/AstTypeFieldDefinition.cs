using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTypeFieldDefinition : AstNode,
        IAstIdentifierSite
    {
        protected AstTypeFieldDefinition(AstNodeType nodeType = AstNodeType.Field)
            : base(nodeType)
        { }

        public ParserRuleContext? Context { get; protected set; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier? identifier)
            => Ast.SafeSet(ref _identifier, identifier);
    }
}
