using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTypeFieldDefinition : AstTypeField
    {
        protected AstTypeFieldDefinition()
            : base(AstNodeKind.Field)
        { }

        protected AstTypeFieldDefinition(ParserRuleContext context, AstNodeKind nodeKind = AstNodeKind.Field)
            : base(context, nodeKind)
        { }
    }
}
