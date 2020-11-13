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

        protected AstIdentifier(ParserRuleContext context, AstIdentifierType identifierType)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Name = context.GetText();
            CanonicalName = ToCanonical(Name);
            IdentifierType = identifierType;
        }

        protected AstIdentifier(string name, AstIdentifierType identifierType)
        {
            Name = name;
            CanonicalName = ToCanonical(Name);
            IdentifierType = identifierType;
        }

        public ParserRuleContext? Context { get; }

        public string Name { get; }
        public string CanonicalName { get; }

        public AstIdentifierType IdentifierType { get; }

        public bool IsEqual(AstIdentifier? that)
        {
            if (that == null)
                return false;

            return CanonicalName == that.CanonicalName &&
                IdentifierType == that.IdentifierType;
        }

        public bool IsNameMatch(string name)
        {
            return IsNameMatch(Name, name);
        }

        public static bool IsNameMatch(string name1, string name2)
        {
            return ToCanonical(name1) == ToCanonical(name2);
        }

        private static string ToCanonical(string symbolName)
        {
            var simplified = symbolName.Replace("_", String.Empty);
            return simplified[0] + simplified[1..].ToLowerInvariant();
        }
    }
}