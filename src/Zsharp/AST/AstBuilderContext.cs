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

        public AstBuilderContext(UInt32 indent)
        {
            Indent = indent;
            IntrinsicSymbols = CreateIntrinsicSymbols();
        }

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
                AddError(context, AstError.IndentationInvalid);
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

        public void SetCurrent<T>(T current)
            where T : AstNode
        {
            _current.Push(current);
        }

        public void RevertCurrent()
        {
            _current.Pop();
        }

        public AstCodeBlock? GetCodeBlock()
        {
            foreach (var c in _current)
            {
                var p = c as AstCodeBlock;
                if (p != null)
                {
                    return p;
                }
            }

            var site = GetCurrent<IAstCodeBlockSite>();
            return site?.CodeBlock;
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

        public bool AddIdentifier(AstIdentifier identifier)
        {
            var namedObj = GetCurrent<IAstIdentifierSite>();
            if (namedObj != null)
            {
                return namedObj.SetIdentifier(identifier);
            }
            return false;
        }

        private readonly List<AstError> _errors = new List<AstError>();
        public IEnumerable<AstError> Errors => _errors;

        public bool HasErrors => _errors.Count > 0;

        public AstError AddError(ParserRuleContext context, string text)
        {
            var error = new AstError(context)
            {
                Text = text
            };
            _errors.Add(error);
            return error;
        }

        public AstSymbolTable IntrinsicSymbols { get; }

        private AstSymbolTable CreateIntrinsicSymbols()
        {
            var symbols = new AstSymbolTable();
            AstTypeIntrinsic.AddAll(symbols);
            return symbols;
        }
    }
}