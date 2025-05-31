using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Diagnostics;

internal static class ParserDiagnostics
{
    public static IEnumerable<DiagnosticMessage> Errors(this ParserRuleContext ctx)
    {
        var errors = new List<DiagnosticMessage>();

        if (ctx.exception is not null)
        {
            var location = SyntaxLocation.From(ctx);
            errors.Add(new DiagnosticMessage(location, ctx.exception));
        }

        if (ctx.children is not null)
        {
            foreach (var c in ctx.children.OfType<ParserRuleContext>())
            {
                var childErrs = c.Errors();
                if (childErrs.Any())
                    errors.AddRange(childErrs);
            }
        }
        return errors;
    }
}

internal static class SyntaxDiagnostics
{
    public static void AddAll(this DiagnosticList diagnostics, IEnumerable<ErrorToken> errors)
    {
        foreach (var err in errors)
        {
            diagnostics.Add(DiagnosticMessageKind.Error, err.Location, err.Text);
        }
    }

    public static void ExpressionPrecedenceNotSpecified(this DiagnosticList diagnostics, SyntaxLocation location, string expression)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Expression '{expression}' requires explicit precedence specification using '()' because it uses more than one (type of) operator.");
}

internal static class IrDiagnostics
{
    public static void FunctionAlreadyDeclared(this DiagnosticList diagnostics, SyntaxLocation location, string functionName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Function '{functionName}' is already declared.");

    public static void FunctionTemplateAlreadyDeclared(this DiagnosticList diagnostics, SyntaxLocation location, string functionName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Function Template '{functionName}' is already declared.");

    public static void ParameterNameAlreadyDeclared(this DiagnosticList diagnostics, SyntaxLocation location, string parameterName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Parameter name '{parameterName}' is already declared.");

    public static void TypeAlreadyDeclared(this DiagnosticList diagnostics, SyntaxLocation location, string typeName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Type '{typeName}' is already declared.");

    public static void EnumValueNotConstant(this DiagnosticList diagnostics, SyntaxLocation location, string expr)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Enum initialization value expression '{expr}' is not a compiler constant.");

    public static void FunctionNotFound(this DiagnosticList diagnostics, SyntaxLocation location, string functionName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Function reference '{functionName}' cannot be resolved. Function not found.");

    public static void FunctionTemplateNotFound(this DiagnosticList diagnostics, SyntaxLocation location, string functionName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Function template reference '{functionName}' cannot be resolved. Function not found.");

    public static void NoParameterForNamedArgument(this DiagnosticList diagnostics, SyntaxLocation location, string argumentName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"The named argument'{argumentName}' does not match to a function parameter. Function parameter not found.");

    public static void CannotDetermineArgumentType(this DiagnosticList diagnostics, SyntaxLocation location, string argument)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Cannot determine the Type for argument '{argument}'.");

    public static void MismatchTypeArgumentCount(this DiagnosticList diagnostics, SyntaxLocation location, string functionName, int count)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"The number of Type-Arguments {count} is mismatched for function '{functionName}'.");

    public static void MismatchArgumentCount(this DiagnosticList diagnostics, SyntaxLocation location, string functionName, int count)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"The number of Arguments {count} is mismatched for function '{functionName}'.");

    public static void VoidFunctionCannotReturnValue(this DiagnosticList diagnostics, SyntaxLocation location, string functionName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"The Void function '{functionName}' cannot return a value.");

    public static void CannotAssignVariableWithVoid(this DiagnosticList diagnostics, SyntaxLocation location, string variableName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Cannot assign Void to variable '{variableName}'.");

    public static void VariableNotFound(this DiagnosticList diagnostics, SyntaxLocation location, string variableName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Variable reference '{variableName}' cannot be resolved. Variable not found.");

    public static void VariableAlreadyDeclared(this DiagnosticList diagnostics, SyntaxLocation location, string variableName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Variable name '{variableName}' is already declared.");

    public static void TypeNotFound(this DiagnosticList diagnostics, SyntaxLocation location, string typeName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Type reference '{typeName}' cannot be resolved. Type not found.");

    public static void TypeMismatch(this DiagnosticList diagnostics, SyntaxLocation location, string sourceType, string targetType)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Cannot implicitly use Type '{sourceType}' as Type '{targetType}'.");

    public static void FieldNotFoundOnType(this DiagnosticList diagnostics, SyntaxLocation location, string typeName, string fieldName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Field reference '{fieldName}' cannot be resolved. Type {typeName} does not define it.");

    public static void TypeTemplateAlreadyDeclared(this DiagnosticList diagnostics, SyntaxLocation location, string typeName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Type Template '{typeName}' is already declared.");

    public static void DiscardOnlyInvocation(this DiagnosticList diagnostics, SyntaxLocation location)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Invalid Assignment. Only the result of an invocation can be assigned to the discard '_'.");

    public static void InvalidLoopExpression(this DiagnosticList diagnostics, SyntaxLocation location)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Invalid Loop Expression. A loop expression must evaluate to a count (I32), a Boolean or a Range.");

    public static void InvalidStatementExpression(this DiagnosticList diagnostics, SyntaxLocation location)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Invalid Expression Statement: Can only be an Invocation Expression.");

    public static void ImportNotFound(this DiagnosticList diagnostics, SyntaxLocation location, string importName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Import reference '{importName}' cannot be resolved. Module not found.");

    public static void InvalidExpressionLeftValueMemberAccess(this DiagnosticList diagnostics, SyntaxLocation location, string leftExpression)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"The expression '{leftExpression}' cannot be used as a left-value in a member-access expression.");
}
