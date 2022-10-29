using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR
{
    internal sealed class IrCodeBlock : IrNode
    {
        public IrCodeBlock(CodeBlockSyntax syntax,
            IEnumerable<IrStatement> statements, IEnumerable<IrDeclaration> declarations)
            : base(syntax)
        {
            Statements = statements;
            Declarations = declarations;
        }

        public IEnumerable<IrStatement> Statements { get; }
        public IEnumerable<IrDeclaration> Declarations { get; }
    }
}