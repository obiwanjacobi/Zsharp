using Antlr4.Runtime;

namespace Zsharp.AST
{
    public static class AstErrorExtensions
    {
        public static AstMessage UndefinedVariable(this AstErrorSite errorSite, AstVariableReference variable)
        {
            return errorSite.AddError(variable, variable.Context!,
                $"Reference to an undefined Variable '{variable.Identifier!.Name}'");
        }

        public static AstMessage UndefinedFunction(this AstErrorSite errorSite, AstFunctionReference function)
        {
            return errorSite.AddError(function, function.Context!,
                $"Reference to an undefined Function '{function}'");
        }

        public static AstMessage UndefinedType(this AstErrorSite errorSite, AstTypeReference type)
        {
            return errorSite.AddError(type, type.Context!,
                $"Reference to an undefined Type '{type.Identifier!.Name}'");
        }

        public static AstMessage ExpressionNoType(this AstErrorSite errorSite, AstExpression expression)
        {
            return errorSite.AddError(expression, expression.Context!,
                $"Could not determine the Type for Expression '{expression.AsString()}'");
        }

        public static AstMessage InvalidEnumBaseType(this AstErrorSite errorSite, AstTypeReference type)
        {
            return errorSite.AddError(type, type.Context!,
                $"Specified Type is not valid as an Enum Base Type: '{type.Identifier!.Name}'");
        }

        public static AstMessage UndefinedEnumeration(this AstErrorSite errorSite, AstTypeFieldReferenceEnumOption enumOption)
        {
            return errorSite.AddError(enumOption, enumOption.Context!,
                $"Reference to an undefined Enumeration Option '{enumOption.Identifier!.Name}'");
        }

        public static AstMessage FunctionReturnValueNotUsed(this AstErrorSite errorSite, AstFunctionReference function)
        {
            return errorSite.AddError(function, function.Context!,
                $"The return value must be assigned for Function '{function}'");
        }

        public static AstMessage InvalidIndentation(this AstErrorSite errorSite, ParserRuleContext context)
        {
            return errorSite.AddError(context,
                $"Number of Indentation characters is invalid");
        }

        public static AstMessage SyntaxError(this AstErrorSite errorSite, ParserRuleContext context)
        {
            return errorSite.AddError(context, $"Syntax Error");
        }
    }
}
