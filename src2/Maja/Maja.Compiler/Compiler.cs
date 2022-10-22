using System.IO;
using Antlr4.Runtime;
using Maja.Compiler.Parser;
using Maja.Compiler.Syntax;

namespace Maja.Compiler;

internal sealed class Compiler
{
    private readonly AntlrInputStream _inputStream;
    private readonly MajaLexer _lexer;
    private readonly CommonTokenStream _tokens;
    private readonly MajaParser _parser;

    public Compiler()
    {
        _inputStream = new AntlrInputStream();
        _lexer = new MajaLexer(_inputStream);
        _lexer.InitializeTokens(MajaLexer.Indent, MajaLexer.Dedent, MajaLexer.Eol);
        _tokens = new CommonTokenStream(_lexer);
        _parser = new MajaParser(_tokens);
    }

    public CompilationUnitSyntax Parse(string code, string sourceName = "")
    {
        _inputStream.Load(new StringReader(code), AntlrInputStream.InitialBufferSize, AntlrInputStream.ReadBufferSize);
        _tokens.Reset();

        var builder = new ParserNodeConvertor(sourceName);
        var cu = _parser.compilationUnit();
        var nodes = builder.VisitCompilationUnit(cu);
        return (CompilationUnitSyntax)nodes[0].Node!;
    }

    public static MajaParser CreateParser(string code, string sourceName = "", bool throwOnError = false)
    {
        var lexer = CreateLexer(code, sourceName, throwOnError);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new MajaParser(tokenStream);

        if (throwOnError)
        {
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ThrowingErrorListener<IToken>());
        }
#if DEBUG
        else
            parser.AddErrorListener(new DiagnosticErrorListener());
#endif

        return parser;
    }

    public static MajaLexer CreateLexer(string code, string sourceName = "", bool throwOnError = false)
    {
        var stream = new AntlrInputStream(code)
        {
            name = sourceName
        };
        var lexer = new MajaLexer(stream)
        {
            WhitespaceMode = Dentlr.WhitespaceMode.Skip
        };
        lexer.InitializeTokens(MajaLexer.Indent, MajaLexer.Dedent, MajaLexer.Eol);

        if (throwOnError)
        {
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowingErrorListener<int>());
        }

        return lexer;
    }
}