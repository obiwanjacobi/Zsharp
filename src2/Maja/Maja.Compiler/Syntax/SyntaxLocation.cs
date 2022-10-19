using System;
using Antlr4.Runtime;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a span of source code.
/// </summary>
/// <param name="Start">The starting position.</param>
/// <param name="End">The end position.</param>
public record struct SyntaxSpan(int Start, int End)
{
    public SyntaxSpan Append(SyntaxSpan span)
        => new(Start, span.End);
}
/// <summary>
/// Represents the location in the source code.
/// </summary>
/// <param name="SourceName">The name of the source (file).</param>
/// <param name="Line">The source line.</param>
/// <param name="Column">The character column.</param>
/// <param name="Span">The source span.</param>
public record struct SyntaxLocation(string SourceName, int Line, int Column, SyntaxSpan Span)
{
    internal static SyntaxLocation From(ParserRuleContext context)
        => new(String.Empty, context.Start.Line, context.Start.Column, new SyntaxSpan(context.SourceInterval.a, context.SourceInterval.b));

    public SyntaxLocation Append(SyntaxLocation location)
        => new(SourceName, Line, Column, Span.Append(location.Span));
}
