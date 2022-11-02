using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrProgram : IrNode
{
    public IrProgram(SyntaxNode syntax, IrModuleScope scope, IrModule module, IrCompilation root,
        IEnumerable<DiagnosticMessage> diagnostics)
        : base(syntax)
    {
        Scope = scope;
        Module = module;
        Root = root;
        Diagnostics = diagnostics.ToImmutableArray();
    }

    public IrModuleScope Scope { get; }
    public IrModule Module { get; }
    public IrCompilation Root { get; }
    public ImmutableArray<DiagnosticMessage> Diagnostics { get; }
}