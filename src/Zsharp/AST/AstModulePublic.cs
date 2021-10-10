using System;
using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstModulePublic : AstModule,
        IAstSymbolSite
    {
        private readonly List<Statement_moduleContext> _contexts = new();
        private readonly List<Statement_exportContext> _exports = new();
        private readonly List<AstFile> _files = new();

        public AstModulePublic(string moduleName)
            : base(AstModuleLocality.Public)
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
                Ast.Guard(Identifier!.CanonicalName == AstSymbolName.ToCanonical(moduleCtx.module_name().GetText()), "Not the same module.");
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

        private AstSymbol? _symbol;
        public AstSymbol? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbol? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);
    }
}
