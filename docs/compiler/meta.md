# Meta Programming

Allow to write Z# code that executes at compile time in order to shift the workload as much as possible to the compiler.

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

> Perhaps have a very short version of `#name` (no space) for it will be used most often.

```csharp
a = 42
#a              // 'a'
#U8             // 'U8'
#MyFunction     // 'MyFunction'
```

This may be conflicting with pragmas. Symbol must be in scope, resolvement will be up to parent scopes as usual.

## Pragmas

A pragma is a directive that instructs the compiler to take some action. For instance turn off a compiler warning temporarily.

A pragma is prefixed with: `#` that starts at the indent level of the current scope. It also starts a new scope.

> TBD: A new scope is not always practical. How to make the dev choose?

> Is the space after `#` mandatory or optional?

```C#
# ignore("CE3091")    // compiler fn call - starts a scope!
    code_that_causes_CE3091
    more_code_that_causes_CE3091
well_behaved_code
```

> A `#` symbol not at the start of the current scope indent position, does not start a new scope.

A scope level 'options' pragma that executes all pragmas for that scope.

```C#
# push()
    enable(Checks.Bounds)
    enable(Checks.Overflow)

...     // rest of code file
```

At the end of the scope the options are `pop`ed automatically and previous settings are restored. Before the end of the scope the `pop` pragma can be used to restore the settings manually.

```csharp
# pop() _
```

We can also use `enable` or `disable` that automatically push the previous state and a `restore` pragma to restore it afterwards.

| Pragma | Description
|--|--
| module | Assigning code to a module
| import | Importing code from a module
| export | Making code public
| push | Pushing compiler configuration onto the (compile-time) stack
| pop | Popping compiler configuration from the (compile-time) stack
| enable | Enable a compiler feature (checks)
| ignore | Disable a compiler warning

---

> TBD

Add a pragma that provides the compilation context and the project settings (think Visual Studio macros).

Project Setting | Description
--|--
`#project.targetPath` | Full path to the generated assembly.
`#project.configuration` | Debug or Release

---

Compiler context are the current file, line, column, symbol etc functions?

---

## Compile-Time Code

_Any_ Z# code can be executed at compile-time. By placing a `#!` in front of the function, the compiler knows it is not to be included in the binary. The use of this symbol does not introduce an extra scope.

```C#
m = MyStruct
    ...

// this code can only run at compile time and is not included in the binary
#! compTimeFn: <T>(m: T)
    t = m#type
    t.name                      // 'MyStruct'
    loop f in t.fields
        "field: {f.name} of type {f.type.name}"

# compTimeFn(m)   // ok, call at compile time

// normal runtime function included in the binary
runtimeFn: <T>(m: T)
    ...

# runtimeFn(m) // call at compile time. Error if function body cannot be run at compile-time.
runtimeFn(m)      // call at runtime.

// alternate: use a #run pragma to run any code at compile time.
# run
    runTimeFn(m)    // can give compile error
    compTimeFn(m)   // run after previous
```

> Some `#` compiler attributes may require the code to be `#!` compile time code. An example is the full `#type` information which is only available at compile time.

Not true anymore for .NET.

---

## Compiler Functions

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
msg = "Error in '{#file()}' at line {#line()}: {#name()} is invalid."
```

> What syntax/operator to use? `#` is a pragma and not a compiler function. Is there a difference between pragma's and compile time functions? `!#` is compile-time execution (declaration) but the name could collide with custom functions. Using `@` could be good alternative?

```csharp
msg = "Error in '{@file()}' at line {@line()}: {@name()} is invalid."
```

---

## Type Information

> This is not the case now we target .NET. However I think type info for structs should still be restricted. Only interface types should be available.

No type information is available at runtime other than the `#typeId` which can only be used as type identifier to compare equality or for use as a key in a map/table store.

Full type information is only available at compile time. Are there any scenarios that would really become a problem not having type info at runtime?

### Type Traits

> A characteristic that defines a specific behavior.

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

Hints to the compiler how to compile code...
Syntax?

```csharp
#inline     // pragma (hint)
@inline()   // compiler function
{inline}    // extension/decorator
inlineFn: (p: U8): Bool => p = 42
```

> The function alias syntax `=` could be used for inline functions...

| Hint | Description
|--|--
| `inline` | duplicate function body at each call site (before optimization)
| `align x` | line struct up at a memory address that is a multiple of specified value

Use standard .NET code attributes for align? (I think those only work on .NET structs and not on classes)

---

> TBD

Test for field exists

```C#
MyStruct
    field1: U8

s = MyStruct
    ...

// does 'field1' exist at compile time?
if s?#field1
    ...

// does 'field1' exist at runtime (dynamic)?
if s?field1
    ...
```

---

> TBD

Are pragma's, Compile-time functions and compiler functions all the same?

Pragma's and compiler functions are the same and custom compile-time functions are simply a way to add 'pragma's'...?

Invocation always uses `#` to indicate the compile-time nature of the result (you can `#`-call runtime functions if they can be evaluated at compile time). Compile-time functions are declared with `#!` to indicate they should not end up in the binary. But that could also just be a `#`...

Problem here is that some pragma's start a new scope and others don't. How to fix that?
