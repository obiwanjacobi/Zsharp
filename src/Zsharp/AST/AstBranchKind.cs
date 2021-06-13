namespace Zsharp.AST
{
    public enum AstBranchKind
    {
        NotSet,
        Conditional,        // if-else
        ExitIteration,      // continue
        ExitLoop,           // break
        ExitFunction,       // return
        ExitProgram,        // abort
    }
}
