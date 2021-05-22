using Zsharp.AST;

namespace Zsharp.EmitCS
{
    internal sealed class CodeBuilder
    {
        private readonly CsBuilder _csBuilder;

        internal CodeBuilder(CsBuilder csBuilder)
        {
            _csBuilder = csBuilder;
        }

        internal CsBuilder CsBuilder => _csBuilder;

        public void AddVariable(AstVariableDefinition variable)
        {
            _csBuilder.StartVariable(variable.TypeReference.ToCode(), variable.Identifier!.CanonicalName);
        }

        public void StartBranch(AstBranchExpression branch)
        {
            var br = branch.BranchType switch
            {
                AstBranchType.ExitFunction => BranchStatement.Return,
                AstBranchType.ExitLoop => BranchStatement.Break,
                AstBranchType.ExitIteration => BranchStatement.Continue,
                AstBranchType.Conditional => BranchStatement.If,
                _ => throw new InternalErrorException("No BranchType set!")
            };
            _csBuilder.StartBranch(br);
        }

        public override string ToString()
        {
            return _csBuilder.ToString();
        }
    }
}
