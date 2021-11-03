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
    }
}
