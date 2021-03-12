using Antlr4.Runtime;

namespace Zsharp.AST
{
    public abstract class AstTemplateParameter : AstNode
    {
        protected AstTemplateParameter(ParserRuleContext? context)
            : base(AstNodeType.TemplateParameter)
        {
            Context = context;
        }

        public ParserRuleContext? Context { get; }

        public abstract bool TryResolve();
    }
}