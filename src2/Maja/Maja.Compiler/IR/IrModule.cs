using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrModule : IrNode
{
    public IrModule(SyntaxNode syntax, ModuleSymbol symbol, IrModuleScope scope,
        IEnumerable<IrImport> imports, IEnumerable<IrExport> exports,
        IEnumerable<IrStatement> statements, IEnumerable<IrDeclaration> declarations)
        : base(syntax)
    {
        Symbol = symbol;
        Scope = scope;

        Imports = imports.ToImmutableArray();
        Exports = exports.ToImmutableArray();
        Statements = statements.ToImmutableArray();
        Declarations = declarations.ToImmutableArray();
    }

    // can be null if no mod keyword was found
    public ModuleSyntax? ModuleSyntax
        => base.Syntax as ModuleSyntax;

    public ModuleSymbol Symbol { get; }

    public IrModuleScope Scope { get; }

    public ImmutableArray<IrImport> Imports { get; }
    public ImmutableArray<IrExport> Exports { get; }
    public ImmutableArray<IrStatement> Statements { get; }
    public ImmutableArray<IrDeclaration> Declarations { get; }

    public IEnumerable<T> GetDescendentsOfType<T>() where T : IrNode
        => Statements.GetDescendentsOfType<T>().Concat(Declarations.GetDescendentsOfType<T>());
}