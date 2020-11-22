using System;

namespace Zsharp.AST
{
    public class AstTypeDefinitionIntrinsic : AstTypeDefinition
    {
        public AstTypeDefinitionIntrinsic(AstIdentifier identifier, bool isUnsigned, Type? systemType)
            : base(identifier)
        {
            IsUnsigned = isUnsigned;
            SystemType = systemType;
        }

        public override bool IsIntrinsic => true;

        public override bool IsUnsigned { get; }

        public Type? SystemType { get; private set; }

        public static readonly AstTypeDefinitionIntrinsic U8 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.U8, true, typeof(Byte));
        public static readonly AstTypeDefinitionIntrinsic U16 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.U16, true, typeof(UInt16));
        public static readonly AstTypeDefinitionIntrinsic U64 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.U64, true, typeof(UInt64));
        public static readonly AstTypeDefinitionIntrinsic U32 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.U32, true, typeof(UInt32));
        public static readonly AstTypeDefinitionIntrinsic I8 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.I8, false, typeof(SByte));
        public static readonly AstTypeDefinitionIntrinsic I16 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.I16, false, typeof(Int16));
        public static readonly AstTypeDefinitionIntrinsic I64 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.I64, false, typeof(UInt64));
        public static readonly AstTypeDefinitionIntrinsic I32 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.I32, false, typeof(Int32));
        public static readonly AstTypeDefinitionIntrinsic F64 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.F64, false, typeof(Double));
        public static readonly AstTypeDefinitionIntrinsic F32 = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.F32, false, typeof(Single));
        public static readonly AstTypeDefinitionIntrinsic Str = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.Str, false, typeof(String));
        public static readonly AstTypeDefinitionIntrinsic Bool = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.Bool, false, typeof(Boolean));
        public static readonly AstTypeDefinitionIntrinsic Void = new AstTypeDefinitionIntrinsic(AstIdentifierIntrinsic.Void, false, null);

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
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.Void);
        }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }

        private static void AddIntrinsicSymbol(AstSymbolTable symbols, AstTypeDefinitionIntrinsic type)
            => symbols.AddSymbol(type.Identifier!.CanonicalName, AstSymbolKind.Type, type);
    }
}