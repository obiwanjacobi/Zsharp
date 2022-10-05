using System;
using Antlr4.Runtime;

namespace Maja.Compiler.Syntax;

public record struct SyntaxSpan(int Start, int End);
public record struct SyntaxLocation(string SourceName, int Line, int Column, SyntaxSpan Span)
{
    internal static SyntaxLocation From(ParserRuleContext context)
        => new(String.Empty, context.Start.Line, context.Start.Column, new SyntaxSpan(context.SourceInterval.a, context.SourceInterval.b));
}

