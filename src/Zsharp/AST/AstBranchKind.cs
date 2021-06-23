namespace Zsharp.AST
{
    public enum AstBranchKind
    {
        NotSet,
        Loop,               // loop
        Conditional,        // if-else
        ExitIteration,      // continue
        ExitLoop,           // break
        ExitFunction,       // return
        ExitProgram,        // abort
    }
}
