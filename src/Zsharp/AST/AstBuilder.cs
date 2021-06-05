using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBuilder
    {
        private readonly AstBuilderContext _context;

        public AstBuilder(CompilerContext compilerContext)
        {
            _context = new AstBuilderContext(compilerContext);
        }

        public bool HasErrors => _context.HasErrors;

        public IEnumerable<AstMessage> Errors => _context.Errors;

        public AstFile Build(FileContext fileCtx, string moduleName)
        {
            // look ahead
            var modCtx = ToStatementModule(fileCtx);
            if (modCtx is not null)
            {
                moduleName = AstSymbolName.ToCanonical(modCtx.module_name().identifier_module().GetText());
            }
            else
            {
                moduleName = AstSymbolName.ToCanonical(moduleName);
                var mod = _context.CompilerContext.Modules.GetOrAddModule(moduleName);
                _context.SetCurrent(mod);
            }

            var file = BuildFile(moduleName, fileCtx);

            if (modCtx is null)
            {
                // remove the AstModule we added.
                _context.RevertCurrent();
            }

            var module = _context.CompilerContext.Modules.FindModule<AstModulePublic>(file.Symbols.Name);
            module!.AddFile(file!);
            return file;
        }

        private AstFile BuildFile(string moduleName, FileContext fileCtx)
        {
            var builder = new AstNodeBuilder(_context, moduleName);
            var file = builder.VisitFile(fileCtx);
            return (AstFile)file!;
        }

        private static Statement_moduleContext? ToStatementModule(FileContext fileCtx)
            => fileCtx.header()
                .Select(h => h.module_statement())
                .Where(m => m?.statement_module() is not null)
                .Select(m => m.statement_module())
                .SingleOrDefault()
                ;
    }
}