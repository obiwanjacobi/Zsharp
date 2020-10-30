using System;

namespace Zsharp.AST
{
    public class AstTypeDefinitionIntrinsic : AstTypeDefinition
    {
        public AstTypeDefinitionIntrinsic(AstIdentifier identifier, Type systemType)
            : base(identifier)
        {
            SystemType = systemType ?? throw new ArgumentNullException(nameof(systemType));
        }

        public override bool IsIntrinsic => true;

        public Type SystemType { get; private set; }

        public static readonly AstTypeDefinitionIntrinsic U8 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.U8, typeof(Byte));
        public static readonly AstTypeDefinitionIntrinsic U16 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.U16, typeof(UInt16));
        public static readonly AstTypeDefinitionIntrinsic U64 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.U64, typeof(UInt64));
        public static readonly AstTypeDefinitionIntrinsic U32 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.U32, typeof(UInt32));
        public static readonly AstTypeDefinitionIntrinsic I8 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.I8, typeof(SByte));
        public static readonly AstTypeDefinitionIntrinsic I16 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.I16, typeof(Int16));
        public static readonly AstTypeDefinitionIntrinsic I64 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.I64, typeof(UInt64));
        public static readonly AstTypeDefinitionIntrinsic I32 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.I32, typeof(Int32));
        public static readonly AstTypeDefinitionIntrinsic F64 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.F64, typeof(Double));
        public static readonly AstTypeDefinitionIntrinsic F32 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.F32, typeof(Single));
        public static readonly AstTypeDefinitionIntrinsic Str = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.Str, typeof(String));
        public static readonly AstTypeDefinitionIntrinsic Bool = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.Bool, typeof(Boolean));

        public static void AddAll(AstSymbolTable symbols)
        {
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.Bool);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.F64);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.F32);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.I16);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.I64);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.I32);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.I8);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.Str);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.U16);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.U64);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.U32);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.U8);
        }

        private static void AddIntrinsicSymbol(AstSymbolTable symbols, AstTypeDefinitionIntrinsic type)
            => symbols.AddSymbol(type.Identifier!.Name, AstSymbolKind.Type, type);
    }
}