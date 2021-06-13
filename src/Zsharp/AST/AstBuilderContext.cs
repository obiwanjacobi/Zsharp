using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstBuilderContext : AstCurrentContext
    {
        public AstBuilderContext(CompilerContext compilerContext, UInt32 indent = 0)
        {
            CompilerContext = compilerContext;
            Indent = indent;
        }

        public CompilerContext CompilerContext { get; }

        public IEnumerable<AstMessage> Errors => CompilerContext.Errors;
        public bool HasErrors => CompilerContext.HasErrors;

        public UInt32 Indent { get; private set; }

        public UInt32 CheckIndent(ParserRuleContext context)
        {
            if (context is null)
                return 0;

            if (context is Function_defContext)
                return 0;

            var indentCtx = context switch
            {
                Statement_returnContext ctx => ctx.indent(),
                Statement_ifContext ctx => ctx.indent(),
                Statement_elseContext ctx => ctx.indent(),
                Statement_elseifContext ctx => ctx.indent(),
                Statement_breakContext ctx => ctx.indent(),
                Statement_continueContext ctx => ctx.indent(),
                Statement_loopContext ctx => ctx.indent(),
                Function_useContext ctx => ctx.indent(),
                Variable_defContext ctx => ctx.indent(),
                Struct_field_defContext ctx => ctx.indent(),
                Struct_field_initContext ctx => ctx.indent(),
                Enum_option_def_listlineContext ctx => ctx.indent(),
                Enum_option_defContext ctx => ctx.indent(),
                _ => null
            };

            if (indentCtx is null)
                return CheckIndent((ParserRuleContext)context.Parent);

            return CheckIndent(context, indentCtx);
        }

        public UInt32 CheckIndent(ParserRuleContext context, IndentContext indentCtx)
        {
            if (indentCtx is null)
                return 0;

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

        public AstCodeBlock GetCodeBlock(ParserRuleContext context)
        {
            var indent = CheckIndent(context);
            return GetCodeBlock(indent);
        }

        private AstCodeBlock GetCodeBlock(UInt32 indent)
        {
            AstCodeBlock? cb = TryGetCodeBlock(indent);
            Ast.Guard(cb, $"CodeBlock was not found for indent {indent}.");
            return cb!;
        }

        private AstCodeBlock? TryGetCodeBlock(UInt32 indent)
        {
            foreach (var c in Nodes)
            {
                if (c is AstCodeBlock cb &&
                    cb.Indent == indent)
                {
                    return cb;
                }
            }

            return null;
        }

        public bool TryAddIdentifier(AstIdentifier? identifier)
        {
            var namedObj = GetCurrent<IAstIdentifierSite>();
            if (namedObj is not null)
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