using System.Collections;
using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Diagnostics
{
    internal sealed class DiagnosticList : IEnumerable<DiagnosticMessage>
    {
        private readonly List<DiagnosticMessage> _messages = new();

        private DiagnosticMessage Add(DiagnosticMessageKind kind, SyntaxLocation location, string message)
        {
            // TODO: check for duplicates

            var diagMsg = new DiagnosticMessage(kind, location, message);
            _messages.Add(diagMsg);
            return diagMsg;
        }

        public IEnumerator<DiagnosticMessage> GetEnumerator()
            => _messages.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _messages.GetEnumerator();

        public void FunctionAlreadyDelcared(SyntaxLocation location, string functionName)
            => Add(DiagnosticMessageKind.Error, location, $"Function '{functionName}' is already declared.");

        public void ParameterNameAlreadyDeclared(SyntaxLocation location, string parameterName)
            => Add(DiagnosticMessageKind.Error, location, $"Parameter name '{parameterName}' is already declared.");

        public void TypeAlreadyDelcared(SyntaxLocation location, string typeName)
            => Add(DiagnosticMessageKind.Error, location, $"Type '{typeName}' is already declared.");

        public void EnumValueNotConstant(SyntaxLocation location, string expr)
            => Add(DiagnosticMessageKind.Error, location, $"Enum initialization value expression '{expr}' is not a compiler constant.");

        public void FunctionNotFound(SyntaxLocation location, string functionName)
            => Add(DiagnosticMessageKind.Error, location, $"Function reference '{functionName}' cannot be resolved. Function not found.");

        public void CannotAssignVariableWithVoid(SyntaxLocation location, string variableName)
            => Add(DiagnosticMessageKind.Error, location, $"Cannot assign Void to variable '{variableName}'.");

        public void VariableNotFound(SyntaxLocation location, string variableName)
            => Add(DiagnosticMessageKind.Error, location, $"Variable reference '{variableName}' cannot be resolved. Variable not found.");

        public void VariableAlreadyDeclared(SyntaxLocation location, string variableName)
            => Add(DiagnosticMessageKind.Error, location, $"Variable name '{variableName}' is already declared.");

        public void TypeNotFound(SyntaxLocation location, string typeName)
            => Add(DiagnosticMessageKind.Error, location, $"Type reference '{typeName}' cannot be resolved. Type not found.");

        public void ImportNotFound(SyntaxLocation location, string importName)
            => Add(DiagnosticMessageKind.Error, location, $"Import reference '{importName}' cannot be resolved. Module not found.");
    }
}
