using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeFieldReferenceEnumOption : AstTypeFieldReference
    {
        public AstTypeFieldReferenceEnumOption(Enum_option_useContext context)
            : base(context)
        { }

        public override bool TrySetIdentifier(AstIdentifier identifier)
            => TrySetIdentifier(identifier, AstIdentifierKind.EnumOption);

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeFieldReferenceEnumOption(this);
    }
}
