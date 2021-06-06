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
            : this(context, AstIdentifierType.Type)
        { }

        public AstIdentifier(Identifier_varContext context)
            : this(context, AstIdentifierType.Variable)
        { }

        public AstIdentifier(Identifier_paramContext context)
            : this(context, AstIdentifierType.Parameter)
        { }

        public AstIdentifier(Identifier_funcContext context)
            : this(context, AstIdentifierType.Function)
        { }

        public AstIdentifier(Identifier_fieldContext context)
            : this(context, AstIdentifierType.Field)
        { }

        public AstIdentifier(Identifier_enumoptionContext context)
            : this(context, AstIdentifierType.EnumOption)
        { }

        public AstIdentifier(Enum_option_useContext context)
            : this(context, AstIdentifierType.EnumOption)
        { }

        public AstIdentifier(Identifier_template_paramContext context)
            : this(context, AstIdentifierType.TemplateParameter)
        { }

        public AstIdentifier(string name, AstIdentifierType identifierType)
        {
            SymbolName = AstSymbolName.Parse(name, AstSymbolNameParseOptions.IsSource);
            IdentifierType = identifierType;
        }

        protected AstIdentifier(ParserRuleContext context, AstIdentifierType identifierType)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            SymbolName = AstSymbolName.Parse(context.GetText(), AstSymbolNameParseOptions.IsSource);
            IdentifierType = identifierType;
        }

        public ParserRuleContext? Context { get; }

        public virtual bool IsIntrinsic => false;

        public AstSymbolName SymbolName { get; internal set; }
        public string Name => SymbolName.FullName;
        public string CanonicalName => SymbolName.ToCanonical().FullName;

        public AstIdentifierType IdentifierType { get; }

        public bool IsEqual(AstIdentifier? that)
        {
            if (that is null)
                return false;

            return CanonicalName == that.CanonicalName &&
                IdentifierType == that.IdentifierType;
        }
    }
}