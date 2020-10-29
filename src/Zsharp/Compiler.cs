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
        private readonly AstBuilder _astBuilder;
        private readonly ResolveSymbols _resolveSymbols;
        private readonly ResolveTypes _resolveTypes;

        public Compiler()
        {
            Context = new CompilerContext();
            _astBuilder = new AstBuilder(Context);
            _resolveSymbols = new ResolveSymbols(Context);
            _resolveTypes = new ResolveTypes(Context);
        }

        public CompilerContext Context { get; }

        public IEnumerable<AstError> Compile(string filePath, string defaultModuleName, string code)
        {
            var parser = CreateParser(filePath, code);
            var file = parser.file();
            var parseErrors = file.Errors();

            if (parseErrors.Any())
            {
                return parseErrors;
            }

            var astFile = _astBuilder.Build(file, defaultModuleName);
            if (_astBuilder.HasErrors)
            {
                return _astBuilder.Errors;
            }

            ApplySemantics(astFile);

            return Context.Errors;
        }

        private void ApplySemantics(AstFile file)
        {
            // Note: processing is in order
            _resolveSymbols.Visit(file);
            _resolveTypes.Visit(file);
        }

        private static ZsharpParser CreateParser(string sourceName, string code)
        {
            var stream = new AntlrInputStream(code)
            {
                name = sourceName
            };
            var lexer = new ZsharpLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ZsharpParser(tokenStream);
            return parser;
        }
    }
}
