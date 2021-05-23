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

        public ParserRuleContext? Context { get; }

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);
    }
}