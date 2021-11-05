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
            _csBuilder.StartVariable(variable.TypeReference.ToCode(), variable.Identifier!.SymbolName.CanonicalName.FullName);
        }

        public void StartBranch(AstBranchExpression branch)
        {
            var br = branch.BranchKind switch
            {
                AstBranchKind.ExitFunction => BranchStatement.Return,
                AstBranchKind.ExitLoop => BranchStatement.Break,
                AstBranchKind.ExitIteration => BranchStatement.Continue,
                AstBranchKind.Conditional => BranchStatement.If,
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
