using Maja.Compiler.Syntax;

namespace Maja.Compiler.Compilation
{
    public sealed class SemanticModel
    {
        private readonly Compilation _compilation;

        internal SemanticModel(Compilation compilation, SyntaxTree syntaxTree)
        {
            _compilation = compilation;
            SyntaxTree = syntaxTree;
        }

        public SyntaxTree SyntaxTree { get; }

        public Symbol.Symbol? GetDeclaredSymbol(MemberDeclarationSyntax declarationSyntax)
        {
            return declarationSyntax switch
            {
                FunctionDeclarationSyntax funcDecl => GetDeclaredSymbol(funcDecl),
                TypeDeclarationSyntax typeDecl => GetDeclaredSymbol(typeDecl),
                VariableDeclarationSyntax varDecl => GetDeclaredSymbol(varDecl),
                _ => null
            };
        }

        //public Symbol.Symbol GetDeclaredSymbol(TypeDeclarationSyntax typeDeclaration)
        //{

        //}

        //public Symbol.Symbol GetDeclaredSymbol(FunctionDeclarationSyntax functionDelcaration)
        //{

        //}

        //public Symbol.Symbol GetDeclaredSymbol(VariableDeclarationSyntax variableDeclaration)
        //{

        //}
    }
}