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

        public DiagnosticMessage FunctionAlreadyDelcared(SyntaxLocation location, string functionName)
            => Add(DiagnosticMessageKind.Error, location, $"Function '{functionName} is already declared.");

        public DiagnosticMessage ParameterNameAlreadyDeclared(SyntaxLocation location, string parameterName)
            => Add(DiagnosticMessageKind.Error, location, $"Parameter name '{parameterName} is already declared.");

        public DiagnosticMessage EnumValueNotConstant(SyntaxLocation location, string expr)
            => Add(DiagnosticMessageKind.Error, location, $"Enum initialization value expression '{expr}' is not a compiler constant.");
    }
}
