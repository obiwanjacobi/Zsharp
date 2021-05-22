using Antlr4.Runtime;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionParameterDefinition : AstFunctionParameter, IAstSymbolEntrySite
    {
        public AstFunctionParameterDefinition(Function_parameterContext context)
        {
            Context = context;
            IsSelf = false;
        }

        public AstFunctionParameterDefinition(Function_parameter_selfContext context)
        {
            Context = context;
            IsSelf = true;
        }

        public AstFunctionParameterDefinition(AstIdentifier identifier)
        {
            this.SetIdentifier(identifier);
            IsSelf = identifier.IsEqual(AstIdentifierIntrinsic.Self);
        }

        public ParserRuleContext? Context { get; }

        public bool IsSelf { get; }

        public override void Accept(AstVisitor visitor)
            => visitor.VisitFunctionParameterDefinition(this);

        private AstSymbolEntry? _symbol;
        public AstSymbolEntry? Symbol => _symbol;

        public bool TrySetSymbol(AstSymbolEntry? symbolEntry)
            => Ast.SafeSet(ref _symbol, symbolEntry);

        public bool TryResolve()
        {
            var entry = Symbol?.SymbolTable.ResolveDefinition(Symbol);
            if (entry != null)
            {
                _symbol = entry;
                return true;
            }
            return false;
        }

        public static AstFunctionParameterDefinition Create(string name, AstTypeDefinition astType)
        {
            var identifier = new AstIdentifier(name, AstIdentifierType.Parameter);
            var param = new AstFunctionParameterDefinition(identifier);
            param.SetTypeReference(AstTypeReference.From(astType));
            return param;
        }
    }
}
