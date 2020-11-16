using Zsharp.AST;

namespace Zsharp
{
    public class CompilerContext : AstErrorSite
    {
        public CompilerContext(IAstModuleLoader moduleLoader)
        {
            IntrinsicSymbols = CreateIntrinsicSymbols();
            Modules = new AstModuleManager(moduleLoader);
        }

        public AstModuleManager Modules { get; }

        public AstSymbolTable IntrinsicSymbols { get; }

        private static AstSymbolTable CreateIntrinsicSymbols()
        {
            var symbols = new AstSymbolTable();
            AstTypeDefinitionIntrinsic.AddAll(symbols);
            AstFunctionDefinitionIntrinsic.AddAll(symbols);
            return symbols;
        }
    }
}
