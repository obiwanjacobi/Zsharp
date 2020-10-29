using Zsharp.AST;

namespace Zsharp
{
    public class CompilerContext : AstErrorSite
    {
        public CompilerContext()
        {
            IntrinsicSymbols = CreateIntrinsicSymbols();
            Modules = new AstModuleManager();
        }

        public AstModuleManager Modules { get; }

        public AstSymbolTable IntrinsicSymbols { get; }

        private static AstSymbolTable CreateIntrinsicSymbols()
        {
            var symbols = new AstSymbolTable();
            AstTypeIntrinsic.AddAll(symbols);
            return symbols;
        }
    }
}
