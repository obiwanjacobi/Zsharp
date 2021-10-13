using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstFunctionParameterDefinition : AstFunctionParameter
    {
        internal AstFunctionParameterDefinition(ParserRuleContext context, bool isSelf)
            : base(context)
        {
            IsSelf = isSelf;
        }

        public AstFunctionParameterDefinition(AstIdentifier identifier)
            : base(null)
        {
            this.SetIdentifier(identifier);
            IsSelf = identifier.IsEqual(AstIdentifierIntrinsic.Self);
        }

        public bool IsSelf { get; }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFunctionParameterDefinition(this);

        public static AstFunctionParameterDefinition Create(string name, AstTypeDefinition astType)
        {
            var identifier = new AstIdentifier(name, AstIdentifierKind.Parameter);
            var param = new AstFunctionParameterDefinition(identifier);
            param.SetTypeReference(AstTypeReferenceType.From(astType));
            return param;
        }
    }
}
