using Zsharp.Parser;

namespace Zsharp.AST
{
    public class AstTemplateInstanceFunction : AstFunctionDefinitionImpl
    {
        public AstTemplateInstanceFunction(ZsharpParser.Function_defContext functionCtx)
            : base(functionCtx)
        { }


    }
}
