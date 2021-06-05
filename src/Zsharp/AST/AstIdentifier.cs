using Antlr4.Runtime;
using System;
using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    [DebuggerDisplay("{Name}")]
    public class AstIdentifier
    {
        private const string TemplateDelimiter = "%";
        private const string ParameterDelimiter = ";";

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
            SymbolName = AstSymbolName.Parse(name);
            IdentifierType = identifierType;
        }

        protected AstIdentifier(ParserRuleContext context, AstIdentifierType identifierType)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            SymbolName = AstSymbolName.Parse(context.GetText());
            IdentifierType = identifierType;
        }

        public ParserRuleContext? Context { get; }

        public virtual bool IsIntrinsic => false;

        public AstSymbolName SymbolName { get; }
        public string Name => SymbolName.FullName;
        // TODO: add canonical instance
        public string CanonicalName => SymbolName.ToCanonical().FullName;

        public AstIdentifierType IdentifierType { get; }

        //public void Append(string? name)
        //{
        //    if (!String.IsNullOrEmpty(name))
        //    {
        //        Name += name;
        //        CanonicalName = AstDotName.ToCanonical(Name);
        //    }
        //}

        // for template definition
        private int _templateParameterCount;
        public int TemplateParameterCount
        {
            get { return _templateParameterCount; }
            set
            {
                _templateParameterCount = value;
                SymbolName.SetTemplateParameterCount(value);
                //var parts = Name.Split(TemplateDelimiter);
                //if (_templateParameterCount > 0)
                //    Name = $"{parts[0]}{TemplateDelimiter}{_templateParameterCount}";
                //else
                //    Name = parts[0];
                //CanonicalName = AstDotName.ToCanonical(Name);
            }
        }

        // 'MyTemplate%1'
        public string TemplateDefinitionName
            => SymbolName.TemplateDefinitionName;
        //{
        //    get
        //    {
        //        if (_templateParameterCount > 0)
        //            return CanonicalName;

        //        if (Name.Contains(ParameterDelimiter))
        //        {
        //            var parts = Name.Split(ParameterDelimiter);
        //            return $"{parts[0]}{TemplateDelimiter}{parts.Length - 1}";
        //        }

        //        return String.Empty;
        //    }
        //}

        // for template usage
        public void AddTemplateParameter(string? name)
        {
            Ast.Guard(name, "Parameter name is null.");

            SymbolName.AddTemplateParameter(name);
            //Name += ParameterDelimiter + name;
            //CanonicalName = AstDotName.ToCanonical(Name);
        }

        public bool IsEqual(AstIdentifier? that)
        {
            if (that is null)
                return false;

            return CanonicalName == that.CanonicalName &&
                IdentifierType == that.IdentifierType;
        }
    }
}