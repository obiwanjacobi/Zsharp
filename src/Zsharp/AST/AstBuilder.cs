using System.Collections.Generic;
using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstBuilder
    {
        private readonly AstBuilderContext _context = new AstBuilderContext(0);
        private readonly List<AstModule> _modules = new List<AstModule>();

        public IEnumerable<AstModule> Modules => _modules;

        public bool HasErrors => _context.HasErrors;

        public IEnumerable<AstError> Errors => _context.Errors;

        public AstModule AddModule(Statement_moduleContext moduleCtx)
        {
            Ast.Guard(moduleCtx, "Context is null.");
            var mod = new AstModule();
            mod.AddModule(moduleCtx);

            _modules.Add(mod);
            return mod;
        }

        public void Build(FileContext fileCtx)
        {
            var modCtx = ParseTreeNavigator.ToStatementModule(fileCtx);
            Ast.Guard(modCtx, "Module could not be found.");

            var module = AddModule(modCtx!);
            var file = BuildFile(module.Name, fileCtx);
            module.AddFile(file!);
        }

        public AstFile? BuildFile(string moduleName, FileContext fileCtx)
        {
            var builder = new AstNodeBuilder(_context, moduleName);
            var file = builder.VisitFile(fileCtx);
            return file as AstFile;
        }

        private static class ParseTreeNavigator
        {
            public static Module_statementContext? ToModuleStatement(FileContext fileCtx)
            {
                foreach (SourceContext src in fileCtx.source())
                {
                    var modStat = src.module_statement();
                    if (modStat != null)
                    {
                        return modStat;
                    }
                }

                return null;
            }

            public static Statement_moduleContext? ToStatementModule(FileContext fileCtx)
            {
                var moduleStatement = ToModuleStatement(fileCtx);

                if (moduleStatement != null)
                {
                    return moduleStatement.statement_module();
                }

                return null;
            }
        }
    }
}