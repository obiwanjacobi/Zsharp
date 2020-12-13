using Antlr4.Runtime;
using System;
using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Name}")]
    public class AstIdentifier
    {
        private const string Delimiter = "%";

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

        public AstIdentifier(Identifier_template_paramContext context)
            : this(context, AstIdentifierType.TemplateParameter)
        { }

        protected AstIdentifier(ParserRuleContext context, AstIdentifierType identifierType)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Name = context.GetText();
            CanonicalName = AstDotName.ToCanonical(Name);
            IdentifierType = identifierType;
        }

        protected AstIdentifier(string name, AstIdentifierType identifierType)
        {
            Name = name;
            CanonicalName = AstDotName.ToCanonical(Name);
            IdentifierType = identifierType;
        }

        public ParserRuleContext? Context { get; }

        public string Name { get; private set; }
        public string CanonicalName { get; private set; }
        public AstIdentifierType IdentifierType { get; }

        private int _templateParameterCount;
        public int TemplateParameterCount
        {
            get { return _templateParameterCount; }
            set
            {
                _templateParameterCount = value;
                var parts = Name.Split(Delimiter);
                if (_templateParameterCount > 0)
                    Name = $"{parts[0]}{Delimiter}{_templateParameterCount}";
                else
                    Name = parts[0];
                CanonicalName = AstDotName.ToCanonical(Name);
            }
        }

        public bool IsEqual(AstIdentifier? that)
        {
            if (that == null)
                return false;

            return CanonicalName == that.CanonicalName &&
                IdentifierType == that.IdentifierType;
        }
    }
}