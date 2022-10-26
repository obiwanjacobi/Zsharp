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
    public DiagnosticMessage(SyntaxLocation location, Exception exception)
    {
        MessageKind = DiagnosticMessageKind.Error;
        Location = location;
        Text = exception.Message;
        Error = exception;
    }

    public DiagnosticMessage(DiagnosticMessageKind kind, SyntaxLocation location, string message)
    {
        MessageKind = DiagnosticMessageKind.Error;
        Location = location;
        Text = message;
    }

    public DiagnosticMessageKind MessageKind { get; }
    public string Text { get; }
    public SyntaxLocation Location { get; }
    public Exception? Error { get; }

    public override string ToString()
        => $"{MessageKind}: {Text} at {Location.Line}, {Location.Column}.";
}