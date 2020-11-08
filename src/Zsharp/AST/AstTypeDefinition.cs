using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinition : AstType
    {
        public AstTypeDefinition(Type_defContext context)
        {
            Context = context;
        }

        protected AstTypeDefinition(AstIdentifier identifier)
            : base(identifier)
        { }

        public AstTypeReference? BaseType { get; internal set; }

        public virtual bool IsIntrinsic => false;

        public virtual bool IsExternal => false;

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

        public override void Accept(AstVisitor visitor) => visitor.VisitTypeDefinition(this);

        public static AstTypeDefinition? SelectKnownTypeDefinition(Known_typesContext context)
        {
            if (context == null)
                return null;
            //if (context.type_Bit()) return TypeBit;
            //if (context.type_Ptr()) return TypePtr;

            if (context.BOOL() != null)
                return AstTypeDefinitionIntrinsic.Bool;
            if (context.STR() != null)
                return AstTypeDefinitionIntrinsic.Str;
            if (context.F64() != null)
                return AstTypeDefinitionIntrinsic.F64;
            if (context.F32() != null)
                return AstTypeDefinitionIntrinsic.F32;
            if (context.I8() != null)
                return AstTypeDefinitionIntrinsic.I8;
            if (context.I16() != null)
                return AstTypeDefinitionIntrinsic.I16;
            if (context.I64() != null)
                return AstTypeDefinitionIntrinsic.I64;
            if (context.I32() != null)
                return AstTypeDefinitionIntrinsic.I32;
            if (context.U8() != null)
                return AstTypeDefinitionIntrinsic.U8;
            if (context.U16() != null)
                return AstTypeDefinitionIntrinsic.U16;
            if (context.U64() != null)
                return AstTypeDefinitionIntrinsic.U64;
            if (context.U32() != null)
                return AstTypeDefinitionIntrinsic.U32;

            return null;
        }
    }
}