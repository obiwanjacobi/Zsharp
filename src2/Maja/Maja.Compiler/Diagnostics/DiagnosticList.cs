using System.Collections;
using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Diagnostics
{
    internal sealed class DiagnosticList : IEnumerable<DiagnosticMessage>
    {
        private readonly List<DiagnosticMessage> _messages = new();

        public DiagnosticMessage Add(DiagnosticMessageKind kind, SyntaxLocation location, string message)
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

        // make error specific reporting methods.
        public void AddXxxx(SyntaxLocation location, string param1)
        {

        }
    }
}
