﻿using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstGenericParameter : AstNode,
        IAstSymbolEntrySite
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

        private AstSymbol? _symbol;
        public AstSymbol? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbol? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);
    }
}