using System;
using static ZsharpParser;

namespace Zlang.NET.AST
{
    public interface IAstIdentifierSite
    {
        AstIdentifier? Identifier { get; }
        bool SetIdentifier(AstIdentifier identifier);
    }

    public enum AstIdentifierType
    {
        Unknown,
        Type,
        Variable,
        Parameter,
        Function,
        Field,
        EnumOption
    }

    public class AstIdentifier : AstNode
    {
        private Identifier_typeContext? _typeCtx;
        private Identifier_varContext? _varCtx;
        private Identifier_paramContext? _paramCtx;
        private Identifier_funcContext? _funcCtx;
        private Identifier_fieldContext? _fieldCtx;
        private Identifier_enumoptionContext? _enumOptCtx;

        public AstIdentifier(Identifier_typeContext ctx)
        : base(AstNodeType.Identifier)
        {
            _typeCtx = ctx;
        }
        public AstIdentifier(Identifier_varContext ctx)
            : base(AstNodeType.Identifier)
        {
            _varCtx = ctx;
        }
        public AstIdentifier(Identifier_paramContext ctx)
            : base(AstNodeType.Identifier)
        {
            _paramCtx = ctx;
        }
        public AstIdentifier(Identifier_funcContext ctx)
            : base(AstNodeType.Identifier)
        {
            _funcCtx = ctx;
        }
        public AstIdentifier(Identifier_fieldContext ctx)
            : base(AstNodeType.Identifier)
        {
            _fieldCtx = ctx;
        }
        public AstIdentifier(Identifier_enumoptionContext ctx)
            : base(AstNodeType.Identifier)
        {
            _enumOptCtx = ctx;
        }
        protected AstIdentifier()
            : base(AstNodeType.Identifier)
        { }

        public virtual string Name
        {
            get
            {
                if (_typeCtx != null) return _typeCtx.GetText();
                if (_varCtx != null) return _varCtx.GetText();
                if (_paramCtx != null) return _paramCtx.GetText();
                if (_funcCtx != null) return _funcCtx.GetText();
                if (_fieldCtx != null) return _fieldCtx.GetText();
                if (_enumOptCtx != null) return _enumOptCtx.GetText();
                return String.Empty;
            }
        }

        public virtual AstIdentifierType IdentifierType
        {
            get
            {
                if (_typeCtx != null) return AstIdentifierType.Type;
                if (_varCtx != null) return AstIdentifierType.Variable;
                if (_paramCtx != null) return AstIdentifierType.Parameter;
                if (_funcCtx != null) return AstIdentifierType.Function;
                if (_fieldCtx != null) return AstIdentifierType.Field;
                if (_enumOptCtx != null) return AstIdentifierType.EnumOption;
                return AstIdentifierType.Unknown;
            }
        }

        public bool IsEqual(AstIdentifier that)
        {
            if (that == null) return false;
            return Name == that.Name &&
                IdentifierType == that.IdentifierType;
        }

        public virtual AstIdentifier Clone()
        {
            var identifier = new AstIdentifier();
            CopyTo(identifier);
            return identifier;
        }

        protected void CopyTo(AstIdentifier target)
        {
            target._enumOptCtx = _enumOptCtx;
            target._fieldCtx = _fieldCtx;
            target._funcCtx = _funcCtx;
            target._paramCtx = _paramCtx;
            target._typeCtx = _typeCtx;
            target._varCtx = _varCtx;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitIdentifier(this);
        }
    }

    public static class AstIdentifierExtensions
    {
        public static AstSymbolEntry AddSymbol(this AstIdentifier identifier)
        {
            var symbols = identifier.GetParentRecursive<IAstSymbolTableSite>() ??
                throw new InvalidOperationException("No SymbolTable Site could be found.");

            return symbols.AddSymbol(identifier.Name,
                ToSymbolKind(identifier.IdentifierType), identifier);
        }

        private static AstSymbolKind ToSymbolKind(AstIdentifierType idType)
        {
            switch (idType)
            {
                case AstIdentifierType.Variable:
                    return AstSymbolKind.Variable;
                case AstIdentifierType.EnumOption:
                case AstIdentifierType.Field:
                    return AstSymbolKind.Field;
                case AstIdentifierType.Function:
                    return AstSymbolKind.Function;
                case AstIdentifierType.Parameter:
                    return AstSymbolKind.Parameter;
                case AstIdentifierType.Type:
                    return AstSymbolKind.Type;
                default:
                    return AstSymbolKind.NotSet;
            }
        }
    }

    public partial class AstIdentifierIntrinsic : AstIdentifier
    {
        public AstIdentifierIntrinsic(string name, AstIdentifierType identifierType)
        { 
            Name = name;
            IdentifierType = identifierType;
        }

        public override string Name { get; }
        public override AstIdentifierType IdentifierType {get;}

        public override AstIdentifier Clone()
        {
            return new AstIdentifierIntrinsic(Name, IdentifierType);
        }
    }
}