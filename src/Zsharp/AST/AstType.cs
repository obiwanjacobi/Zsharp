using Antlr4.Runtime;
using System;

namespace Zsharp.AST
{
    // See AstIdentifier.cs
    public partial class AstIdentifierIntrinsic
    {
        public static readonly AstIdentifierIntrinsic U8 = new AstIdentifierIntrinsic("U8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U16 = new AstIdentifierIntrinsic("U16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U64 = new AstIdentifierIntrinsic("U64", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U32 = new AstIdentifierIntrinsic("U32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I8 = new AstIdentifierIntrinsic("I8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I16 = new AstIdentifierIntrinsic("I16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I64 = new AstIdentifierIntrinsic("I64", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I32 = new AstIdentifierIntrinsic("I32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F64 = new AstIdentifierIntrinsic("F64", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F32 = new AstIdentifierIntrinsic("F32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Str = new AstIdentifierIntrinsic("Str", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Bool = new AstIdentifierIntrinsic("Bool", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Void = new AstIdentifierIntrinsic("Void", AstIdentifierType.Type);
    }

    public abstract class AstType : AstNode, IAstIdentifierSite, IAstSymbolEntrySite
    {
        protected AstType()
            : base(AstNodeType.Type)
        { }

        protected AstType(AstIdentifier identifier)
            : base(AstNodeType.Type)
        {
            TrySetIdentifier(identifier);
        }

        public ParserRuleContext? Context { get; protected set; }

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

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry symbolEntry) => Ast.SafeSet(ref _symbol, symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
        {
            if (!TrySetSymbol(symbolEntry))
                throw new InvalidOperationException("Symbol is already set or null.");
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

        public virtual bool IsEqual(AstType type)
        {
            if (type == null)
                return false;
            if (Identifier == null)
                return false;
            if (type.Identifier == null)
                return false;

            return Identifier.IsEqual(type.Identifier);
        }
    }
}