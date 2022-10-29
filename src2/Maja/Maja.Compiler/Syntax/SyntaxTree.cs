using System;
using Antlr4.Runtime;
using Maja.Compiler.Parser;

namespace Maja.Compiler.Syntax;

// the root/accessor of the syntax tree
// implements helper methods and probably cache
public class SyntaxTree
{
    private AntlrInputStream? _inputStream;
    private MajaLexer? _lexer;
    private CommonTokenStream? _tokens;
    private MajaParser? _parser;

    private SyntaxTree()
    { }

    private CompilationUnitSyntax ParseInternal(string code, string sourceName)
    {
        _inputStream = new AntlrInputStream(code)
        {
            name = sourceName
        };
        _lexer = new MajaLexer(_inputStream)
        {
            WhitespaceMode = Dentlr.WhitespaceMode.Skip
        };
        _lexer.InitializeTokens(MajaLexer.Indent, MajaLexer.Dedent, MajaLexer.Eol);
        _tokens = new CommonTokenStream(_lexer);
        _parser = new MajaParser(_tokens);

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
        => _root ?? throw new InvalidOperationException("Root was not initialized.");
}
