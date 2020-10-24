using System;

namespace Zsharp.AST
{
    public interface IAstCodeBlockSite
    {
        AstCodeBlock? CodeBlock { get; }
        bool TrySetCodeBlock(AstCodeBlock codeBlock);
        void SetCodeBlock(AstCodeBlock codeBlock);

        public void ThrowIfCodeBlockNotSet()
            => _ = CodeBlock ?? throw new InvalidOperationException("CodeBlock is not set.");
    }
}
