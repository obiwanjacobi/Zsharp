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
            _symbol = parameterToCopy.Symbol;
        }

        public ParserRuleContext? Context { get; }

        private AstSymbol? _symbol;
        public AstSymbol? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbol? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);
    }
}