using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTemplateParameter : AstNode,
        IAstSymbolEntrySite
    {
        protected AstTemplateParameter()
            : base(AstNodeType.TemplateParameter)
        { }

        protected AstTemplateParameter(ParserRuleContext context)
            : base(AstNodeType.TemplateParameter)
        {
            Context = context;
        }

        protected AstTemplateParameter(AstTemplateParameter parameterToCopy)
            : base(AstNodeType.TemplateParameter)
        {
            Context = parameterToCopy.Context;
            _symbol = parameterToCopy.Symbol;
        }

        public ParserRuleContext? Context { get; }

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);
    }
}