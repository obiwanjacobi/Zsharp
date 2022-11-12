using Maja.Compiler.External;
using Maja.Compiler.IR;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.Compilation
{
    public sealed class CompilationModel
    {
        private readonly Compilation _compilation;
        private readonly IrProgram _program;

        internal CompilationModel(Compilation compilation, SyntaxTree syntaxTree, IExternalModuleLoader moduleLoader)
        {
            _compilation = compilation;
            SyntaxTree = syntaxTree;
            _program = IrBuilder.Program(syntaxTree, moduleLoader);
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