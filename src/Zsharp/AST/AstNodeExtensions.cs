namespace Zsharp.AST
{
    public static class AstNodeExtensions
    {
        public static bool IsTopLevel(this AstNode node)
        {
            if (node.ParentAs<AstAssignment>() is not null)
                node = node.Parent!;

            return node?.ParentAs<AstCodeBlock>()?.ParentAs<AstFile>() is not null;
        }
    }
}
