﻿using Antlr4.Runtime;
using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionParameterDefinition : AstFunctionParameter, IAstSymbolEntrySite
    {
        public AstFunctionParameterDefinition(Function_parameterContext context)
        {
            Context = context;
        }

        public AstFunctionParameterDefinition(Function_parameter_selfContext context)
        {
            Context = context;
        }

        public AstFunctionParameterDefinition(AstIdentifier identifier)
        {
            SetIdentifier(identifier);
        }

        public ParserRuleContext? Context { get; }

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionParameterDefinition(this);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry symbolEntry) => Ast.SafeSet(ref _symbol, symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
        {
            if (!TrySetSymbol(symbolEntry))
                throw new InvalidOperationException(
                    "SymbolEntry is already set or null.");
        }

        public bool TryResolve()
        {
            var entry = Symbol?.SymbolTable.Resolve(Symbol);
            if (entry != null)
            {
                _symbol = entry;
                return true;
            }
            return false;
        }
    }
}