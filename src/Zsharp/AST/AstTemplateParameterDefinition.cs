using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTemplateParameterDefinition : AstTemplateParameter,
        IAstIdentifierSite, IAstSymbolEntrySite
    {
        public AstTemplateParameterDefinition(Template_param_anyContext context)
            : base(context)
        { }

        protected AstTemplateParameterDefinition()
            : base(null)
        { }

        public virtual bool IsIntrinsic => false;

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set or null.");
        }

        public bool TrySetIdentifier(AstIdentifier identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
        {
            if (!TrySetSymbol(symbolEntry))
                throw new InvalidOperationException(
                    "Symbol is already set or null.");
        }

        public override bool TryResolve()
        {
            // TODO: ??
            return true;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterDefinition(this);
    }
}