namespace Zsharp.AST
{
    public partial class AstIdentifierIntrinsic : AstIdentifier
    {
        // Types
        public static readonly AstIdentifierIntrinsic U8 = new("U8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U16 = new("U16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U64 = new("U64", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic U32 = new("U32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I8 = new("I8", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I16 = new("I16", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I64 = new("I64", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic I32 = new("I32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F64 = new("F64", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic F32 = new("F32", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Str = new("Str", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Bool = new("Bool", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Void = new("Void", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Array = new("Array", AstIdentifierType.Type);
        // Parameters
        public static readonly AstIdentifierIntrinsic Self = new("self", AstIdentifierType.Parameter);
        // Template Parameters
        public static readonly AstIdentifierIntrinsic T = new("T", AstIdentifierType.TemplateParameter);

        public AstIdentifierIntrinsic(string name, AstIdentifierType identifierType)
            : base(name, identifierType)
        { }

        public override bool IsIntrinsic => true;
    }
}