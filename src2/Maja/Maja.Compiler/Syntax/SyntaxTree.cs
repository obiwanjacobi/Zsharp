using System;
using Antlr4.Runtime;
using Maja.Compiler.Diagnostics;
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

        var context = _parser.compilationUnit();
        var builder = new SyntaxTreeBuilder(sourceName);
        var nodes = builder.VisitCompilationUnit(context);

        _diagnostics = builder.Diagnostics;
        return (CompilationUnitSyntax)nodes[0].Node!;
    }

    public static SyntaxTree Parse(string code, string sourceName = "")
    {
        var tree = new SyntaxTree();
        tree._root = tree.ParseInternal(code, sourceName);
        tree.Diagnostics.AddAll(tree._root.GetErrors());

        return tree;
    }

    private CompilationUnitSyntax? _root;
    public CompilationUnitSyntax Root
        => _root ?? throw new InvalidOperationException("Root was not initialized.");

    private DiagnosticList? _diagnostics;
    public DiagnosticList Diagnostics
        => _diagnostics ?? throw new InvalidOperationException("No Diagnostics have been initialized.");
}
