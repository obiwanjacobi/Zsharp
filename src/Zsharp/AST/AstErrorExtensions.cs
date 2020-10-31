namespace Zsharp.AST
{
    public static class AstErrorExtensions
    {
        public static AstError UndefinedVariable(this AstErrorSite errorSite, AstVariableReference variable)
        {
            return errorSite.AddError(variable, variable.Context, "Reference to an Undefined Variable.");
        }
    }
}
