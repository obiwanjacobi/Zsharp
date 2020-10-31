using System;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionDefinition : AstFunction,
        IAstCodeBlockSite, IAstSymbolTableSite
    {
        private readonly List<AstFunctionParameter> _parameters = new List<AstFunctionParameter>();

        public AstFunctionDefinition(Function_defContext functionCtx)
        {
            Context = functionCtx;
        }

        public Function_defContext Context { get; }

        public IEnumerable<AstFunctionParameter> Parameters => _parameters;

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool TrySetCodeBlock(AstCodeBlock codeBlock)
        {
            if (this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                _codeBlock!.Indent = Indent + 1;
                AddFunctionSymbols();
                return true;
            }
            return false;
        }

        public void SetCodeBlock(AstCodeBlock codeBlock)
        {
            if (!TrySetCodeBlock(codeBlock))
                throw new InvalidOperationException(
                    "CodeBlock is already set or null.");
        }

        public AstSymbolTable Symbols
        {
            get
            {
                var codeBlock = CodeBlock;
                if (codeBlock != null)
                {
                    return codeBlock.Symbols;
                }

                var site = ParentAs<IAstSymbolTableSite>() ??
                    throw new InvalidOperationException("Function Parent not a SymbolTable Site.");
                return site.Symbols;
            }
        }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode node)
        {
            if (Symbols == null)
            {
                throw new InvalidOperationException("SymbolTable not set.");
            }

            return Symbols.AddSymbol(symbolName, kind, node);
        }

        public bool TryAddParameter(AstFunctionParameter param)
        {
            if (param != null &&
                param.TrySetParent(this))
            {
                _parameters.Add(param);
                return true;
            }
            return false;
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionDefinition(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in _parameters)
            {
                param.Accept(visitor);
            }

            CodeBlock?.Accept(visitor);
        }

        /// <summary>
        /// Deferred registration of function parameter symbols in the codeblock's symbol table.
        /// </summary>
        private void AddFunctionSymbols()
        {
            foreach (var param in _parameters)
            {
                // function parameters are registered as variables
                Symbols.AddSymbol(param.Identifier.Name, AstSymbolKind.Variable, param);
            }
        }
    }
}
