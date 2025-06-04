# Meta Programming

Allow to write Z# code that executes at compile time in order to shift the workload as much as possible to the compiler.

> `.NET` we can let the Z# compiler extract all the compile time code and generate an assembly behind the scenes during compilation. Then we can integrate calls to this assembly during further compilation.

---

## Intrinsic Attributes

The compiler will allow accessing intrinsic attributes of the compiled code. These attributes are constants whose value was determined by the compiler at compile time.

A special operator is used to access them: `#`

| Attribute | Description
|----|-----
| `#type` | Full type info.
| `#typeid` | A unique numerical type identifier.
| `#name` | The identifier (name).
| `#min` | The minimum value possible.
| `#max` | The maximum value possible.
| `#bits` | The number of bits for the type.
| `#count` | The number of elements.
| `#size` | The size in bytes the type takes up in memory.
| `#default` | Default value for the type.
| `#mask` | Mask for retrieving a bit field value.
| `#offset` | Byte offset from the start of a structure to a field.
| `#expr` | Expression that was passed as a parameter (tracing)
| `#caller` | name of the caller of the function (tracing)

- `#typeid` is a U16 hash value over (part of) the module name and type name (Not really needed for .NET).

- `#type` is only available in a compile-time function that is tagged with a `#!` (restriction not needed for .NET).

Not all types support all attributes. The compiler will give an error when the code accesses an attribute that is not supported by the type in question.

```C#
a = 42      // U8
a#size      // 1
a#bits      // 8
a#min       // 0
a#max       // 255
a#name      // 'a'

U8#size     // 1
U8#bits     // 8
U8#min      // 0
U8#max      // 255
U8#name     // 'U8'

Bit<3>#size // 1
Bit<3>#bits // 3
Bit<3>#min  // 0
Bit<3>#max  // 7
Bit<3>#name // 'Bit3'
```

> Perhaps have a very short version of `#$name` (no space) for it will be used most often. (`#` = compile-time, `$` = string)

```csharp
a := 42
#$a              // 'a'
#$U8             // 'U8'
#$MyFunction     // 'MyFunction'
```

---

## Compiler Directives

> TBD: Difference between Compiler-Directives and -Functions? Directives have no return value? Directives have no `()` around their parameters?

- Are directives, Compile-time functions and compiler functions all the same?
Directives and compiler functions are the same and custom compile-time functions are simply a way to add 'directives'...?

| Directive | Description
|--|--
| module | Assigning code to a module
| use | Importing code from a module
| pub | Making code public
| push | Pushing compiler configuration onto the (compile-time) stack
| pop | Popping compiler configuration from the (compile-time) stack
| enable | Enable a compiler feature (checks)
| ignore | Disable a compiler warning
| restore | re-enable previously disabled warnings
| align | align structure
| pack | pack structure

```C#
#ignore("CW3091") // ignore this warning
code_that_causes_CW3091
more_code_that_causes_CW3091
#restore("CW3091")  // back to normal

well_behaved_code
```

---

> TBD: Hints to the compiler how to compile code...

Syntax?

```csharp
#inline     // pragma (hint)
@inline()   // compiler function
[[inline]]  // extension/decorator
inlineFn: (p: U8): Bool -> p = 42
```

> The expression body function syntax `=` could be used for inline functions...

| Hint | Description
|--|--
| `inline` | duplicate function body at each call site (before optimization)
| `align x` | line struct up at a memory address that is a multiple of specified value

Use standard .NET code attributes for align? (I think those only work on .NET structs and not on classes)

> TBD: allow call-site inlineing? `b := #inline inlineFn(42)`

---

> TBD

Add a directive that provides the compilation context and the project settings (think Visual Studio macros).

| Project Setting | Description
|--|--
| `#project.targetPath` | Full path to the generated assembly.
| `#project.configuration` | Debug or Release

Compiler context are the current file, line, column, symbol etc functions/directives?

---

## Compiler Functions

A compilier-function call is prefixed with `#`.

A compiler function instructs the compiler to take some action. For instance turn off a compiler warning temporarily.

```C#
#ignore("CW3091")
code_that_causes_CW3091
more_code_that_causes_CW3091
#restore("CW3091")

well_behaved_code
```

The compiler supplies a set of functions that allows interaction with- and modification of the generated code. There is also contextual information available for formatting diagnostic messages.

| Function | Note
|--|--
| line() | the current source code line number
| col() | the current source code column number
| file() | the current source code file name
| module() | the current module the source code is part of
| name() | the name of current function or type being compiled
| location() | Fully formatted file/name/line/col text.

```csharp
msg := "Error in '{#file()}' at line {#line()}: {#name()} is invalid."
```

---

## Compile-Time Code

_Any_ Z# code can be executed at compile-time. By placing a `#!` in front of the function, the compiler knows it is not to be included in the binary.

> TBD: `#!` is considered for compiler-error directive. Something else: `#>`?

```C#
m = MyStruct
    ...

// this code can only run at compile time and is not included in the binary
#! compTimeFn: <T>(m: T)
    t = m#type
    t.name                      // 'MyStruct'
    loop f in t.fields
        "field: {f.name} of type {f.type.name}"

#compTimeFn(m)   // ok, call at compile time

// normal runtime function included in the binary
runtimeFn: <T>(m: T)
    ...

# runtimeFn(m) // call at compile time. Error if function body cannot be run at compile-time.
runtimeFn(m)      // call at runtime.

// alternate: use a #run directive to run any code at compile time.
#run
    runTimeFn(m)    // can give compile error
    compTimeFn(m)   // run after previous
```

---

## Type Information

The full Type information is available at compile-time and runtime as is provided by the dotnet metadata (reflection).

---

### Type Traits

> This needs to be redone.

> A Type-characteristic that defines a specific behavior.

- built-in traits
- custom traits

A lot of the `IsXxxx` properties on the .NET `Type` class are traits.

```csharp
fn: <T>()
    // special syntax?
    T::IsIntegral
    T::IsImmutable
    // traits as common template functions
    IsIntegral<T>()
    IsImmutable<T>()

// custom trait
#! IsTrait: <T>(): Bool
    ...
```

All traits are compile time functions. (You may want this at runtime?)

---

## Symbol Exists

> TBD: Test for field exists

```C#
MyStruct
    field1: U8

s := MyStruct
    ...

// does 'field1' exist at compile time?
if s?#field1
    ...

// does 'field1' exist at runtime (dynamic)?
if s?field1
    ...
```

---

> TBD: Transparently pass custom meta programs to roslyn source generators?

> TBD: Allow compile time code to build code using (perhaps) a builder pattern. TypeBuilder, FunctionBuilder and CodeBuilder.
> Building the structure is not the problem, but the function body is. How to add the actual code (CodeBuilder)?

> TBD: How could we expose the type information of the to-be-compiled code to meta/compile time code?

> TBD: Give compile time code access to the type info of source code and possibly extend the source code. The original code cannot be altered but it can be enhanced or extended. Pre and post processing of functions, wrapping of structs, implementing interfaces etc. See also [Extensions](extensions.md).
Also influencing type inferrence desicions the compiler makes for generic or template types.

> TBD: `#!` = compile error, `#?` compile warning, `#$` compile info.

```csharp
if (compile-time-condition)
    #! Hey, this is not suppose to happen.
```
