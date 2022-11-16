using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrCompilation : IrNode
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
}