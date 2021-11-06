using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTemplateParameter : AstNode,
        IAstSymbolSite
    {
        protected AstTemplateParameter()
            : base(AstNodeKind.TemplateParameter)
        { }

        protected AstTemplateParameter(ParserRuleContext context)
            : base(AstNodeKind.TemplateParameter)
        {
            Context = context;
        }

        protected AstTemplateParameter(AstTemplateParameter parameterToCopy)
            : base(AstNodeKind.TemplateParameter)
        {
            Context = parameterToCopy.Context;
            if (parameterToCopy.HasSymbol)
                _symbol = parameterToCopy.Symbol;
        }

        public ParserRuleContext? Context { get; }

        public bool HasSymbol => _symbol is not null;

        private AstSymbol? _symbol;
        public AstSymbol Symbol => _symbol ?? throw new InternalErrorException("Symbol was not set.");

        public bool TrySetSymbol(AstSymbol? symbol)
            => Ast.SafeSet(ref _symbol, symbol);
    }
}