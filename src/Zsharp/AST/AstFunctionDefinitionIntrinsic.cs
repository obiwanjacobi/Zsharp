using System.Linq;

namespace Zsharp.AST
{
    public class AstFunctionDefinitionIntrinsic : AstFunctionDefinition
    {
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
