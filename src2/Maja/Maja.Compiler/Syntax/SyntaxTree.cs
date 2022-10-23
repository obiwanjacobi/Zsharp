using System;
using System.IO;
using Antlr4.Runtime;
using Maja.Compiler.Parser;

namespace Maja.Compiler.Syntax;

// the root/accessor of the syntax tree
// implements helper methods and probably cache
public class SyntaxTree
{
    private readonly AntlrInputStream _inputStream;
    private readonly MajaLexer _lexer;
    private readonly CommonTokenStream _tokens;
    private readonly MajaParser _parser;

    private SyntaxTree()
    {
        _inputStream = new AntlrInputStream();
        _lexer = new MajaLexer(_inputStream);
        _lexer.InitializeTokens(MajaLexer.Indent, MajaLexer.Dedent, MajaLexer.Eol);
        _tokens = new CommonTokenStream(_lexer);
        _parser = new MajaParser(_tokens);
    }

    private CompilationUnitSyntax ParseInternal(string code, string sourceName)
    {
        _inputStream.Load(new StringReader(code), AntlrInputStream.InitialBufferSize, AntlrInputStream.ReadBufferSize);
        _tokens.Reset();

        var builder = new ParserNodeConvertor(sourceName);
        var cu = _parser.compilationUnit();
        var nodes = builder.VisitCompilationUnit(cu);
        return (CompilationUnitSyntax)nodes[0].Node!;
    }

    public static SyntaxTree Parse(string code, string sourceName = "")
    {
        var tree = new SyntaxTree();
        tree._root = tree.ParseInternal(code, sourceName);
        return tree;
    }

    private CompilationUnitSyntax? _root;
    public CompilationUnitSyntax Root
    {
        get { return _root ?? throw new InvalidOperationException("No root SyntaxNode was initialized."); }
    }
}
