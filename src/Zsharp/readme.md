# Zsharp

---

## TODO

- Check unit test coverage
- More tests on syntax errors (non-happy-flow).
- grammar: Nested templates does not parse: `Struct<Array<U8>>`.
- Move all Semantic errors to CheckRules.
- Need a (source file) Location class around Antlr Context.
- Validate Template instantiation parameters with template definition.
- Add GenericParameters besides TemplateParameters (fix grammar #)
- AstTemplateInstanceFunction has duplicate code with AstFunctionDefinitionImpl 
    and does not support partial templates (missing TemplateParameters).
- AstFunctionParameterReference should not have a TypeReference for literal numeric.
    Function resolvement also takes (template) parameters (and its types) to work.
- Expression Operators may need to increase the resulting type (U8 * U8 = U16).
    RetVal of operator overload determines resulting type.
    Type of target (variable) determines resulting type.
- Resolve Expression operators to (intrinsic or custom) function names.
- Add (custom) Type Conversion (Type names as function names)
    Conversions are simply functions with same name as (target) type and a self parameter of source type.
    Type Constructor functions are very similar - but without the self parameter.
- Function Parameters are readonly variables. Add IsReadonly to AstVariableDefinition.
- `_ = fn()` results in an Assignment without a Variable...
- TypeReferences of built-in types are added to root SymbolTable that contains definition.
    Need to see if this is a problem when multiple files are compiled using the same root symbol table.
- Namespace on AstSymbolEntry is wrong for imported module functions. 
    It should be the name of the external module but is the name of the local symbol table it is refered in.
- Importing External modules does not find nested public classes (Zsharp.Runtime.Conversion.Checked).
- TrySetIdentifier should check if IdentifierType matches the AstNodeType.
- Rename all Enums from 'Type' to 'Kind'.
- Assign initial TypeReference to SymbolTable (entry) and MakeProxy for all other references?
- AstFunctionDefinitionIntrinsic.TrySetSymbol should not be called?
- Function Overloads handling: Currently based on function name are all in the same SymbolEntry.
    Function can have FunctionDefinition that is not correct.
- EmitFunctionTest - multiple usings 'using Zsharp.Runtime;' emitted.
- Extract common from AstTypeDefinitionFunction and AstTypeReferenceFunction into static helper
- Move Function.CreateSymbols impl to FunctionType
- How to match in incomplete FunctionReference to its FunctionDefinition?

---

## Optimizations

- [IL] Struct field init is always done using a secondary instance - not always necessary (ctor).

## Resources

- How C++ resolves a function call https://preshing.com/20210315/how-cpp-resolves-a-function-call/
