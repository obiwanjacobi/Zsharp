using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeFieldReferenceStructField : AstTypeFieldReference
    {
        public AstTypeFieldReferenceStructField(Variable_field_refContext context)
            : base(context)
        { }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitTypeFieldReferenceStructField(this);
        }
    }
}
