using System;
using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstModuleImpl : AstModule,
        IAstSymbolSite
    {
        private readonly List<Statement_moduleContext> _contexts = new();
        private readonly List<Statement_exportContext> _exports = new();
        private readonly List<AstFile> _files = new();

        public AstModuleImpl(string moduleName)
            : base(AstModuleLocality.Local)
        {
            this.SetIdentifier(new AstIdentifier(moduleName, AstIdentifierKind.Module));
        }

        public IEnumerable<AstFile> Files => _files;

        public IEnumerable<Statement_exportContext> Exports => _exports;

        public bool HasExports => _exports.Any() || _files.Any(f => f.HasExports);

        public void AddModule(Statement_moduleContext moduleCtx)
        {
            if (moduleCtx is not null)
            {
                Ast.Guard(Identifier!.SymbolName.CanonicalName.FullName == AstSymbolName.ToCanonical(moduleCtx.module_name().GetText()), "Not the same module.");
                _contexts.Add(moduleCtx);
            }
        }

        public void AddExport(Statement_exportContext exportCtx)
        {
            if (exportCtx is null)
                throw new ArgumentNullException(nameof(exportCtx));

            _exports.Add(exportCtx);
        }

        public void AddFile(AstFile file)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            _files.Add(file);
        }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitModulePublic(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var file in _files)
            {
                file.Accept(visitor);
            }
        }

        public bool HasSymbol => _symbol is not null;

        private AstSymbol? _symbol;
        public AstSymbol Symbol => _symbol ?? throw new InternalErrorException("Symbol was not set.");

        public bool TrySetSymbol(AstSymbol? symbol)
            => Ast.SafeSet(ref _symbol, symbol);
    }
}
