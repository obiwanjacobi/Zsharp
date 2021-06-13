using System;

namespace Zsharp.AST
{
    public class AstTypeDefinitionIntrinsic : AstTypeDefinitionTemplate
    {
        public AstTypeDefinitionIntrinsic(AstIdentifier identifier, Type? systemType,
            AstTemplateParameterDefinition? templateParameter = null)
        {
            this.SetIdentifier(identifier);
            SystemType = systemType;

            if (templateParameter is not null)
                this.AddTemplateParameter(templateParameter);
        }

        public override bool IsIntrinsic => true;

        public Type? SystemType { get; private set; }

        public static readonly AstTypeDefinitionIntrinsic U8 = new(AstIdentifierIntrinsic.U8, typeof(Byte));
        public static readonly AstTypeDefinitionIntrinsic U16 = new(AstIdentifierIntrinsic.U16, typeof(UInt16));
        public static readonly AstTypeDefinitionIntrinsic U64 = new(AstIdentifierIntrinsic.U64, typeof(UInt64));
        public static readonly AstTypeDefinitionIntrinsic U32 = new(AstIdentifierIntrinsic.U32, typeof(UInt32));
        public static readonly AstTypeDefinitionIntrinsic I8 = new(AstIdentifierIntrinsic.I8, typeof(SByte));
        public static readonly AstTypeDefinitionIntrinsic I16 = new(AstIdentifierIntrinsic.I16, typeof(Int16));
        public static readonly AstTypeDefinitionIntrinsic I64 = new(AstIdentifierIntrinsic.I64, typeof(Int64));
        public static readonly AstTypeDefinitionIntrinsic I32 = new(AstIdentifierIntrinsic.I32, typeof(Int32));
        public static readonly AstTypeDefinitionIntrinsic F64 = new(AstIdentifierIntrinsic.F64, typeof(Double));
        public static readonly AstTypeDefinitionIntrinsic F32 = new(AstIdentifierIntrinsic.F32, typeof(Single));
        public static readonly AstTypeDefinitionIntrinsic Str = new(AstIdentifierIntrinsic.Str, typeof(String));
        public static readonly AstTypeDefinitionIntrinsic Bool = new(AstIdentifierIntrinsic.Bool, typeof(Boolean));
        public static readonly AstTypeDefinitionIntrinsic Void = new(AstIdentifierIntrinsic.Void, null);
        public static readonly AstTypeDefinitionIntrinsic Array = new(AstIdentifierIntrinsic.Array, typeof(Array), AstTemplateParameterDefinitionIntrinsic.T);

        public static AstTypeDefinitionIntrinsic? Lookup(string typeName)
        {
            return AstSymbolName.ToCanonical(typeName) switch
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
                "Array" => Array,
                "Array%1" => Array,
                _ => null
            };
        }

        public static void AddAll(AstSymbolTable symbols)
        {
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.Array);
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

        protected static void AddIntrinsicSymbol(AstSymbolTable symbols, AstTypeDefinitionIntrinsic type)
            => symbols.AddSymbol(type.Identifier!.CanonicalName, AstSymbolKind.Type, type);

        public override bool TrySetSymbol(AstSymbolEntry? symbolEntry)
        {
            throw new InternalErrorException(
                "Intrinsic Type Definitions are static and have no reference to the Symbol Table.");
        }
    }
}