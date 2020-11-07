using Zsharp.AST;

namespace Zsharp.Semantics
{
    /// <summary>
    /// Resolves variable, function and parameter name references to their definitions.
    /// </summary>
    /// <remarks>See AstSymbolTable.</remarks>
    public class ResolveSymbols : AstVisitorWithSymbols
    {
        private readonly AstErrorSite _errorSite;

        public ResolveSymbols(AstErrorSite errorSite)
        {
            _errorSite = errorSite;
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            VisitChildren(assign);

            if (assign.Variable is AstVariableReference varRef &&
                varRef.VariableDefinition == null)
            {
                var entry = varRef.Symbol;

                // variable.TypeReference can be null
                // VariableDefinition should get its type references from ResolveTypes.
                var varDef = new AstVariableDefinition(varRef.TypeReference);
                varDef.SetIdentifier(varRef.Identifier!);
                varDef.SetSymbol(entry!);
                entry!.PromoteToDefinition(varDef, varRef);

                assign.SetVariableDefinition(varDef);
            }
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            if (!variable.HasDefinition)
            {
                var entry = variable.Symbol;
                Ast.Guard(entry, "Variable has no Symbol.");

                var success = variable.TryResolve();

                if (!success &&
                    variable.ParentAs<AstAssignment>() == null)
                {
                    _errorSite.UndefinedVariable(variable);
                }
            }
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            if (function.FunctionDefinition == null)
            {
                var success = function.TryResolve();

                if (!success)
                {
                    _errorSite.UndefinedFunction(function);
                }
            }
        }
    }
}
