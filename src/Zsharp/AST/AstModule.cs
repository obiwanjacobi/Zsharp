using System;
using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public enum ModuleLocality
    {
        Public,
        External
    }

    public class AstModule : AstNode
    {
        private readonly List<Statement_moduleContext> _contexts = new List<Statement_moduleContext>();
        private readonly List<Statement_importContext> _imports = new List<Statement_importContext>();
        private readonly List<Statement_exportContext> _exports = new List<Statement_exportContext>();

        public AstModule(string modName)
            : base(AstNodeType.Module)
        {
            Name = modName;
        }

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitModule(this);
        }

        public override void VisitChildren(AstVisitor visitor)
        {
            foreach (var file in _files)
            {
                file.Accept(visitor);
            }
        }

        public ModuleLocality Locality { get; set; }

        public string Name { get; private set; }

        private readonly List<AstFile> _files = new List<AstFile>();
        public IEnumerable<AstFile> Files => _files;

        public IEnumerable<Statement_importContext> Imports => _imports;

        public IEnumerable<Statement_exportContext> Exports => _exports;

        public bool HasExports => _exports.Any();

        public void AddModule(Statement_moduleContext moduleCtx)
        {
            if (moduleCtx != null)
            {
                Ast.Guard(Name == moduleCtx.module_name().GetText(), "Not the same module.");
                _contexts.Add(moduleCtx);
            }
        }

        public void AddImport(Statement_importContext importCtx, AstModule externalModule)
        {
            // TODO: externalModule
            if (importCtx == null)
                throw new ArgumentNullException(nameof(importCtx));

            _imports.Add(importCtx);
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
    }
}