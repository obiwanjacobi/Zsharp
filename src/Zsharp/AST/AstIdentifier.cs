using System;
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
            _typeCtx = context;
        }

        public AstIdentifier(Identifier_varContext context)
        {
            _varCtx = context;
        }

        public AstIdentifier(Identifier_paramContext context)
        {
            _paramCtx = context;
        }

        public AstIdentifier(Identifier_funcContext context)
        {
            _funcCtx = context;
        }

        public AstIdentifier(Identifier_fieldContext context)
        {
            _fieldCtx = context;
        }

        public AstIdentifier(Identifier_enumoptionContext context)
        {
            _enumOptCtx = context;
        }

        protected AstIdentifier()
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

        public bool IsEqual(AstIdentifier? that)
        {
            if (that == null)
                return false;

            return Name == that.Name &&
                IdentifierType == that.IdentifierType;
        }
    }
}