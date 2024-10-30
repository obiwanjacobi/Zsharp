using Maja.Compiler.External;
using Maja.Compiler.IR;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Compilation;

public sealed class CompilationModel
{
    private readonly Compilation _compilation;

    internal CompilationModel(Compilation compilation, SyntaxTree syntaxTree, IExternalModuleLoader moduleLoader)
    {
        _compilation = compilation;
        SyntaxTree = syntaxTree;
        Program = IrBuilder.Program(syntaxTree, moduleLoader);
    }

    public SyntaxTree SyntaxTree { get; }

    internal IrProgram Program { get; }

    public Symbol.Symbol? GetDeclaredSymbol(MemberDeclarationSyntax declarationSyntax)
    {
        return null;
    }
}