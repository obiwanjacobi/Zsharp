using Antlr4.Runtime;
using System;
using System.Diagnostics;

namespace Zsharp.AST
{
    [DebuggerDisplay("{NativeFullName}")]
    public class AstIdentifier
    {
        public AstIdentifier(string symbolName, AstIdentifierKind identifierKind)
            : this(AstName.ParseFullName(symbolName), identifierKind)
        { }

        public AstIdentifier(AstName symbolName, AstIdentifierKind identifierKind)
            : this(new AstSymbolName(symbolName), identifierKind)
        { }

        public AstIdentifier(AstSymbolName symbolName, AstIdentifierKind identifierKind)
        {
            SymbolName = symbolName;
            IdentifierKind = identifierKind;
        }

        internal AstIdentifier(ParserRuleContext context, AstIdentifierKind identifierKind)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            SymbolName = AstSymbolName.Parse(context.GetText());
            IdentifierKind = identifierKind;
        }

        protected AstIdentifier(AstIdentifier identifierToCopy)
        {
            Context = identifierToCopy.Context;
            IdentifierKind = identifierToCopy.IdentifierKind;
            SymbolName = new AstSymbolName(identifierToCopy.SymbolName);
        }

        public ParserRuleContext? Context { get; }

        public virtual bool IsIntrinsic => false;

        public AstSymbolName SymbolName { get; internal set; }
        public string NativeFullName => SymbolName.NativeName.FullName;
        public string CanonicalFullName => SymbolName.CanonicalName.FullName;

        public AstIdentifierKind IdentifierKind { get; }

        public bool IsEqual(AstIdentifier? that)
        {
            return SymbolName.CanonicalName.FullName == that?.SymbolName.CanonicalName.FullName &&
                IdentifierKind == that?.IdentifierKind;
        }

        public virtual AstIdentifier MakeCopy()
            => new(this);
    }
}