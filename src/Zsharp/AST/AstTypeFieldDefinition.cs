using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTypeFieldDefinition : AstTypeField
    {
        protected AstTypeFieldDefinition()
            : base(AstNodeType.Field)
        { }

        protected AstTypeFieldDefinition(ParserRuleContext context, AstNodeType nodeType = AstNodeType.Field)
            : base(context, nodeType)
        { }
    }
}
