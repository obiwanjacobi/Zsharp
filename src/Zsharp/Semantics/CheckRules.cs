using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class CheckRules : AstVisitorWithSymbols
    {
        private readonly AstErrorSite _errorSite;

        public CheckRules(AstErrorSite errorSite)
        {
            _errorSite = errorSite;
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            if (function.EnforceReturnValueUse &&
                function.ParentAs<AstCodeBlock>() != null &&
                function.TypeReference != null)
            {
                _errorSite.FunctionReturnValueNotUsed(function);
            }
        }
    }
}
