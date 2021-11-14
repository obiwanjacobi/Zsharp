# Zsharp

---

- `_ = fn()` results in an Assignment without a Variable...

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
- Honor the '#' in template parameters (#template or generic)
- AstTemplateInstanceStruct.Instantiate does not check field TypeReferences (or base type) to be replaced with template parameter values.
- Extract common from AstTypeDefinitionFunction and AstTypeReferenceFunction into static helper
- Can AstTemplateInstanceType derive from AstTypeDefinitionTemplate?
- Importing External modules does not find nested public types (Zsharp.Runtime.Conversion.Checked).
- Import External Modules also loads in all external-to-the-module referenced types (like System.Object).
    we need Module level dependency detection (and loading) and reference resolvement (SymbolTable).
- AstModuleManager should not return AstModuleExternal instances, but AstModule instances (move Aliases to AstModule).
- AstTypeReference.MakeCopy should not add to same symboltable.
- SymbolTable: different flavors of FindSymbol (FindDefintion) do not use the same algorithm to find symbols.
    Some do not check in Modules, others do not check for DotNames...
- .NET struct interop
- A faster file stream (ICharStream) based on `Span<T>` or Memory<T>`?
- Try to get template and generic parameter counts up-front so we don't have to mutate the type-name as the nodes are visited (currently).
- Rename parameters on references to arguments. Parameters are for defintions, arguments are the provided values to the parameters.
- Refactor function overload resolvement (ResolveDefinition) to use AstFunctionArgumentMap.
- Function with return type should give error if no expression of said type is returned from impl.
- Conversion Functions as Type in a template function do not work: `fn: <T>(p: U8): T; return T(p)`.
- External/imported generic parameters are not correctly marked (IsTemplate/IsTemplateParameter)
- Make external module loading lazy. Only load in the types of local referenced symbols. 
    Secondary dependencies could wait until really needed. Knowing the correct `usings` to emit would probably be enough?
    The ResolveDefinition phase would need to be aware of lazy loaded externals in order not to fail them when a secondairy dependecy's definition cannot be resolved.'
- try to map C# compiler messages to Z# source code locations.
- Import statement is a ModuleReference and can be registered in the SymbolTable as such.

---

## Bugs

- Namespace on AstSymbol is wrong for imported modules. 
    TBD: how module and import naming is going to work.

---

## Resources

- How C++ resolves a function call https://preshing.com/20210315/how-cpp-resolves-a-function-call/
