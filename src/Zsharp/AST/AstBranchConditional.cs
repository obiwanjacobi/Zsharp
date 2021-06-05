﻿using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBranchConditional : AstBranchExpression,
        IAstCodeBlockSite
    {
        public AstBranchConditional(Statement_ifContext context)
            : base(AstBranchType.Conditional)
        {
            Context = context;
        }
        public AstBranchConditional(Statement_elseContext context)
            : base(AstBranchType.Conditional)
        {
            Context = context;
        }
        public AstBranchConditional(Statement_elseifContext context)
            : base(AstBranchType.Conditional)
        {
            Context = context;
        }

        internal AstBranchConditional(ParserRuleContext context)
            : base(AstBranchType.Conditional)
        {
            Context = context;
        }

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool TrySetCodeBlock(AstCodeBlock? codeBlock)
        {
            if (this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                codeBlock!.Indent = Indent + 1;
                return true;
            }
            return false;
        }

        private AstBranchConditional? _subBranch;
        public AstBranchConditional? SubBranch => _subBranch;

        public bool HasSubBranch => _subBranch is not null;

        public bool TryAddSubBranch(AstBranchConditional? subBranch)
        {
            if (this.SafeSetParent(ref _subBranch, subBranch))
            {
                _subBranch!.Indent = Indent;
                return true;
            }
            return false;
        }

        public void AddSubBranch(AstBranchConditional subBranch)
        {
            if (!TryAddSubBranch(subBranch))
                throw new InternalErrorException(
                    "SubBranch is already set or null.");
        }

        public AstBranchConditional LastSubBranch() => HasSubBranch ? _subBranch!.LastSubBranch() : this;

        public override void Accept(AstVisitor visitor) => visitor.VisitBranchConditional(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);
            if (HasSubBranch)
            {
                _subBranch!.Accept(visitor);
            }
        }
    }
}
