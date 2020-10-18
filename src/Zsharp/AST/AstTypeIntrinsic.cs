using System;

namespace Zsharp.AST
{
    public class AstTypeIntrinsic : AstTypeDefinition
    {
        public AstTypeIntrinsic(AstIdentifier identifier, Type systemType)
            : base(identifier)
        {
            SystemType = systemType ?? throw new ArgumentNullException(nameof(systemType));
        }

        public override bool IsIntrinsic => true;

        public Type SystemType { get; private set; }

        public static readonly AstTypeIntrinsic U8 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U8, typeof(Byte));
        public static readonly AstTypeIntrinsic U16 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U16, typeof(UInt16));
        public static readonly AstTypeIntrinsic U64 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U64, typeof(UInt64));
        public static readonly AstTypeIntrinsic U32 = new AstTypeIntrinsic(AstIdentifierIntrinsic.U32, typeof(UInt32));
        public static readonly AstTypeIntrinsic I8 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I8, typeof(SByte));
        public static readonly AstTypeIntrinsic I16 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I16, typeof(Int16));
        public static readonly AstTypeIntrinsic I64 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I64, typeof(UInt64));
        public static readonly AstTypeIntrinsic I32 = new AstTypeIntrinsic(AstIdentifierIntrinsic.I32, typeof(Int32));
        public static readonly AstTypeIntrinsic F64 = new AstTypeIntrinsic(AstIdentifierIntrinsic.F64, typeof(Double));
        public static readonly AstTypeIntrinsic F32 = new AstTypeIntrinsic(AstIdentifierIntrinsic.F32, typeof(Single));
        public static readonly AstTypeIntrinsic Str = new AstTypeIntrinsic(AstIdentifierIntrinsic.Str, typeof(String));
        public static readonly AstTypeIntrinsic Bool = new AstTypeIntrinsic(AstIdentifierIntrinsic.Bool, typeof(Boolean));

        public static void AddAll(AstSymbolTable symbols)
        {
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.Bool);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.F64);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.F32);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I16);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I64);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I32);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.I8);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.Str);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U16);
            AddIntrinsicSymbol(symbols, AstTypeIntrinsic.U64);
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