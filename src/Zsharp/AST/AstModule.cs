using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstModule : AstNode
    {
        private readonly List<Statement_moduleContext> _contexts = new List<Statement_moduleContext>();

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

        public string Name { get; private set; }

        private readonly List<AstFile> _files = new List<AstFile>();
        public IEnumerable<AstFile> Files => _files;

        public bool HasExports => _files.Any(f => f.Exports.Any());

        public void AddModule(Statement_moduleContext moduleCtx)
        {
            if (moduleCtx != null)
            {
                Ast.Guard(Name == moduleCtx.module_name().GetText(), "Not the same module.");
                _contexts.Add(moduleCtx);
            }
        }

        public void AddFile(AstFile file)
        {
            if (file != null)
            {
                _files.Add(file);
            }
        }
    }
}