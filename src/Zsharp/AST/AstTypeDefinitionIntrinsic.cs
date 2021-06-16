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

        public static void AddAll(AstSymbolTable symbols)
        {
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.U8, typeof(Byte)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.U16, typeof(UInt16)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.U64, typeof(UInt64)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.U32, typeof(UInt32)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.I8, typeof(SByte)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.I16, typeof(Int16)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.I64, typeof(Int64)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.I32, typeof(Int32)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.C16, typeof(Char)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.F32, typeof(Single)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.F64, typeof(Double)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.F96, typeof(Decimal)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.Str, typeof(String)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.Bool, typeof(Boolean)));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.Void, null));
            AddIntrinsicSymbol(symbols, new(AstIdentifierIntrinsic.Array, typeof(Array), new(AstIdentifierIntrinsic.T)));
        }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }

        protected static void AddIntrinsicSymbol(AstSymbolTable symbols, AstTypeDefinitionIntrinsic type)
            => symbols.AddSymbol(type.Identifier!.CanonicalName, AstSymbolKind.Type, type);
    }
}