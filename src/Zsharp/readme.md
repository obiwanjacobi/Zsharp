# Zsharp

---

- `_ = fn()` results in an Assignment without a Variable...
- External Function definition is not a codeblockline (indent - from AstFunction).

## TODO

- Check unit test coverage
- More tests on syntax errors (non-happy-flow).
- Move all Semantic errors to CheckRules.
- Need a (source file) Location class around Antlr Context.
- Validate Template instantiation parameters with template definition constraints.
- Expression Operators may need to increase the resulting type (U8 * U8 = U16).
    RetVal of operator overload determines resulting type.
    Type of target (variable) determines resulting type.
- Resolve Expression operators to (intrinsic or custom) function names.
- Add (custom) Type Conversion (Type names as function names)
    Conversions are simply functions with same name as (target) type and a self parameter of source type.
    Type Constructor functions are very similar - but without the self parameter.
- AstTemplateInstanceFunction has duplicate code with AstFunctionDefinitionImpl 
    and does not support partial templates (missing TemplateParameters).
- AstTemplateInstanceStruct.Instantiate does not check field TypeReferences (or base type) to be replaced with template parameter values.
- Extract common from AstTypeDefinitionFunction and AstTypeReferenceFunction into static helper
- Can AstTemplateInstanceType derive from AstTypeDefinitionTemplate?
- Importing External modules does not find nested public types (Zsharp.Runtime.Conversion.Checked).
- Import External Modules also loads in all external-to-the-module referenced types (like System.Object).
    we need Module level dependency detection (and loading) and reference resolvement (SymbolTable).
- AstModuleManager should not return AstModuleExternal instances, but AstModule instances (move Aliases to AstModule).
- AstFunctionDefinitionExternal has no FunctionType (and constructs on an empty AstTypeDefinitionFunction?).
- AstTypeReference.MakeCopy should not add to same symboltable.
- AstSymbolName: fix hybrid generic/template postfixes.
- SymbolTable: different flavors of FindSymbol (FindDefintion) do not use the same algorithm to find symbols.
    Some do not check in Modules, others do not check for DotNames...
- .NET struct interop
- A faster file stream (ICharStream) based on `Span<T>` or Memory<T>`?
- Try to get template and generic parameter counts up-front so we don't have to mutate the type-name as the nodes are visited (currently).
- Rename parameters on references to arguments. Parameters are for defintions, arguments are the provided values to the parameters.

---

## Bugs

- ResolveDefinition.FindTemplateDefinition needs resolving based on number of template/generic params.
- Namespace on AstSymbol is wrong for imported modules. 
    TBD: how module and import naming is going to work.
- FunctionType identifier shows '()' even when it has params.

---

## Resources

- How C++ resolves a function call https://preshing.com/20210315/how-cpp-resolves-a-function-call/
