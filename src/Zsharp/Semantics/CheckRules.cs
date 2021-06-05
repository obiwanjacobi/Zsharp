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
                function.ParentAs<AstCodeBlock>() is not null &&
                function.FunctionType.TypeReference is not null &&
                function.FunctionType.TypeReference?.TypeDefinition?.Identifier != AstIdentifierIntrinsic.Void)
            {
                _errorSite.FunctionReturnValueNotUsed(function);
            }
        }
    }
}
