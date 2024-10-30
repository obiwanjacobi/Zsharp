using System;
using System.Collections.Generic;
using System.Linq;
using Maja.Compiler.External;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Compilation;

/// <summary>
/// It represents an immutable snapshot of a complete compilation, containing:
/// All source code
/// Referenced assemblies
/// Compilation options
/// Target Assembly Metadata
/// 
/// It acts as the main entry point for semantic analysis by:
/// Providing symbol resolution
/// Enabling type checking
/// Managing binding operations (Ir)
/// Handling name resolution
/// 
/// It coordinates the compilation process by:
/// Managing syntax trees
/// Handling assembly references
/// Controlling emission of code
/// Managing diagnostics
/// </summary>
public sealed class Compilation
{
    private readonly List<SyntaxTree> _syntaxTrees = new();
    private readonly IExternalModuleLoader _moduleLoader;

    private Compilation(IEnumerable<SyntaxTree> syntaxTrees, IExternalModuleLoader moduleLoader)
    {
        _syntaxTrees.AddRange(syntaxTrees);
        _moduleLoader = moduleLoader;
    }

    public static Compilation Create(
        /*AssemblyInfo targetAssembly, */
        /*CompilationOptions options, */
        /*IEnumerable<AssemblyReference> references, */
        params SyntaxTree[] syntaxTrees)
    {
        var moduleLoader = new AssemblyManagerBuilder()
            .AddSystemAll()
            .ToModuleLoader();

        return new Compilation(syntaxTrees, moduleLoader);
    }

    public Compilation WithAddSyntaxTrees(params SyntaxTree[] syntaxTrees)
    {
        // TODO: check for duplicates
        var allSyntaxTrees = _syntaxTrees.Concat(syntaxTrees);
        return new Compilation(allSyntaxTrees, _moduleLoader);
    }

    public CompilationModel GetModel(SyntaxTree tree)
    {
        if (!_syntaxTrees.Contains(tree))
            throw new ArgumentException("Specified SyntaxTree does not belong to this Compilation.");

        return new CompilationModel(this, tree, _moduleLoader);
    }
}
