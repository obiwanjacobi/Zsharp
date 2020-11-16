using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBuilderContext
    {
        private readonly Stack<AstNode> _current = new Stack<AstNode>();

        public AstBuilderContext(CompilerContext compilerContext, UInt32 indent = 0)
        {
            CompilerContext = compilerContext;
            Indent = indent;
        }

        public CompilerContext CompilerContext { get; }

        public IEnumerable<AstMessage> Errors => CompilerContext.Errors;
        public bool HasErrors => CompilerContext.HasErrors;

        public UInt32 Indent { get; private set; }

        public UInt32 CheckIndent(ParserRuleContext context, IndentContext indentCtx)
        {
            var indent = (UInt32)indentCtx.GetText().Length;
            Debug.Assert(indent < Int32.MaxValue);

            if (Indent == 0)
            {
                Indent = indent;
            }

            if (indent % Indent > 0)
            {
                CompilerContext.InvalidIndentation(context);
                // guess where it should go
                return (UInt32)Math.Round((double)indent / Indent);
            }

            return indent / Indent;
        }

        public T GetCurrent<T>()
            where T : class
        {
            T? p = null;

            foreach (var c in _current)
            {
                p = c as T;
                if (p != null)
                {
                    break;
                }
            }

            Ast.Guard(p, "GetCurrent() => null");
            return p!;
        }

        public void SetCurrent<T>(T current) where T : AstNode
            => _current.Push(current);

        public void RevertCurrent() => _current.Pop();

        public AstCodeBlock GetCodeBlock()
        {
            foreach (var c in _current)
            {
                if (c is AstCodeBlock p)
                {
                    return p;
                }
            }

            var site = GetCurrent<IAstCodeBlockSite>();
            site.ThrowIfCodeBlockNotSet();
            return site?.CodeBlock!;
        }

        public AstCodeBlock GetCodeBlock(UInt32 indent)
        {
            AstCodeBlock? p = null;

            foreach (var c in _current)
            {
                p = c as AstCodeBlock;
                if (p != null &&
                    p.Indent == indent)
                {
                    break;
                }
            }

            Ast.Guard(p != null, "GetCodeBlock => null");
            return p!;
        }

        public bool TryAddIdentifier(AstIdentifier identifier)
        {
            var namedObj = GetCurrent<IAstIdentifierSite>();
            if (namedObj != null)
            {
                return namedObj.TrySetIdentifier(identifier);
            }
            return false;
        }

        public void AddIdentifier(AstIdentifier identifier)
        {
            if (!TryAddIdentifier(identifier))
                throw new InvalidOperationException(
                    "Identifier is already set, null or no Site could be found.");
        }
    }
}