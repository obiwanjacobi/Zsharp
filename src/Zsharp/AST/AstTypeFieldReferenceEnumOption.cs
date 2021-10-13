using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTypeFieldReferenceEnumOption : AstTypeFieldReference
    {
        internal AstTypeFieldReferenceEnumOption(ParserRuleContext context)
            : base(context)
        { }

        public override bool TrySetIdentifier(AstIdentifier identifier)
            => TrySetIdentifier(identifier, AstIdentifierKind.EnumOption);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeFieldReferenceEnumOption(this);
    }
}
