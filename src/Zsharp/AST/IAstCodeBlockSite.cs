namespace Zsharp.AST
{
    public interface IAstCodeBlockSite
    {
        AstCodeBlock? CodeBlock { get; }
        bool SetCodeBlock(AstCodeBlock codeBlock);
    }
}
