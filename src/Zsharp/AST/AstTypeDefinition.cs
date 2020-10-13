using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinition : AstType
    {
        public AstTypeDefinition(Type_defContext ctx)
            : base(ctx.type_ref_use().type_ref().type_name())
        {
            Context = ctx;
        }
        protected AstTypeDefinition(AstIdentifier identifier)
            : base(identifier)
        { }

        public new Type_defContext? Context { get; }

        public AstTypeReference? BaseType { get; internal set; }

        public static AstTypeDefinition Create(Type_defContext ctx)
        {
            var typeDef = new AstTypeDefinition(ctx)
            {
                BaseType = AstTypeReference.Create(ctx.type_ref_use())
            };

            var identifier = new AstIdentifier(ctx.identifier_type());
            typeDef.SetIdentifier(identifier);

            // TODO: type parameters: ctx.type_param_list()
            return typeDef;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitTypeDefinition(this);
        }

        public static AstTypeDefinition? SelectKnownTypeDefinition(Known_typesContext ctx)
        {
            if (ctx == null)
                return null;
            //if (ctx.type_Bit()) return TypeBit;
            //if (ctx.type_Ptr()) return TypePtr;

            if (ctx.BOOL() != null)
                return AstTypeIntrinsic.Bool;
            if (ctx.STR() != null)
                return AstTypeIntrinsic.Str;
            if (ctx.F16() != null)
                return AstTypeIntrinsic.F16;
            if (ctx.F32() != null)
                return AstTypeIntrinsic.F32;
            if (ctx.I8() != null)
                return AstTypeIntrinsic.I8;
            if (ctx.I16() != null)
                return AstTypeIntrinsic.I16;
            // if (ctx.I24() != null)
            //     return AstTypeIntrinsic.I24;
            if (ctx.I32() != null)
                return AstTypeIntrinsic.I32;
            if (ctx.U8() != null)
                return AstTypeIntrinsic.U8;
            if (ctx.U16() != null)
                return AstTypeIntrinsic.U16;
            // if (ctx.U24() != null)
            //     return AstTypeIntrinsic.U24;
            if (ctx.U32() != null)
                return AstTypeIntrinsic.U32;

            return null;
        }
    }
}