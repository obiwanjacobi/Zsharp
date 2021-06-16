# Zsharp

---

- TypeReferences of built-in types are added to root SymbolTable that contains definition.
    Need to see if this is a problem when multiple files are compiled using the same root symbol table.

## TODO

- Check unit test coverage
- More tests on syntax errors (non-happy-flow).
- grammar: Nested templates does not parse: `Struct<Array<U8>>`.
- Move all Semantic errors to CheckRules.
- Need a (source file) Location class around Antlr Context.
- Validate Template instantiation parameters with template definition.
- Add GenericParameters besides TemplateParameters (fix grammar #)
- Expression Operators may need to increase the resulting type (U8 * U8 = U16).
    RetVal of operator overload determines resulting type.
    Type of target (variable) determines resulting type.
- Resolve Expression operators to (intrinsic or custom) function names.
- Add (custom) Type Conversion (Type names as function names)
    Conversions are simply functions with same name as (target) type and a self parameter of source type.
    Type Constructor functions are very similar - but without the self parameter.
- `_ = fn()` results in an Assignment without a Variable...
- AstTemplateInstanceFunction has duplicate code with AstFunctionDefinitionImpl 
    and does not support partial templates (missing TemplateParameters).
- Extract common from AstTypeDefinitionFunction and AstTypeReferenceFunction into static helper
- Importing External modules does not find nested public classes (Zsharp.Runtime.Conversion.Checked).
- Check if ResolveDefinition for variable expression operand is still 'optimal'.
    It looks a bit convoluted.

---

## Bugs

- Namespace on AstSymbol is wrong for imported modules. 
    TBD: how module and import naming is going to work.

---

## Resources

- How C++ resolves a function call https://preshing.com/20210315/how-cpp-resolves-a-function-call/
