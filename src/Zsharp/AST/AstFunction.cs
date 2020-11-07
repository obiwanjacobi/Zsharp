using System;
using System.Collections.Generic;
using System.Text;

namespace Zsharp.AST
{
    public abstract class AstFunction<ParamT> : AstCodeBlockItem,
        IAstIdentifierSite, IAstTypeReferenceSite, IAstSymbolEntrySite
        where ParamT : AstFunctionParameter
    {
        private readonly List<ParamT> _parameters = new List<ParamT>();

        protected AstFunction()
            : base(AstNodeType.Function)
        { }

        public IEnumerable<ParamT> Parameters => _parameters;

        public bool TryAddParameter(ParamT param)
        {
            if (param != null &&
                param.TrySetParent(this))
            {
                _parameters.Add(param);
                return true;
            }
            return false;
        }

        public string OverloadKey
        {
            get
            {
                var key = new StringBuilder(Identifier.Name);
                foreach (var p in _parameters)
                {
                    key.Append(p.TypeReference.Identifier.Name);
                }
                return key.ToString();
            }
        }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier) => Ast.SafeSet(ref _identifier, identifier);

        public void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set or null.");
        }

        private AstTypeReference? _typeRef;
        public AstTypeReference? TypeReference => _typeRef;

        public bool TrySetTypeReference(AstTypeReference typeReference) => Ast.SafeSet(ref _typeRef, typeReference);

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
            var entry = Symbol?.SymbolTable.Resolve(Symbol);
            if (entry != null)
            {
                _symbol = entry;
                return true;
            }
            return false;
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);
            TypeReference?.Accept(visitor);
        }
    }
}