using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionReference : AstFunction
    {
        public AstFunctionReference(Function_callContext context)
        {
            Context = context;
        }

        public Function_callContext Context { get; }

        private AstFunctionDefinition? _functionDefinition;

        public AstFunctionDefinition? FunctionDefinition => _functionDefinition;

        public bool TrySetFunctionDefinition(AstFunctionDefinition functionDefinition) => Ast.SafeSet(ref _functionDefinition, functionDefinition);

        public void SetFunctionDefinition(AstFunctionDefinition functionDefinition)
        {
            if (!TrySetFunctionDefinition(functionDefinition))
                throw new InvalidOperationException(
                    "Function Definition is already set or null.");
        }
    }
}
