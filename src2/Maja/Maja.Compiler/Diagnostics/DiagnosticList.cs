using System.Collections;
using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Diagnostics;

public sealed class DiagnosticList : IEnumerable<DiagnosticMessage>
{
    private readonly List<DiagnosticMessage> _messages = new();

    internal DiagnosticMessage Add(DiagnosticMessageKind kind, SyntaxLocation location, string message)
    {
        // TODO: check for duplicates

        var diagMsg = new DiagnosticMessage(kind, location, message);
        _messages.Add(diagMsg);
        return diagMsg;
    }

    public bool HasDiagnostics => _messages.Count > 0;

    public IEnumerator<DiagnosticMessage> GetEnumerator()
        => _messages.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _messages.GetEnumerator();
}
