using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstIdentifier : AstNode
    {
        private Identifier_typeContext? _typeCtx;
        private Identifier_varContext? _varCtx;
        private Identifier_paramContext? _paramCtx;
        private Identifier_funcContext? _funcCtx;
        private Identifier_fieldContext? _fieldCtx;
        private Identifier_enumoptionContext? _enumOptCtx;

        public AstIdentifier(Identifier_typeContext context)
        : base(AstNodeType.Identifier)
        {
            _typeCtx = context;
        }

        public AstIdentifier(Identifier_varContext context)
            : base(AstNodeType.Identifier)
        {
            _varCtx = context;
        }

        public AstIdentifier(Identifier_paramContext context)
            : base(AstNodeType.Identifier)
        {
            _paramCtx = context;
        }

        public AstIdentifier(Identifier_funcContext context)
            : base(AstNodeType.Identifier)
        {
            _funcCtx = context;
        }

        public AstIdentifier(Identifier_fieldContext context)
            : base(AstNodeType.Identifier)
        {
            _fieldCtx = context;
        }

        public AstIdentifier(Identifier_enumoptionContext context)
            : base(AstNodeType.Identifier)
        {
            _enumOptCtx = context;
        }

        protected AstIdentifier()
            : base(AstNodeType.Identifier)
        { }

        public virtual string Name
        {
            get
            {
                if (_typeCtx != null)
                    return _typeCtx.GetText();
                if (_varCtx != null)
                    return _varCtx.GetText();
                if (_paramCtx != null)
                    return _paramCtx.GetText();
                if (_funcCtx != null)
                    return _funcCtx.GetText();
                if (_fieldCtx != null)
                    return _fieldCtx.GetText();
                if (_enumOptCtx != null)
                    return _enumOptCtx.GetText();
                return String.Empty;
            }
        }

        public virtual AstIdentifierType IdentifierType
        {
            get
            {
                if (_typeCtx != null)
                    return AstIdentifierType.Type;
                if (_varCtx != null)
                    return AstIdentifierType.Variable;
                if (_paramCtx != null)
                    return AstIdentifierType.Parameter;
                if (_funcCtx != null)
                    return AstIdentifierType.Function;
                if (_fieldCtx != null)
                    return AstIdentifierType.Field;
                if (_enumOptCtx != null)
                    return AstIdentifierType.EnumOption;
                return AstIdentifierType.Unknown;
            }
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitIdentifier(this);
        }

        public bool IsEqual(AstIdentifier that)
        {
            if (that == null)
                return false;
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
    }
}