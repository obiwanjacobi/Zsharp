using Zsharp.AST;

namespace Zsharp.Semantics
{
    public abstract class AstVisitorWithSymbols : AstVisitor
    {
        private AstSymbolTable? _globalSymbols;
        private AstSymbolTable? _symbolTable;

        protected AstSymbolTable? GlobalSymbols => _globalSymbols;

        protected AstSymbolTable? SymbolTable => _symbolTable ?? _globalSymbols;

        public override void VisitCodeBlock(AstCodeBlock codeBlock)
        {
            var symbols = SetSymbolTable(codeBlock.Symbols);
            VisitChildren(codeBlock);
            SetSymbolTable(symbols);
        }

        public override void VisitFile(AstFile file)
        {
            var symbols = SetSymbolTable(file.Symbols);
            VisitChildren(file);
            SetSymbolTable(symbols);
        }

        protected AstSymbolTable? SetSymbolTable(AstSymbolTable? symbolTable)
        {
            if (_globalSymbols is null)
            {
                _globalSymbols = symbolTable;
            }

            var symbols = _symbolTable;
            _symbolTable = symbolTable;
            return symbols;
        }
    }
}
