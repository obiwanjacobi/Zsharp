namespace Zsharp.AST
{
    public enum AstBranchType
    {
        NotSet,
        Conditional,        // if-else
        ExitIteration,      // continue
        ExitLoop,           // break
        ExitFunction,       // return
        ExitProgram,        // abort
    }
}
