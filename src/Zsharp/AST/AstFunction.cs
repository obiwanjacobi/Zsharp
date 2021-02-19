using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public abstract class AstFunction<ParamT> : AstNode, IAstCodeBlockItem,
        IAstIdentifierSite, IAstTypeReferenceSite, IAstSymbolEntrySite
        where ParamT : AstFunctionParameter
    {
        private readonly List<ParamT> _parameters = new List<ParamT>();

        protected AstFunction()
            : base(AstNodeType.Function)
        { }

        public ParserRuleContext? Context { get; protected set; }

        public uint Indent { get; set; }

        public virtual bool IsIntrinsic => false;

        public IEnumerable<ParamT> Parameters => _parameters;

        public bool TryAddParameter(ParamT param)
        {
            if (param != null &&
                param.TrySetParent(this))
            {
                // always make sure 'self' is first param
                if (param.Identifier == AstIdentifierIntrinsic.Self)
                    _parameters.Insert(0, param);
                else
                    _parameters.Add(param);
                return true;
            }
            return false;
        }

        public void AddParameter(ParamT param)
        {
            if (!TryAddParameter(param))
                throw new InvalidOperationException("Parameter was already set or null.");
        }

        public string OverloadKey =>
            String.Join(String.Empty, _parameters.Select(p => p.TypeReference.Identifier.CanonicalName));

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        public void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set or null.");
        }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference typeReference)
            => this.SafeSetParent(ref _typeRef, typeReference);

        public void SetTypeReference(AstTypeReference typeReference)
        {
            if (!TrySetTypeReference(typeReference))
                throw new InvalidOperationException(
                    "TypeReference is already set or null.");
        }

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
            var entry = Symbol?.SymbolTable.ResolveDefinition(_symbol);
            if (entry != null)
            {
                _symbol = entry;
                return true;
            }
            return false;
        }

        public void CreateSymbols(AstSymbolTable symbols)
        {
            if (TypeReference != null &&
                TypeReference.Symbol == null)
            {
                symbols.Add(TypeReference);
            }
            foreach (var parameter in Parameters)
            {
                if (parameter.TypeReference != null &&
                    parameter.TypeReference.Symbol == null)
                {
                    symbols.Add(parameter.TypeReference);
                }
            }
            if (Symbol == null)
            {
                symbols.Add(this);
            }
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var param in Parameters)
            {
                param.Accept(visitor);
            }

            TypeReference?.Accept(visitor);
        }
    }
}