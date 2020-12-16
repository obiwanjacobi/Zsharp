using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTemplateParameterDefinition : AstTemplateParameter
    {
        public AstTemplateParameterDefinition(Template_param_anyContext context)
            : base(context)
        { }

        public override bool TryResolve()
        {
            return true;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitTemplateParameterDefinition(this);
        }
    }
}