using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    internal void AddRange(IEnumerable<DiagnosticMessage> messages)
    {
        foreach (var msg in messages)
        {
            _messages.Add(msg);
        }
    }

    public bool HasDiagnostics => _messages.Count > 0;

    public bool Has(DiagnosticMessageKind kind)
        => _messages.Any(m => m.MessageKind == kind);

    public IEnumerator<DiagnosticMessage> GetEnumerator()
        => _messages.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _messages.GetEnumerator();
}
