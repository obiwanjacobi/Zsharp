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
}

internal static class IrDiagnostics
{
    public static void FunctionAlreadyDelcared(this DiagnosticList diagnostics, SyntaxLocation location, string functionName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Function '{functionName}' is already declared.");

    public static void ParameterNameAlreadyDeclared(this DiagnosticList diagnostics, SyntaxLocation location, string parameterName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Parameter name '{parameterName}' is already declared.");

    public static void TypeAlreadyDelcared(this DiagnosticList diagnostics, SyntaxLocation location, string typeName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Type '{typeName}' is already declared.");

    public static void EnumValueNotConstant(this DiagnosticList diagnostics, SyntaxLocation location, string expr)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Enum initialization value expression '{expr}' is not a compiler constant.");

    public static void FunctionNotFound(this DiagnosticList diagnostics, SyntaxLocation location, string functionName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Function reference '{functionName}' cannot be resolved. Function not found.");

    public static void CannotAssignVariableWithVoid(this DiagnosticList diagnostics, SyntaxLocation location, string variableName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Cannot assign Void to variable '{variableName}'.");

    public static void VariableNotFound(this DiagnosticList diagnostics, SyntaxLocation location, string variableName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Variable reference '{variableName}' cannot be resolved. Variable not found.");

    public static void VariableAlreadyDeclared(this DiagnosticList diagnostics, SyntaxLocation location, string variableName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Variable name '{variableName}' is already declared.");

    public static void TypeNotFound(this DiagnosticList diagnostics, SyntaxLocation location, string typeName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Type reference '{typeName}' cannot be resolved. Type not found.");

    public static void ImportNotFound(this DiagnosticList diagnostics, SyntaxLocation location, string importName)
        => diagnostics.Add(DiagnosticMessageKind.Error, location, $"Import reference '{importName}' cannot be resolved. Module not found.");
}
