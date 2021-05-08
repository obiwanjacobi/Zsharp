using Antlr4.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zsharp.AST;
using Zsharp.Parser;
using Zsharp.Semantics;

namespace Zsharp
{
    public class Compiler
    {
        private readonly AstBuilder _astBuilder;
        private readonly ResolveDefinition _resolveDefinition;
        private readonly CheckRules _checkRules;

        public Compiler(IAstModuleLoader moduleLoader)
        {
            Context = new CompilerContext(moduleLoader);
            _astBuilder = new AstBuilder(Context);
            _resolveDefinition = new ResolveDefinition(Context);
            _checkRules = new CheckRules(Context);
        }

        public CompilerContext Context { get; }

        public IEnumerable<AstMessage> Compile(string filePath, string defaultModuleName, string code)
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
            _resolveDefinition.Visit(file);
            _checkRules.Visit(file);
        }

        private ZsharpParser CreateParser(string sourceName, string code)
        {
            var stream = new AntlrInputStream(code)
            {
                name = sourceName
            };

            var lexer = new ZsharpLexer(stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ZsharpParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new AstErrorHandlerParser(Context));

            return parser;
        }

        private class AstErrorHandlerParser : IAntlrErrorListener<IToken>
        {
            private readonly CompilerContext _context;

            public AstErrorHandlerParser(CompilerContext context)
            {
                _context = context;
            }

            public void SyntaxError(TextWriter output, IRecognizer recognizer,
                IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                AstMessage err;
                if (e != null)
                {
                    err = _context.AddError((ParserRuleContext)e.Context, $"Syntax Error: {msg}");
                }
                else
                {
                    err = _context.AddError(line, charPositionInLine + 1, $"Syntax Error near '{offendingSymbol.Text}' : {msg}");
                }

                err.Source = recognizer.InputStream.SourceName;
            }
        }
    }
}
