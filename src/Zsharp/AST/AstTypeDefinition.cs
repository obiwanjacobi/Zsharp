using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinition : AstType
    {
        public AstTypeDefinition(Type_defContext context)
            : base(context.type_ref_use().type_ref().type_name())
        {
            Context = context;
        }
        protected AstTypeDefinition(AstIdentifier identifier)
            : base(identifier)
        { }

        public new Type_defContext? Context { get; }

        public AstTypeReference? BaseType { get; internal set; }

        public virtual bool IsIntrinsic => false;

        public static AstTypeDefinition Create(Type_defContext context)
        {
            var typeDef = new AstTypeDefinition(context)
            {
                BaseType = AstTypeReference.Create(context.type_ref_use())
            };

            var identifier = new AstIdentifier(context.identifier_type());
            typeDef.SetIdentifier(identifier);

            // TODO: type parameters: context.type_param_list()
            return typeDef;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitTypeDefinition(this);
        }

        public static AstTypeDefinition? SelectKnownTypeDefinition(Known_typesContext context)
        {
            if (context == null)
                return null;
            //if (context.type_Bit()) return TypeBit;
            //if (context.type_Ptr()) return TypePtr;

            if (context.BOOL() != null)
                return AstTypeIntrinsic.Bool;
            if (context.STR() != null)
                return AstTypeIntrinsic.Str;
            if (context.F64() != null)
                return AstTypeIntrinsic.F64;
            if (context.F32() != null)
                return AstTypeIntrinsic.F32;
            if (context.I8() != null)
                return AstTypeIntrinsic.I8;
            if (context.I16() != null)
                return AstTypeIntrinsic.I16;
            if (context.I64() != null)
                return AstTypeIntrinsic.I64;
            if (context.I32() != null)
                return AstTypeIntrinsic.I32;
            if (context.U8() != null)
                return AstTypeIntrinsic.U8;
            if (context.U16() != null)
                return AstTypeIntrinsic.U16;
            if (context.U64() != null)
                return AstTypeIntrinsic.U64;
            if (context.U32() != null)
                return AstTypeIntrinsic.U32;

            return null;
        }
    }
}