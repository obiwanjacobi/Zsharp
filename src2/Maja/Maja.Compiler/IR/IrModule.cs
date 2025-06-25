using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrModule : IrNode
{
    public IrModule(SyntaxNode syntax, ModuleSymbol symbol, IrModuleScope scope,
        IEnumerable<IrImport> imports, IEnumerable<IrExport> exports,
        IEnumerable<IrNode> nodes)
        : base(syntax)
    {
        Symbol = symbol;
        Scope = scope;

        Imports = imports.ToImmutableArray();
        Exports = exports.ToImmutableArray();

        Nodes = nodes.ToImmutableArray();
    }

    // can be null if no mod keyword was found
    public ModuleSyntax? ModuleSyntax
        => base.Syntax as ModuleSyntax;

    public ModuleSymbol Symbol { get; }

    public IrModuleScope Scope { get; }

    public ImmutableArray<IrImport> Imports { get; }
    public ImmutableArray<IrExport> Exports { get; }
    public ImmutableArray<IrNode> Nodes { get; }

    public IEnumerable<IrStatement> Statements => Nodes.OfType<IrStatement>();
    public IEnumerable<IrDeclaration> Declarations => Nodes.OfType<IrDeclaration>();

    public IEnumerable<T> GetDescendantsOfType<T>() where T : IrNode
        => Nodes.GetDescendantsOfType<T>();
}