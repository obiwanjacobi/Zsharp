using Zsharp.AST;

namespace Zsharp.Semantics
{
    /// <summary>
    /// Resolves variable, function and parameter name references to their definitions.
    /// </summary>
    /// <remarks>See AstSymbolTable.</remarks>
    public class ResolveSymbols : AstVisitorWithSymbols
    {
        public void Apply(AstFile file) => VisitFile(file);

        public override void VisitAssignment(AstAssignment assign)
        {
            base.VisitAssignment(assign);

            if (assign.Variable is AstVariableReference varRef &&
                varRef.VariableDefinition != null &&
                varRef.VariableDefinition.Parent == null)
            {
                // catch newly created inferred definition
                assign.SetVariableDefinition(varRef.VariableDefinition);
            }
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            if (!variable.HasDefinition)
            {
                AstVariableDefinition? varDef = null;

                var entry = variable.Symbol;
                if (entry == null)
                {
                    entry = SymbolTable.FindEntry(variable.Identifier.Name, AstSymbolKind.Variable);
                    if (entry != null)
                    {
                        varDef = entry!.DefinitionAs<AstVariableDefinition>();
                    }
                }
                else
                {
                    varDef = entry.DefinitionAs<AstVariableDefinition>();
                }

                // variable
                if (entry != null)
                {
                    if (varDef == null)
                    {
                        // variable.TypeReference can be null
                        // VariableDefinition should get its type references from ResolveTypes.
                        varDef = new AstVariableDefinition(variable.TypeReference);
                        varDef.SetIdentifier(variable.Identifier.Clone());
                        varDef.SetSymbol(entry);
                        entry.PromoteToDefinition(varDef, variable);
                    }
                    variable.SetVariableDefinition(varDef);
                }
                else    // parameter
                {
                    entry = SymbolTable.FindEntry(variable.Identifier.Name, AstSymbolKind.Parameter);
                    // TODO: syntax error
                    Ast.Guard(entry, "No Symbol found for variable reference name.");

                    var paramDef = entry!.DefinitionAs<AstFunctionParameter>();
                    Ast.Guard(paramDef, "No Parameter Definition found for variable reference.");

                    variable.SetVariableDefinition(paramDef!);
                    entry.AddNode(variable);
                }
            }
        }
    }
}
