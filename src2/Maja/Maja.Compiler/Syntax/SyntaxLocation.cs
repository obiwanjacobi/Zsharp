using System;
using Antlr4.Runtime;

namespace Maja.Compiler.Syntax;

public record struct SyntaxSpan(int Start, int End)
{
    public SyntaxSpan Append(SyntaxSpan span)
        => new(Start, span.End);
}
public record struct SyntaxLocation(string SourceName, int Line, int Column, SyntaxSpan Span)
{
    internal static SyntaxLocation From(ParserRuleContext context)
        => new(String.Empty, context.Start.Line, context.Start.Column, new SyntaxSpan(context.SourceInterval.a, context.SourceInterval.b));

    public SyntaxLocation Append(SyntaxLocation location)
        => new(SourceName, Line, Column, Span.Append(location.Span));
}

