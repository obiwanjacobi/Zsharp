using System.Runtime.InteropServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Maja.Compiler.Syntax;

/// <summary>
/// Represents a span of source code.
/// </summary>
/// <param name="Start">The starting position.</param>
/// <param name="End">The end position.</param>
public readonly record struct SyntaxSpan(int Start, int End)
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
[StructLayout(LayoutKind.Auto)]
public readonly record struct SyntaxLocation(string SourceName, int Line, int Column, SyntaxSpan Span)
{
    internal static SyntaxLocation From(ParserRuleContext context, string source = "")
        => new(source, context.Start.Line, context.Start.Column, new SyntaxSpan(context.Start.StartIndex, context.Stop.StopIndex));

    internal static SyntaxLocation From(ITerminalNode node, string source = "")
    {
        var token = node.Payload as CommonToken;
        return new(source, node.Symbol.Line, node.Symbol.Column,
            new SyntaxSpan(
                token?.StartIndex ?? node.SourceInterval.a,
                token?.StopIndex ?? node.SourceInterval.b));
    }

    public SyntaxLocation Append(SyntaxLocation location)
        => new(SourceName, Line, Column, Span.Append(location.Span));
}
