namespace Zsharp.AST
{
    public partial class AstIdentifierIntrinsic : AstIdentifier
    {
        // Types
        public static readonly AstIdentifierIntrinsic U8 = new("U8", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic U16 = new("U16", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic U64 = new("U64", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic U32 = new("U32", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic I8 = new("I8", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic I16 = new("I16", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic I64 = new("I64", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic I32 = new("I32", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic F64 = new("F64", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic F32 = new("F32", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic Str = new("Str", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic Bool = new("Bool", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic Void = new("Void", AstIdentifierKind.Type);
        public static readonly AstIdentifierIntrinsic Array = new("Array", AstIdentifierKind.Type);
        // Parameters
        public static readonly AstIdentifierIntrinsic Self = new("self", AstIdentifierKind.Parameter);
        // Template Parameters
        public static readonly AstIdentifierIntrinsic T = new("T", AstIdentifierKind.TemplateParameter);

        public AstIdentifierIntrinsic(string name, AstIdentifierKind identifierType)
            : base(name, identifierType)
        { }

        public AstIdentifierIntrinsic(AstIdentifierIntrinsic identifierToCopy)
            : base(identifierToCopy)
        { }

        public override bool IsIntrinsic => true;

        public override AstIdentifier MakeCopy()
            => new AstIdentifierIntrinsic(this);
    }
}