# Extensions

> `.NET` Code Attributes are objects, decorators are functions. How to make these meet? Also we may need to rethink some of the extension points.

> TBD: compiler extensions. plugins that supply code. Don't allow language extension, but do allow compile-time function extensions.

> A meta extension: implement a custom # tag. Register C++ code with the compiler to be called when the `#` tag is encountered. The extension either manipulates the Abstract Syntax Tree or emits Machine Code Representation.

> A 'code attribute' or 'decorator' extension. Annotated code that gets in the loop for generating the code for that scope. This, for instance, allows implementation of detailed entry and exit tracing and function interception etc.

```csharp
// Syntax??
{Decorator}
[[Decorator]]
decoratedFn: ()
```

> I like the `[[]]` syntax for it doesn't interfere with the object syntax `{}`.

```C#
// function code attributes
{CodeAttr}
myFunction: (p: U8) Bool

// multiple code attributes
{CodeAttr1, CodeAttr2}      // comma separated
myFunction: (p: U8) Bool

{p: CodeAttr}
myFunction: (p: U8) Bool

{: CodeAttr}                // : for retval
myFunction: (p: U8) Bool

// full set
{p: CodeAttr}               // order does not matter
{: CodeAttr}
{CodeAttr1, CodeAttr2}      // for function
myFunction: (p: U8) Bool

// concat
{p: CodeAttr, : CodeAttr, CodeAttr1, CodeAttr2}
myFunction: (p: U8) Bool
```

Code attributes with parameters:

```C#
// func(param)
{CodeAttr("Parameter")}
// multiple params
{CodeAttr(42, "42")}
```

## Decorator Functions

> Are decorators only 'used' at compile time?

Decorators are functions. For each type of Decorator a specific function with the same (decorator) name is created. If a decorator function is not found, it cannot be applied for that scenario.

The following code constructs can be decorated:

- Enum
  - Type
  - Field
- Structure
  - Type
  - Field
- Function
  - Type
  - Parameters
  - Return value
- Module

For each of these cases a specific decorator function signature is defined.

```C#
Enum: (self: EnumInfo)
EnumOption: (self: EnumOptionInfo)

Structure: (self: StructureInfo)
StructureField: (self: StructureFieldInfo)

Function: (self: FunctionInfo)
FunctionParameter: (self: FunctionParameterInfo)
FunctionReturn: (self: FunctionReturnInfo)

Module: (self: ModuleInfo)
```

The decorator function parameters must start with the specific self type in order to be used for the specific code construct. Additionally any number of extra parameters may be added to the decorator function. Matching decorator functions to a code construct is done purely on the self parameter.

The `self` structures that are defined mainly provide type information of the decorated code construct. The Info types also contain an entry point into the compiler for the decorator function to be able to modify the code compiled for the code construct.

Decorator functions are run at compile time and should therefor use the `#!` at the implementation definition to make sure the code is compile-time execution ready and are not compiled into the binary.

```C#
#! MyFnDecorator: (self: FunctionInfo, p: U8)
    ...

// the self parameter is passed implicitly
{MyFnDecorator(42)}   // mark it here too?
SomeDecoratedFn: ()
    ...
```
