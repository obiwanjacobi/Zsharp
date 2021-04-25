# Zsharp

---

## TODO

- Check unit test coverage
- More tests on syntax errors (non-happy-flow).
- grammar: Nested templates does not parse: `Struct<Array<U8>>`.
- Move all Semantic errors to CheckRules.
- Need a (source file) Location class around Antlr Context.
- Identifier.CanonicalName for template (function) reference is wrong (type starts with lower case).
- Validate Template instantiation parameters with template definition.
- AstTemplateInstanceFunction has duplicate code with AstFunctionDefinitionImpl 
    and does not support partial templates (missing TemplateParameters).
- Expression Operators may need to increase the resulting type (U8 * U8 = U16).
    RetVal of operator overload determines resulting type.
    Type of target (variable) determines resulting type.
- Resolve Expression operators to (intrinsic or custom) function names.
- Add (custom) Type Conversion (Type names as function names)
    Conversions are simply functions with same name as (target) type and a self parameter of source type.
    Type Constructor functions are very similar - but without the self parameter.
- Add Custom Data Types (and export)
- Function Parameters are readonly variables. Add IsReadonly to AstVariableDefinition.
- `_ = fn()` results in an Assignment without a Variable...
- Can we move ParserRuleContext Context property to AstNode?
- Add SymbolEntrySite to TypeDefinition?
- May need to split Function Type ([template] params + retval) from function definition name.
    Lambda's?
- TypeReferences of built-in types are added to root SymbolTable that contains definition.
    Need to see if this is a problem when multiple files are compiled using the same root symbol table.
- Namespace on AstSymbolEntry is wrong for imported module functions. 
    It should be the name of the external module but is the name of the local symbol table it is refered in.
- Add AssemblyManager to ExternalModuleLoader to hide Cecil implementation use.

---

## Optimizations

- [IL] Struct field init is always done using a secondary instance - not always necessary (ctor).

## Resources

- How C++ resolves a function call https://preshing.com/20210315/how-cpp-resolves-a-function-call/
