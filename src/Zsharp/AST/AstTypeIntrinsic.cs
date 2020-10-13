using System;

namespace Zsharp.AST
{
    public class AstTypeIntrinsic : AstTypeDefinition
    {
        public AstTypeIntrinsic(AstIdentifier identifier)
            : base(identifier)
        { }

        public static readonly AstTypeIntrinsic U8 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U8);
        public static readonly AstTypeIntrinsic U16 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U16);
        public static readonly AstTypeIntrinsic U24 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U24);
        public static readonly AstTypeIntrinsic U32 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U32);
        public static readonly AstTypeIntrinsic I8 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I8);
        public static readonly AstTypeIntrinsic I16 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I16);
        public static readonly AstTypeIntrinsic I24 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I24);
        public static readonly AstTypeIntrinsic I32 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I32);
        public static readonly AstTypeIntrinsic F16 = new AstTypeIntrinsic(AstIdentifierIntrinsic.F16);
        public static readonly AstTypeIntrinsic F32 = new AstTypeIntrinsic(AstIdentifierIntrinsic.F32);
        public static readonly AstTypeIntrinsic Str = new AstTypeIntrinsic(AstIdentifierIntrinsic.Str);
        public static readonly AstTypeIntrinsic Bool = new AstTypeIntrinsic(AstIdentifierIntrinsic.Bool);

        public static void AddAll(AstSymbolTable symbols)
        {
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.Bool);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.F16);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.F32);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I16);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I24);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I32);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I8);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.Str);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U16);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U24);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U32);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U8);
        }

        private static void AddIntrinsicSymbol(AstSymbolTable symbols, AstTypeIntrinsic type)
        {
            if (type?.Identifier == null)
                throw new ArgumentNullException(nameof(type));
            symbols.AddSymbol(type.Identifier.Name, AstSymbolKind.Type, type);
        }
    }
}