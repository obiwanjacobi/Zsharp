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

        public override void VisitTypeReferenceType(AstTypeReferenceType type)
        {
            if (type.TypeDefinition is null)
            {
                _errorSite.UndefinedType(type);
            }

            type.VisitChildren(this);
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            if (!variable.HasDefinition)
            {
                _errorSite.UndefinedVariable(variable);
            }

            variable.VisitChildren(this);
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            if (function.FunctionDefinition is null)
            {
                _errorSite.UndefinedFunction(function);
            }
            else if (function.EnforceReturnValueUse &&
                function.ParentAs<AstCodeBlock>() is not null &&
                function.FunctionType.HasTypeReference &&
                function.FunctionType.TypeReference.TypeDefinition?.Identifier != AstIdentifierIntrinsic.Void)
            {
                _errorSite.FunctionReturnValueNotUsed(function);
            }

            function.VisitChildren(this);
        }
    }
}
