using System;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Diagnostics;

public enum DiagnosticMessageKind
{
    Trace,
    Info,
    Warning,
    Error,
    Critical,
}

public record DiagnosticMessage
{
    public DiagnosticMessage(DiagnosticMessageKind messageType, SyntaxLocation location, Exception exception)
    {
        MessageKind = messageType;
        Location = location;
        Text = exception.Message;
        Error = exception;
    }

    public DiagnosticMessageKind MessageKind { get; }
    public string Text { get; }
    public SyntaxLocation Location { get; }
    public Exception? Error { get; }

    public override string ToString()
        => $"{MessageKind}: {Text} at {Location.Line}, {Location.Column}.";
}