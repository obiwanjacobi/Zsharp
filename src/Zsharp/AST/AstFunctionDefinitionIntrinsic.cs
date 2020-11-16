namespace Zsharp.AST
{
    public class AstFunctionDefinitionIntrinsic : AstFunctionDefinition
    {
        public static readonly AstIdentifierIntrinsic SourceParameterIdentifier = new AstIdentifierIntrinsic("source", AstIdentifierType.Parameter);

        public static readonly AstFunctionDefinitionIntrinsic ConvertU8ToU16 = new AstFunctionDefinitionIntrinsic(
            AstIdentifierIntrinsic.U16, AstTypeDefinitionIntrinsic.U8, AstTypeDefinitionIntrinsic.U16);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU8ToU32 = new AstFunctionDefinitionIntrinsic(
            AstIdentifierIntrinsic.U32, AstTypeDefinitionIntrinsic.U8, AstTypeDefinitionIntrinsic.U32);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU16ToU32 = new AstFunctionDefinitionIntrinsic(
            AstIdentifierIntrinsic.U32, AstTypeDefinitionIntrinsic.U16, AstTypeDefinitionIntrinsic.U32);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU8ToU64 = new AstFunctionDefinitionIntrinsic(
            AstIdentifierIntrinsic.U64, AstTypeDefinitionIntrinsic.U8, AstTypeDefinitionIntrinsic.U64);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU16ToU64 = new AstFunctionDefinitionIntrinsic(
            AstIdentifierIntrinsic.U64, AstTypeDefinitionIntrinsic.U16, AstTypeDefinitionIntrinsic.U64);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU32ToU64 = new AstFunctionDefinitionIntrinsic(
            AstIdentifierIntrinsic.U64, AstTypeDefinitionIntrinsic.U32, AstTypeDefinitionIntrinsic.U64);

        public AstFunctionDefinitionIntrinsic(AstIdentifierIntrinsic typeIdentifier, AstTypeDefinitionIntrinsic fromParameter, AstTypeDefinitionIntrinsic toReturn)
        {
            SetIdentifier(typeIdentifier);
            SetParameter(fromParameter);
            SetTypeReference(toReturn);
        }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }

        public static void AddAll(AstSymbolTable symbols)
        {
            AddIntrinsicSymbol(symbols, AstFunctionDefinitionIntrinsic.ConvertU8ToU16);
            AddIntrinsicSymbol(symbols, AstFunctionDefinitionIntrinsic.ConvertU8ToU32);
            AddIntrinsicSymbol(symbols, AstFunctionDefinitionIntrinsic.ConvertU16ToU32);
            AddIntrinsicSymbol(symbols, AstFunctionDefinitionIntrinsic.ConvertU8ToU64);
            AddIntrinsicSymbol(symbols, AstFunctionDefinitionIntrinsic.ConvertU16ToU64);
            AddIntrinsicSymbol(symbols, AstFunctionDefinitionIntrinsic.ConvertU32ToU64);
        }

        private static void AddIntrinsicSymbol(AstSymbolTable symbols, AstFunctionDefinitionIntrinsic function)
            => symbols.AddSymbol(function.Identifier!.CanonicalName, AstSymbolKind.Function, function);

        private void SetParameter(AstTypeDefinitionIntrinsic type)
        {
            var parameter = new AstFunctionParameterDefinition(SourceParameterIdentifier);
            parameter.SetTypeReference(AstTypeReference.Create(type));
            AddParameter(parameter);
        }

        private void SetTypeReference(AstTypeDefinitionIntrinsic type)
        {
            SetTypeReference(AstTypeReference.Create(type));
        }
    }
}
