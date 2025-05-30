using System.Collections.Generic;
using System.Collections.Immutable;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

internal sealed class IrProgram : IrNode
{
    public IrProgram(SyntaxNode syntax, IrModule module,
        IEnumerable<DiagnosticMessage> diagnostics)
        : base(syntax)
    {
        Module = module;
        Diagnostics = diagnostics.ToImmutableArray();
    }

    public IrModule Module { get; }
    public ImmutableArray<DiagnosticMessage> Diagnostics { get; }
}