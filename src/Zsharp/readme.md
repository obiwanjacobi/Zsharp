# Zsharp

## TODO

- Dont let connection objects own their TypeReferences.
    The TypeReference is owned by the most logical source code reference (context).
    AstExpressionOperand should not own (a copy of) the TypeReference of the actual operand.
    AstAssignment should not have a TypeReference ?
    AstExpression should not own its TypeReference
- What about Identity?
    Lots of duplication. Perhaps let the Symbol own the Identity and all reference from it?
    Does VisitIdentity requere access to its parent? (Identity Extension AddSymbol does)
    Or let all definition objects (function, parameter, variable, type) own (parent) the identifiers.
- SymbolTable extensions
    Add(AstVariable) =>
        var entry = symbols.AddSymbol(variable, AstSymbolKind.Variable, variable);
        success = variable.SetSymbol(entry);
    Add(AstType)
    etc
- change grammar for Variable_def_typed_init so it can reuse Variable_def_typed (as child)?
- Variable/Assignment. Always use assignment for all variable decls/refs? Variable no longer a CodeBlockItem.


Numeric (infer type => TypeReference) | VariableReference (TypeReference)
Operand (^ TypeReference)
Expression (LHS/RHS ^ TypeReference)
Assignment (^ Expression) / (Variable)
Assignment.Variable => VariableReference / VariableDefinition ??
