using Antlr4.Runtime;

namespace Zsharp.AST
{
    public class AstTypeFieldReferenceStructField : AstTypeFieldReference
    {
        internal AstTypeFieldReferenceStructField(ParserRuleContext context)
            : base(context)
        { }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTypeFieldReferenceStructField(this);
    }
}
