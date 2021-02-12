using System;
using System.Collections.Generic;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionDefinitionImpl : AstFunctionDefinition,
        IAstCodeBlockSite, IAstSymbolTableSite, IAstTemplateSite
    {
        public AstFunctionDefinitionImpl(Function_defContext functionCtx)
        {
            Context = functionCtx;
        }

        private AstCodeBlock? _codeBlock;
        public AstCodeBlock? CodeBlock => _codeBlock;

        public bool TrySetCodeBlock(AstCodeBlock codeBlock)
        {
            if (this.SafeSetParent(ref _codeBlock, codeBlock))
            {
                _codeBlock!.Indent = Indent + 1;
                AddFunctionSymbols();
                return true;
            }
            return false;
        }

        public void SetCodeBlock(AstCodeBlock codeBlock)
        {
            if (!TrySetCodeBlock(codeBlock))
                throw new InvalidOperationException(
                    "CodeBlock is already set or null.");
        }

        public AstSymbolTable Symbols
        {
            get
            {
                var codeBlock = CodeBlock;
                if (codeBlock != null)
                {
                    return codeBlock.Symbols;
                }

                var site = ParentAs<IAstSymbolTableSite>() ??
                    throw new InvalidOperationException("Function Parent not a SymbolTable Site.");
                return site.Symbols;
            }
        }

        public AstSymbolEntry AddSymbol(string symbolName, AstSymbolKind kind, AstNode? node = null)
        {
            Ast.Guard(Symbols != null, "SymbolTable not set.");
            return Symbols!.AddSymbol(symbolName, kind, node);
        }

        // true when type is a template definition
        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameter> _templateParameters = new List<AstTemplateParameter>();
        public IEnumerable<AstTemplateParameter> TemplateParameters => _templateParameters;

        public void AddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (!TryAddTemplateParameter(templateParameter))
                throw new InvalidOperationException(
                    "TemplateParameter is already set or null.");
        }

        public bool TryAddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (templateParameter == null)
                return false;

            Symbols.Add(templateParameter);
            _templateParameters.Add(templateParameter);

            Identifier.TemplateParameterCount = _templateParameters.Count;
            return true;
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionDefinition(this);

        public override void VisitChildren(AstVisitor visitor)
        {
            base.VisitChildren(visitor);
            CodeBlock?.Accept(visitor);
        }

        /// <summary>
        /// Deferred registration of function parameter symbols in the codeblock's symbol table.
        /// </summary>
        private void AddFunctionSymbols()
        {
            foreach (var param in Parameters)
            {
                // function parameters are registered as variables
                var entry = Symbols.AddSymbol(param.Identifier.CanonicalName, AstSymbolKind.Variable, param);
                param.SetSymbol(entry);
            }
        }
    }
}
