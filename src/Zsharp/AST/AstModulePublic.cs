using System;
using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstModulePublic : AstModule,
        IAstSymbolEntrySite
    {
        private readonly List<Statement_moduleContext> _contexts = new();
        private readonly List<Statement_exportContext> _exports = new();
        private readonly List<AstFile> _files = new();

        public AstModulePublic(string moduleName)
            : base(AstModuleLocality.Public)
        {
            SetIdentifier(new AstIdentifier(moduleName, AstIdentifierType.Module));
        }

        public IEnumerable<AstFile> Files => _files;

        public IEnumerable<Statement_exportContext> Exports => _exports;

        public bool HasExports => _exports.Any() || _files.Any(f => f.HasExports);

        public void AddModule(Statement_moduleContext moduleCtx)
        {
            if (moduleCtx != null)
            {
                Ast.Guard(Identifier.Name == moduleCtx.module_name().GetText(), "Not the same module.");
                _contexts.Add(moduleCtx);
            }
        }

        public void AddExport(Statement_exportContext exportCtx)
        {
            if (exportCtx == null)
                throw new ArgumentNullException(nameof(exportCtx));

            _exports.Add(exportCtx);
        }

        public void AddFile(AstFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            _files.Add(file);
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitModulePublic(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var file in _files)
            {
                file.Accept(visitor);
            }
        }

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
        {
            if (!TrySetSymbol(symbolEntry))
                throw new InvalidOperationException("Symbol is already set or null.");
        }

        public virtual bool TryResolve() => true;
    }
}
