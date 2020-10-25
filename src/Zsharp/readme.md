# Zsharp

## TODO

- SymbolTable extensions
    Add(AstVariable) =>
        var entry = symbols.AddSymbol(variable, AstSymbolKind.Variable, variable);
        success = variable.SetSymbol(entry);
    Add(AstType)
    etc
- Check unit test coverage
- change grammar for Variable_def_typed_init so it can reuse Variable_def_typed (as child)?
- Variable/Assignment. Always use assignment for all variable decls/refs? Variable no longer a CodeBlockItem.
- rethink ResolveSymbols/ResolveTypes. Do these need to be separate or integrated?

Numeric (infer type => TypeReference) | VariableReference (TypeReference)
Operand (^ TypeReference)
Expression (LHS/RHS ^ TypeReference)
Assignment (^ Expression) / (Variable)
Assignment.Variable => VariableReference / VariableDefinition ??
