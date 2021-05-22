using System.Linq;

namespace Zsharp.AST
{
    public class AstFunctionDefinitionIntrinsic : AstFunctionDefinition
    {
        public static readonly AstFunctionDefinitionIntrinsic ConvertU8ToU16 = new(
                AstIdentifierIntrinsic.U16, AstTypeDefinitionIntrinsic.U8, AstTypeDefinitionIntrinsic.U16);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU8ToU32 = new(
            AstIdentifierIntrinsic.U32, AstTypeDefinitionIntrinsic.U8, AstTypeDefinitionIntrinsic.U32);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU16ToU32 = new(
            AstIdentifierIntrinsic.U32, AstTypeDefinitionIntrinsic.U16, AstTypeDefinitionIntrinsic.U32);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU8ToU64 = new(
            AstIdentifierIntrinsic.U64, AstTypeDefinitionIntrinsic.U8, AstTypeDefinitionIntrinsic.U64);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU16ToU64 = new(
            AstIdentifierIntrinsic.U64, AstTypeDefinitionIntrinsic.U16, AstTypeDefinitionIntrinsic.U64);
        public static readonly AstFunctionDefinitionIntrinsic ConvertU32ToU64 = new(
            AstIdentifierIntrinsic.U64, AstTypeDefinitionIntrinsic.U32, AstTypeDefinitionIntrinsic.U64);

        public AstFunctionDefinitionIntrinsic(AstIdentifierIntrinsic typeIdentifier, AstTypeDefinitionIntrinsic toReturn)
        {
            this.SetIdentifier(typeIdentifier);
            SetTypeReference(toReturn);
        }

        public AstFunctionDefinitionIntrinsic(AstIdentifierIntrinsic typeIdentifier, AstTypeDefinitionIntrinsic selfParameter, AstTypeDefinitionIntrinsic toReturn)
            : this(typeIdentifier, toReturn)
        {
            SetSelfParameter(selfParameter);
        }

        public void AddParameter(string name, AstTypeDefinition astType)
        {
            var parameter = AstFunctionParameterDefinition.Create(name, astType);
            AddParameter(parameter);
        }

        public void SetSelfParameter(AstTypeDefinitionIntrinsic type)
        {
            Ast.Guard(!Parameters.Any(), "A Self parameter has to be first.");
            var parameter = new AstFunctionParameterDefinition(AstIdentifierIntrinsic.Self);
            parameter.SetTypeReference(AstTypeReference.From(type));
            AddParameter(parameter);
        }

        public override bool IsIntrinsic => true;

        public static void AddAll(AstSymbolTable symbols)
        {
            AddIntrinsicSymbol(symbols, ConvertU8ToU16);
            AddIntrinsicSymbol(symbols, ConvertU8ToU32);
            AddIntrinsicSymbol(symbols, ConvertU16ToU32);
            AddIntrinsicSymbol(symbols, ConvertU8ToU64);
            AddIntrinsicSymbol(symbols, ConvertU16ToU64);
            AddIntrinsicSymbol(symbols, ConvertU32ToU64);
        }

        private static void AddIntrinsicSymbol(AstSymbolTable symbols, AstFunctionDefinitionIntrinsic function)
            => function.CreateSymbols(symbols);

        private void SetTypeReference(AstTypeDefinitionIntrinsic type)
            => this.SetTypeReference(AstTypeReference.From(type));

        public override void Accept(AstVisitor visitor)
            => throw new InternalErrorException("Must not Visit Intrinsic Function Definition.");

        public override bool TrySetSymbol(AstSymbolEntry? symbolEntry)
        {
            // Intrinsic Function Definitions are static and have no reference to the Symbol Table.
            return true;    // fake success
        }
    }
}
