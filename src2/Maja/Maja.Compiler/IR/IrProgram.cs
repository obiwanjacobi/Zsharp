using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrProgram : IrNode
{
    public IrProgram(SyntaxNode syntax, IrScope scope, IrCompilation root,
        IEnumerable<DiagnosticMessage> diagnostics)
        : base(syntax)
    {
        Scope = scope;
        Root = root;
        Diagnostics = diagnostics.ToImmutableArray();
    }

    public IrScope Scope { get; }
    public IrCompilation Root { get; }
    public ImmutableArray<DiagnosticMessage> Diagnostics { get; }
}