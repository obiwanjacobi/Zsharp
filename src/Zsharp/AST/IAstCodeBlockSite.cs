using System;

namespace Zsharp.AST
{
    public interface IAstCodeBlockSite
    {
        AstCodeBlock? CodeBlock { get; }
        bool TrySetCodeBlock(AstCodeBlock codeBlock);

        public void SetCodeBlock(AstCodeBlock codeBlock)
        {
            if (!TrySetCodeBlock(codeBlock))
                throw new InvalidOperationException(
                    "CodeBlock is already set or null.");
        }

        public void ThrowIfCodeBlockNotSet()
            => _ = CodeBlock ?? throw new InvalidOperationException("CodeBlock is not set.");
    }
}
