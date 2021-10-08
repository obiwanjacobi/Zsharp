# Zsharp

---

- TypeReferences of built-in types are added to root SymbolTable that contains definition.
    Need to see if this is a problem when multiple files are compiled using the same root symbol table.
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
- Extract common from AstTypeDefinitionFunction and AstTypeReferenceFunction into static helper
- Importing External modules does not find nested public types (Zsharp.Runtime.Conversion.Checked).
- AstSymbolName: fix hybrid generic/template postfixes.
- GenericParameterReference is probably never going to be used: impossible to detect at parse time (only after resolve definition).
- SymbolTable: different flavors of FindSymbol (FindDefintion) do not use the same algorithm to find symbols.
    Some do not check in Modules, others do not check for DotNames...
- SymbolTable: store symbols in the scopes the occur in. Reference defintions from higher tables with their AstSymbol ref.
    Currently references are stored in the AstSymbol of the definition. By doing this we lose scope information on references.
    Work out if this is a good idea and how to manage it. How to retrieve all references to a symbol and its definition(s).
    Do AstSymbol instances have parent and children refs to other AstSymbol instances in othe SymbolTables?

- .NET struct interop

---

## Bugs

- Namespace on AstSymbol is wrong for imported modules. 
    TBD: how module and import naming is going to work.
- FunctionType identifier shows '()' even when it has params.

---

## Resources

- How C++ resolves a function call https://preshing.com/20210315/how-cpp-resolves-a-function-call/
