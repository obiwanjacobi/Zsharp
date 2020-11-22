using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstTypeDefinitionEnumOption : AstTypeFieldDefinition,
        IAstExpressionSite, IAstSymbolEntrySite
    {
        public AstTypeDefinitionEnumOption(Enum_option_defContext context)
            : base(AstNodeType.EnumOption)
        {
            Context = context;
        }

        private AstExpression? _expression;
        public AstExpression? Expression => _expression;

        public void SetExpression(AstExpression expression)
        {
            if (!TrySetExpression(expression))
                throw new InvalidOperationException(
                    "Expression was already set or null.");
        }

        public bool TrySetExpression(AstExpression expression)
            => this.SafeSetParent(ref _expression, expression);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public void SetSymbol(AstSymbolEntry symbolEntry)
        {
            if (!TrySetSymbol(symbolEntry))
                throw new InvalidOperationException("Symbol was already set or null.");
        }

        public bool TryResolve()
        {
            return _symbol?.Definition == this;
        }
    }
}