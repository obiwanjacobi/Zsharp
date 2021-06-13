using Antlr4.Runtime;
using System;
using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Name}")]
    public class AstIdentifier
    {
        public AstIdentifier(Identifier_typeContext context)
            : this(context, AstIdentifierKind.Type)
        { }

        public AstIdentifier(Identifier_varContext context)
            : this(context, AstIdentifierKind.Variable)
        { }

        public AstIdentifier(Identifier_paramContext context)
            : this(context, AstIdentifierKind.Parameter)
        { }

        public AstIdentifier(Identifier_funcContext context)
            : this(context, AstIdentifierKind.Function)
        { }

        public AstIdentifier(Identifier_fieldContext context)
            : this(context, AstIdentifierKind.Field)
        { }

        public AstIdentifier(Identifier_enumoptionContext context)
            : this(context, AstIdentifierKind.EnumOption)
        { }

        public AstIdentifier(Enum_option_useContext context)
            : this(context, AstIdentifierKind.EnumOption)
        { }

        public AstIdentifier(Identifier_template_paramContext context)
            : this(context, AstIdentifierKind.TemplateParameter)
        { }

        public AstIdentifier(string name, AstIdentifierKind identifierKind)
        {
            SymbolName = AstSymbolName.Parse(name, AstSymbolNameParseOptions.IsSource);
            IdentifierKind = identifierKind;
        }

        protected AstIdentifier(ParserRuleContext context, AstIdentifierKind identifierKind)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            SymbolName = AstSymbolName.Parse(context.GetText(), AstSymbolNameParseOptions.IsSource);
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
        public string Name => SymbolName.FullName;
        public string CanonicalName => SymbolName.ToCanonical().FullName;

        public AstIdentifierKind IdentifierKind { get; }

        public bool IsEqual(AstIdentifier? that)
        {
            if (that is null)
                return false;

            return CanonicalName == that.CanonicalName &&
                IdentifierKind == that.IdentifierKind;
        }

        public virtual AstIdentifier MakeCopy()
            => new AstIdentifier(this);
    }
}