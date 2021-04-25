using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public class CodeBuilder
    {
        private readonly CsBuilder _csBuilder;
        private bool _closeScope;

        public CodeBuilder(CsBuilder csBuilder)
        {
            _csBuilder = csBuilder;
        }

        public CsBuilder CsBuilder => _csBuilder;

        public void AddVariable(AstVariableDefinition variable)
        {
            _csBuilder.StartVariable(variable.TypeReference.Identifier.CanonicalName, variable.Identifier.CanonicalName);
            _csBuilder.EndLine();
        }

        public void StartBranch(AstBranchExpression branch)
        {
            var br = branch.BranchType switch
            {
                AstBranchType.ExitFunction => BranchStatement.Return,
                AstBranchType.ExitLoop => BranchStatement.Break,
                AstBranchType.ExitIteration => BranchStatement.Continue,
                AstBranchType.Conditional => BranchStatement.If,
                _ => throw new ZsharpException("No BranchType set!")
            };
            _csBuilder.StartBranch(br);
        }

        public void Apply()
        {
            if (_closeScope)
                _csBuilder.EndScope();
        }

        public override string ToString()
        {
            return _csBuilder.ToString();
        }

        internal void StartMethod(AccessModifiers access, MethodModifiers modifiers, string retType, string methodName, params (string name, string type)[] parameters)
        {
            _csBuilder.StartMethod(access, modifiers, retType, methodName, parameters);
            _closeScope = true;
        }
    }
}
