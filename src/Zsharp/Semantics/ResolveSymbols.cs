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
            base.VisitAssignment(assign);

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

                var varDef = entry!.DefinitionAs<AstVariableDefinition>();
                if (varDef == null)
                {
                    var paramDef = entry.DefinitionAs<AstFunctionParameter>();
                    if (paramDef != null)
                    {
                        variable.SetParameterDefinition(paramDef);
                    }
                }
                else
                {
                    variable.SetVariableDefinition(varDef);
                }
            }
        }
    }
}
