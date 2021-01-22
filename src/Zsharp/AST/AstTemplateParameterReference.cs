using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTemplateParameterReference : AstTemplateParameter
    {
        public AstTemplateParameterReference(Template_param_useContext context)
            : base(context)
        { }

        public override bool TryResolve()
        {
            // TODO: ??
            return true;
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitTemplateParameterReference(this);
    }
}