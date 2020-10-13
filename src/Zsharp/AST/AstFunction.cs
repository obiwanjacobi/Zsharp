using System;
using System.Collections.Generic;
using static ZsharpParser;

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

        public bool SetIdentifier(AstIdentifier identifier)
        {
            return this.SafeSetParent(ref _identifier, identifier);
        }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool SetTypeReference(AstTypeReference typeRef)
        {
            return Ast.SafeSet(ref _typeRef, typeRef);
        }

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock { get { return _codeBlock; } }

        public bool SetCodeBlock(AstCodeBlock codeBlock)
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

        public bool SetSymbol(AstSymbolEntry symbolEntry)
        {
            return Ast.SafeSet(ref _symbol, symbolEntry);
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

                return GetParent<IAstSymbolTableSite>()!.Symbols;
            }
        }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode node)
        {
            if (Symbols == null)
            {
                throw new InvalidOperationException("SymbolTable not attached.");
            }

            return Symbols.AddSymbol(symbolName, kind, node);
        }

        public bool AddParameter(AstFunctionParameter param)
        {
            if (param != null)
            {
                bool success = param.SetParent(this);
                Ast.Guard(success, "SetParent failed.");
                _parameters.Add(param);
                return true;
            }
            return false;
        }

        public override void Accept(AstVisitor visitor)
        {
            base.Accept(visitor);
            visitor.VisitFunction(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            if (Identifier != null)
            {
                Identifier.Accept(visitor);
            }

            foreach (var param in _parameters)
            {
                param.Accept(visitor);
            }

            if (CodeBlock != null)
            {
                CodeBlock.Accept(visitor);
            }
        }

        private void AddFunctionSymbols()
        {
            // deferred registration of function parameter symbols in the codeblock's symbol table
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