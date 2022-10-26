using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal class IrScope : IrNode
{
    public IrScope(SyntaxNode syntax,
        IEnumerable<IrStatement> statements, IEnumerable<IrDeclaration> declarations)
        : base(syntax)
    {
        Statements = statements.ToImmutableArray();
        Declarations = declarations.ToImmutableArray();
    }

    public ImmutableArray<IrStatement> Statements { get; }
    public ImmutableArray<IrDeclaration> Declarations { get; }

    // LookupSymbol
}

internal sealed class IrModuleScope : IrScope
{
    public IrModuleScope(SyntaxNode syntax,
        IEnumerable<IrExport> exports, IEnumerable<IrImport> imports,
        IEnumerable<IrStatement> statements, IEnumerable<IrDeclaration> declarations)
        : base(syntax, statements, declarations)
    {
        Exports = exports.ToImmutableArray();
        Imports = imports.ToImmutableArray();
    }

    public ImmutableArray<IrExport> Exports { get; }
    public ImmutableArray<IrImport> Imports { get; }
}