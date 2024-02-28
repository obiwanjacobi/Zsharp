using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrCompilation : IrNode, IrContainer
{
    public IrCompilation(SyntaxNode syntax,
        IEnumerable<IrImport> imports, IEnumerable<IrExport> exports,
        IEnumerable<IrStatement> statements, IEnumerable<IrDeclaration> declarations)
        : base(syntax)
    {
        Imports = imports.ToImmutableArray();
        Exports = exports.ToImmutableArray();
        Statements = statements.ToImmutableArray();
        Declarations = declarations.ToImmutableArray();
    }

    public ImmutableArray<IrImport> Imports { get; }
    public ImmutableArray<IrExport> Exports { get; }
    public ImmutableArray<IrStatement> Statements { get; }
    public ImmutableArray<IrDeclaration> Declarations { get; }

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => Statements.GetDescendentsOfType<T>().Concat(Declarations.GetDescendentsOfType<T>());
}