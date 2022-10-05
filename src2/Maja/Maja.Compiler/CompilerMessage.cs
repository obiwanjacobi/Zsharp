using System;
using Maja.Compiler.Syntax;

namespace Maja.Compiler;

public enum CompilerMessageKind
{
    Trace,
    Info,
    Warning,
    Error,
    Critical,
}

public record CompilerMessage
{
    public CompilerMessage(CompilerMessageKind messageType, SyntaxLocation location, Exception exception)
    {
        MessageKind = messageType;
        Location = location;
        Text = exception.Message;
        Error = exception;
    }

    public CompilerMessageKind MessageKind { get; }
    public string Text { get; }
    public SyntaxLocation Location { get; }
    public Exception? Error { get; }

    public override string ToString()
        => $"{MessageKind}: {Text} at {Location.Line}, {Location.Column}.";
}