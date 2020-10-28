using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using Zsharp.AST;
using Zsharp.Parser;
using Zsharp.Semantics;

namespace Zsharp
{
    public class Compiler
    {
        private readonly CompilerContext _context = new CompilerContext();
        private readonly AstBuilder _astBuilder;
        private readonly ResolveSymbols _resolveSymbols;
        private readonly ResolveTypes _resolveTypes;

        public Compiler()
        {
            _astBuilder = new AstBuilder(_context);
            _resolveSymbols = new ResolveSymbols(_context);
            _resolveTypes = new ResolveTypes(_context);
        }

        public IEnumerable<AstError> Compile(string filePath, string defaultModuleName, string code)
        {
            var parser = CreateParser(code);
            var file = parser.file();
            var parseErrors = file.Errors();

            if (parseErrors.Any())
            {
                return parseErrors;
            }

            // TODO: file/module handling
            var astFile = _astBuilder.BuildFile(defaultModuleName, file);
            if (_astBuilder.HasErrors)
            {
                return _astBuilder.Errors;
            }

            ApplySemantics(astFile);

            return _context.Errors;
        }

        private void ApplySemantics(AstFile file)
        {
            // Note: processing is in order
            _resolveSymbols.Visit(file);
            _resolveTypes.Visit(file);
        }

        private static ZsharpParser CreateParser(string code)
        {
            var stream = new AntlrInputStream(code);
            var lexer = new ZsharpLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ZsharpParser(tokenStream);
            return parser;
        }
    }
}
