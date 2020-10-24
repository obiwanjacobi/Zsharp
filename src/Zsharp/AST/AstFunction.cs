using System;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunction : AstCodeBlockItem,
        IAstCodeBlockSite, IAstIdentifierSite, IAstSymbolTableSite,
        IAstTypeReferenceSite, IAstSymbolEntrySite
    {
        private readonly List<AstFunctionParameter> _parameters = new List<AstFunctionParameter>();

        public AstFunction(Function_defContext functionCtx)
            : base(AstNodeType.Function)
        {
            Context = functionCtx;
        }

        public Function_defContext Context { get; }

        public IEnumerable<AstFunctionParameter> Parameters => _parameters;

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier) => this.SafeSetParent(ref _identifier, identifier);

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference typeReference) => Ast.SafeSet(ref _typeRef, typeReference);

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

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry symbolEntry) => Ast.SafeSet(ref _symbol, symbolEntry);

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

        public override void Accept(AstVisitor visitor) => visitor.VisitFunction(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            Identifier?.Accept(visitor);

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
                var identifier = param.Identifier;
                if (identifier != null)
                {
                    identifier.AddSymbol();
                }
            }
        }
    }
}