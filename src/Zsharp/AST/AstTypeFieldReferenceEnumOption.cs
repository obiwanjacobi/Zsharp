using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeFieldReferenceEnumOption : AstTypeFieldReference
    {
        public AstTypeFieldReferenceEnumOption(Enum_option_useContext context)
            : base(context)
        { }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeFieldReferenceEnumOption(this);
    }
}
