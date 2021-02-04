namespace Zsharp.AST
{
    public partial class AstIdentifierIntrinsic : AstIdentifier
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
        public static readonly AstIdentifierIntrinsic Void = new AstIdentifierIntrinsic("Void", AstIdentifierType.Type);
        public static readonly AstIdentifierIntrinsic Self = new AstIdentifierIntrinsic("self", AstIdentifierType.Parameter);

        public AstIdentifierIntrinsic(string name, AstIdentifierType identifierType)
            : base(name, identifierType)
        { }

        public static AstIdentifierIntrinsic? Lookup(string identifier)
        {
            return AstDotName.ToCanonical(identifier) switch
            {
                "U8" => U8,
                "U16" => U16,
                "U32" => U32,
                "U64" => U64,
                "I8" => I8,
                "I16" => I16,
                "I32" => I32,
                "I64" => I64,
                "F32" => F32,
                "F64" => F64,
                "Str" => Str,
                "Bool" => Bool,
                "Void" => Void,
                "self" => Self,
                _ => null
            };
        }
    }
}