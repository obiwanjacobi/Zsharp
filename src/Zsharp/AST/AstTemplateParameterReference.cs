using System;
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
            return true;
        }

        public override void Accept(AstVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}