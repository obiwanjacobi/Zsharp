using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    // See AstIdentifier.cs
    public partial class AstIdentifierIntrinsic
    {
        public static readonly AstIdentifierIntrinsic U8 = new AstIdentifierIntrinsic("U8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U16 = new AstIdentifierIntrinsic("U16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U64 = new AstIdentifierIntrinsic("U64", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U32 = new AstIdentifierIntrinsic("U32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I8 = new AstIdentifierIntrinsic("I8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I16 = new AstIdentifierIntrinsic("I16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I64 = new AstIdentifierIntrinsic("I64", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I32 = new AstIdentifierIntrinsic("I32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F64 = new AstIdentifierIntrinsic("F64", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F32 = new AstIdentifierIntrinsic("F32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Str = new AstIdentifierIntrinsic("Str", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Bool = new AstIdentifierIntrinsic("Bool", AstIdentifierType.Type);
    }

    public abstract class AstType : AstNode, IAstIdentifierSite
    {
        protected AstType()
            : base(AstNodeType.Type)
        { }
        protected AstType(Type_nameContext context)
            : base(AstNodeType.Type)
        {
            Context = context;
        }
        protected AstType(AstIdentifier identifier)
            : base(AstNodeType.Type)
        {
            TrySetIdentifier(identifier);
        }

        public Type_nameContext? Context { get; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;

        public bool TrySetIdentifier(AstIdentifier identifier)
            => Ast.SafeSet(ref _identifier, identifier);

        public void SetIdentifier(AstIdentifier identifier)
        {
            if (!TrySetIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set or null.");
        }

        public virtual bool IsEqual(AstType type)
        {
            if (type == null)
                return false;
            if (Identifier == null)
                return false;
            if (type.Identifier == null)
                return false;

            return Identifier.IsEqual(type.Identifier);
        }

        internal static void Construct(AstType instance, Type_nameContext context)
        {
            AstIdentifier? identifier;

            var idCtx = context.identifier_type();
            if (idCtx != null)
            {
                identifier = new AstIdentifier(idCtx);
                // TODO: type parameters context.type_param_list()
            }
            else
            {
                var knownCtx = context.known_types();
                identifier = SelectKnownIdentifier(knownCtx);
            }

            Ast.Guard(identifier, "Identifier failed.");
            instance.SetIdentifier(identifier!);
        }

        private static AstIdentifier? SelectKnownIdentifier(Known_typesContext context)
        {
            if (context == null)
                return null;
            //if (context.type_Bit()) return Bit;
            //if (context.type_Ptr()) return Ptr;

            AstIdentifier? identifier = null;

            if (context.BOOL() != null)
                identifier = AstIdentifierIntrinsic.Bool;
            if (context.STR() != null)
                identifier = AstIdentifierIntrinsic.Str;
            if (context.F64() != null)
                identifier = AstIdentifierIntrinsic.F64;
            if (context.F32() != null)
                identifier = AstIdentifierIntrinsic.F32;
            if (context.I8() != null)
                identifier = AstIdentifierIntrinsic.I8;
            if (context.I16() != null)
                identifier = AstIdentifierIntrinsic.I16;
            if (context.I64() != null)
                identifier = AstIdentifierIntrinsic.I64;
            if (context.I32() != null)
                identifier = AstIdentifierIntrinsic.I32;
            if (context.U8() != null)
                identifier = AstIdentifierIntrinsic.U8;
            if (context.U16() != null)
                identifier = AstIdentifierIntrinsic.U16;
            if (context.U64() != null)
                identifier = AstIdentifierIntrinsic.U64;
            if (context.U32() != null)
                identifier = AstIdentifierIntrinsic.U32;

            return identifier;
        }
    }
}