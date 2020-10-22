using Zsharp.AST;

namespace Zsharp.Semantics
{
    /// <summary>
    /// Resolves variable, function and parameter name references to their definitions.
    /// </summary>
    /// <remarks>See AstSymbolTable.</remarks>
    public class ResolveSymbols : AstVisitor
    {
        public void Apply(AstFile file) => VisitFile(file);

        public override void VisitVariableReference(AstVariableReference variable)
        {
            if (variable.VariableDefinition == null)
            {
                var entry = variable.Symbol;
                var varDef = entry.GetDefinition<AstVariableDefinition>();
                if (varDef == null)
                {
                    varDef = new AstVariableDefinition(variable.TypeReference);
                    entry.PromoteToDefinition(variable);
                }
                variable.SetVariableDefinition(varDef);
            }
        }
    }
}
