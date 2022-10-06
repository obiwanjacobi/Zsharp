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

        var builder = new SyntaxNodeBuilder(sourceName);
        var cu = _parser.compilationUnit();
        var nodes = builder.VisitCompilationUnit(cu);
        return (CompilationUnitSyntax)nodes[0];
    }

    public static MajaParser CreateParser(string code, string sourceName = "")
    {
        var lexer = CreateLexer(code, sourceName);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new MajaParser(tokenStream);
        //parser.RemoveErrorListeners();
        //parser.AddErrorListener(new AstErrorHandlerParser(Context));

#if DEBUG
        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ThrowingErrorListener<IToken>());
#endif

        return parser;
    }

    public static MajaLexer CreateLexer(string code, string sourceName = "")
    {
        var stream = new AntlrInputStream(code)
        {
            name = sourceName
        };
        var lexer = new MajaLexer(stream);
        lexer.InitializeTokens(MajaLexer.Indent, MajaLexer.Dedent, MajaLexer.Eol);

#if DEBUG
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new ThrowingErrorListener<int>());
#endif

        return lexer;
    }
}