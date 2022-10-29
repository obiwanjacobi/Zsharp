using System.Collections.Generic;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR
{
    internal class IrCompilation : IrNode
    {
        public IrCompilation(SyntaxNode syntax,
            IEnumerable<IrImport> imports, IEnumerable<IrExport> exports,
            IEnumerable<IrStatement> statements, IEnumerable<IrDeclaration> members)
            : base(syntax)
        {
            Imports = imports;
            Exports = exports;
            Statements = statements;
            Members = members;
        }

        public IEnumerable<IrImport> Imports { get; }
        public IEnumerable<IrExport> Exports { get; }
        public IEnumerable<IrStatement> Statements { get; }
        public IEnumerable<IrDeclaration> Members { get; }
    }
}