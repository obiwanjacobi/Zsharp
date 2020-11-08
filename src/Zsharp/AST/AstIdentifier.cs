using Antlr4.Runtime;
using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Name}")]
    public class AstIdentifier
    {
        private readonly Identifier_typeContext? _typeCtx;
        private readonly Identifier_varContext? _varCtx;
        private readonly Identifier_paramContext? _paramCtx;
        private readonly Identifier_funcContext? _funcCtx;
        private readonly Identifier_fieldContext? _fieldCtx;
        private readonly Identifier_enumoptionContext? _enumOptCtx;

        public AstIdentifier(Identifier_typeContext context)
        {
            Context = context;
            Name = context.GetText();
            IdentifierType = AstIdentifierType.Type;
        }

        public AstIdentifier(Identifier_varContext context)
        {
            Context = context;
            Name = context.GetText();
            IdentifierType = AstIdentifierType.Variable;
        }

        public AstIdentifier(Identifier_paramContext context)
        {
            Context = context;
            Context = context;
            Name = context.GetText();
            IdentifierType = AstIdentifierType.Parameter;
        }

        public AstIdentifier(Identifier_funcContext context)
        {
            Context = context;
            Name = context.GetText();
            IdentifierType = AstIdentifierType.Function;
        }

        public AstIdentifier(Identifier_fieldContext context)
        {
            Context = context;
            Name = context.GetText();
            IdentifierType = AstIdentifierType.Field;
        }

        public AstIdentifier(Identifier_enumoptionContext context)
        {
            Context = context;
            Name = context.GetText();
            IdentifierType = AstIdentifierType.EnumOption;
        }

        protected AstIdentifier(string name, AstIdentifierType identifierType)
        {
            Name = name;
            IdentifierType = identifierType;
        }

        public ParserRuleContext? Context { get; }

        public string Name { get; }

        public AstIdentifierType IdentifierType { get; }

        public bool IsEqual(AstIdentifier? that)
        {
            if (that == null)
                return false;

            return Name == that.Name &&
                IdentifierType == that.IdentifierType;
        }
    }
}