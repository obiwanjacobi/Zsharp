using System;
using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstModulePublic : AstModule
    {
        private readonly List<Statement_moduleContext> _contexts = new List<Statement_moduleContext>();
        private readonly List<Statement_exportContext> _exports = new List<Statement_exportContext>();

        public AstModulePublic(string moduleName)
            : base(moduleName, AstModuleLocality.Public)
        { }

        private readonly List<AstFile> _files = new List<AstFile>();
        public IEnumerable<AstFile> Files => _files;

        public IEnumerable<Statement_exportContext> Exports => _exports;

        public bool HasExports => _exports.Any() || _files.Any(f => f.HasExports);

        public void AddModule(Statement_moduleContext moduleCtx)
        {
            if (moduleCtx != null)
            {
                Ast.Guard(Name == moduleCtx.module_name().GetText(), "Not the same module.");
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

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitModulePublic(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var file in _files)
            {
                file.Accept(visitor);
            }
        }
    }
}
