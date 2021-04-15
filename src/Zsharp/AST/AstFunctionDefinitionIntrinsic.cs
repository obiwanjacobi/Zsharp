namespace Zsharp.AST
{
    public class AstFunctionDefinitionIntrinsic : AstFunctionDefinition
    {
        public AstFunctionDefinitionIntrinsic(AstIdentifierIntrinsic typeIdentifier, AstTypeDefinitionIntrinsic selfParameter, AstTypeDefinitionIntrinsic toReturn)
            : this(typeIdentifier, toReturn)
        {
            SetSelfParameter(selfParameter);
        }

        public AstFunctionDefinitionIntrinsic(AstIdentifierIntrinsic typeIdentifier, AstTypeDefinitionIntrinsic toReturn)
        {
            SetIdentifier(typeIdentifier);
            SetTypeReference(toReturn);
        }

        public void AddParameter(string name, AstTypeDefinition astType)
        {
            var parameter = AstFunctionParameterDefinition.Create(name, astType);
            AddParameter(parameter);
        }

        public void SetSelfParameter(AstTypeDefinitionIntrinsic type)
        {
            var parameter = new AstFunctionParameterDefinition(AstIdentifierIntrinsic.Self);
            parameter.SetTypeReference(AstTypeReference.From(type));
            AddParameter(parameter);
        }

        public override bool IsIntrinsic => true;

        public static void AddAll(AstSymbolTable symbols)
        {
            var convertU8ToU16 = new AstFunctionDefinitionIntrinsic(
                AstIdentifierIntrinsic.U16, AstTypeDefinitionIntrinsic.U8, AstTypeDefinitionIntrinsic.U16);
            var convertU8ToU32 = new AstFunctionDefinitionIntrinsic(
                AstIdentifierIntrinsic.U32, AstTypeDefinitionIntrinsic.U8, AstTypeDefinitionIntrinsic.U32);
            var convertU16ToU32 = new AstFunctionDefinitionIntrinsic(
                AstIdentifierIntrinsic.U32, AstTypeDefinitionIntrinsic.U16, AstTypeDefinitionIntrinsic.U32);
            var convertU8ToU64 = new AstFunctionDefinitionIntrinsic(
                AstIdentifierIntrinsic.U64, AstTypeDefinitionIntrinsic.U8, AstTypeDefinitionIntrinsic.U64);
            var convertU16ToU64 = new AstFunctionDefinitionIntrinsic(
                AstIdentifierIntrinsic.U64, AstTypeDefinitionIntrinsic.U16, AstTypeDefinitionIntrinsic.U64);
            var convertU32ToU64 = new AstFunctionDefinitionIntrinsic(
                AstIdentifierIntrinsic.U64, AstTypeDefinitionIntrinsic.U32, AstTypeDefinitionIntrinsic.U64);

            AddIntrinsicSymbol(symbols, convertU8ToU16);
            AddIntrinsicSymbol(symbols, convertU8ToU32);
            AddIntrinsicSymbol(symbols, convertU16ToU32);
            AddIntrinsicSymbol(symbols, convertU8ToU64);
            AddIntrinsicSymbol(symbols, convertU16ToU64);
            AddIntrinsicSymbol(symbols, convertU32ToU64);
        }

        private static void AddIntrinsicSymbol(AstSymbolTable symbols, AstFunctionDefinitionIntrinsic function)
            => function.CreateSymbols(symbols);

        private void SetTypeReference(AstTypeDefinitionIntrinsic type)
            => SetTypeReference(AstTypeReference.From(type));

        public override void Accept(AstVisitor visitor)
            => throw new System.NotImplementedException("Must not Visit Intrinsic Function Definition.");
    }
}
