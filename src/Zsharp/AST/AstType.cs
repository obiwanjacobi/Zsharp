using static ZsharpParser;

namespace Zsharp.AST
{
    // See AstIdentifier.cs
    public partial class AstIdentifierIntrinsic
    {
        public static readonly AstIdentifierIntrinsic U8 = new AstIdentifierIntrinsic("U8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U16 = new AstIdentifierIntrinsic("U16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U24 = new AstIdentifierIntrinsic("U24", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U32 = new AstIdentifierIntrinsic("U32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I8 = new AstIdentifierIntrinsic("I8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I16 = new AstIdentifierIntrinsic("I16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I24 = new AstIdentifierIntrinsic("I24", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I32 = new AstIdentifierIntrinsic("I32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F16 = new AstIdentifierIntrinsic("F16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F32 = new AstIdentifierIntrinsic("F32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Str = new AstIdentifierIntrinsic("Str", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Bool = new AstIdentifierIntrinsic("Bool", AstIdentifierType.Type);
    }

    public abstract class AstType : AstNode, IAstIdentifierSite
    {
        protected AstType()
            : base(AstNodeType.Type)
        { }
        protected AstType(Type_nameContext ctx)
            : base(AstNodeType.Type)
        {
            Context = ctx;
        }
        protected AstType(AstIdentifier identifier)
            : base(AstNodeType.Type)
        {
            SetIdentifier(identifier);
        }

        public Type_nameContext? Context { get; }

        private AstIdentifier? _identifier;
        public AstIdentifier? Identifier => _identifier;
        public bool SetIdentifier(AstIdentifier identifier)
        {
            return this.SafeSetParent(ref _identifier, identifier);
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

        public static void Construct(AstType instance, Type_nameContext ctx)
        {
            AstIdentifier? identifier;

            var idCtx = ctx.identifier_type();
            if (idCtx != null)
            {
                identifier = new AstIdentifier(idCtx);
                // TODO: type parameters ctx.type_param_list()
            }
            else
            {
                var knownCtx = ctx.known_types();
                identifier = SelectKnownIdentifier(knownCtx);
            }

            Ast.Guard(identifier, "Identifier failed.");
            bool success = instance.SetIdentifier(identifier!);
            Ast.Guard(success, "SetIdentifier() failed");
        }

        private static AstIdentifier? SelectKnownIdentifier(Known_typesContext ctx)
        {
            if (ctx == null)
                return null;
            //if (ctx.type_Bit()) return Bit;
            //if (ctx.type_Ptr()) return Ptr;

            AstIdentifier? identifier = null;

            if (ctx.BOOL() != null)
                identifier = AstIdentifierIntrinsic.Bool;
            if (ctx.STR() != null)
                identifier = AstIdentifierIntrinsic.Str;
            if (ctx.F16() != null)
                identifier = AstIdentifierIntrinsic.F16;
            if (ctx.F32() != null)
                identifier = AstIdentifierIntrinsic.F32;
            if (ctx.I8() != null)
                identifier = AstIdentifierIntrinsic.I8;
            if (ctx.I16() != null)
                identifier = AstIdentifierIntrinsic.I16;
            // if (ctx.I24() != null)
            //     identifier = AstIdentifierIntrinsic.I24;
            if (ctx.I32() != null)
                identifier = AstIdentifierIntrinsic.I32;
            if (ctx.U8() != null)
                identifier = AstIdentifierIntrinsic.U8;
            if (ctx.U16() != null)
                identifier = AstIdentifierIntrinsic.U16;
            // if (ctx.U24() != null)
            //     identifier = AstIdentifierIntrinsic.U24;
            if (ctx.U32() != null)
                identifier = AstIdentifierIntrinsic.U32;

            if (identifier != null)
            {
                return identifier.Clone();
            }

            return null;
        }
    }
}