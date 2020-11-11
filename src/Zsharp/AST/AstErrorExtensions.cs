namespace Zsharp.AST
{
    public static class AstErrorExtensions
    {
        public static AstMessage UndefinedVariable(this AstErrorSite errorSite, AstVariableReference variable)
        {
            return errorSite.AddError(variable, variable.Context,
                $"Reference to an undefined Variable '{variable.Identifier.Name}'.");
        }

        public static AstMessage UndefinedFunction(this AstErrorSite errorSite, AstFunctionReference function)
        {
            return errorSite.AddError(function, function.Context,
                $"Reference to an undefined Function '{function.Identifier.Name}'.");
        }

        public static AstMessage UndefinedType(this AstErrorSite errorSite, AstTypeReference type)
        {
            return errorSite.AddError(type, type.Context,
                $"Reference to an undefined Type '{type.Identifier.Name}'.");
        }

        public static AstMessage OverloadNotFound(this AstErrorSite errorSite, AstFunctionReference function)
        {
            return errorSite.AddError(function, function.Context,
                $"No overload was found for '{function}'.");
        }
    }
}
