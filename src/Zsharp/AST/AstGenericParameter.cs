using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstGenericParameter : AstNode,
        IAstSymbolSite
    {
        protected AstGenericParameter()
            : base(AstNodeKind.GenericParameter)
        { }

        protected AstGenericParameter(ParserRuleContext context)
            : base(AstNodeKind.GenericParameter)
        {
            Context = context;
        }

        protected AstGenericParameter(AstGenericParameter parameterToCopy)
            : base(AstNodeKind.GenericParameter)
        {
            Context = parameterToCopy.Context;
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