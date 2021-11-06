namespace Zsharp.AST
{
    public interface IAstCodeBlockSite
    {
        bool HasCodeBlock { get; }
        AstCodeBlock CodeBlock { get; }
        bool TrySetCodeBlock(AstCodeBlock? codeBlock);
    }

    public static class AstCodeBlockSiteExtension
    {
        public static void SetCodeBlock(this IAstCodeBlockSite codeBlockSite, AstCodeBlock codeBlock)
        {
            if (!codeBlockSite.TrySetCodeBlock(codeBlock))
                throw new InternalErrorException(
                    "CodeBlock is already set or null.");
        }
    }
}
