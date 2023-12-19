using System;
using System.IO;
using Antlr4.Runtime;

namespace Maja.UnitTests.Compiler;

internal sealed class ThrowingErrorListener<TSymbol> : IAntlrErrorListener<TSymbol>
{
    public void SyntaxError(TextWriter output, IRecognizer recognizer,
        TSymbol offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
    {
        throw new Exception($"Syntax Error: {msg} ({line}:{charPositionInLine}).\nToken: {offendingSymbol}", e);
    }
}