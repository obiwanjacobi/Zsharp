# Meta Programming

> TBD allow to write Z# code that executes at compile time in order to shift the workload as much as possible to compiler.

## Intrinsic Attributes

The compiler will allow accessing intrinsic attributes of the compiled code. These attributes are constants whose value was determined by the compiler at compile time and they do not take up any storage in the program.

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

> `#typeid` is a U16 hash value over (part of) the module name and type name.

> `#type` is only available in a compile-time function that is tagged with a `#!`.

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

> Perhaps have a very short version of `#name` for it will be used most often.

```csharp
a = 42
#a              // 'a'
#U8             // 'U8'
#MyFunction     // 'MyFunction'
```

(this may be conflicting with pragmas)

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

#! compTimeFn(m)  // ok, call at compile time
compTimeFn(m)     // error! cannot call a compile-time function at runtime. It is not in the binary.

// normal runtime function included in the binary
runtimeFn: <T>(m: T)
    ...

#! runtimeFn(m) // call at compile time. Error if function body cannot be run at compile-time.
runtimeFn(m)      // call at runtime.

// alternate: use a #run pragma to run any code at compile time.
# run
    runTimeFn(m)    // can give compile error
    compTimeFn(m)   // run after previous
```

> Some `#` compiler attributes may require the code to be `#!` compile time code. An example is the full `#type` information which is only available at compile time.

## Type Information

No type information is available at runtime other than the `#typeId` which can only be used as type identifier to compare equality or for use as a key in a map/table store.

Full type information is only available at compile time. Are there any scenarios that would really become a problem not having type info at runtime?

## Compiler Functions

The compiler supplies a set of functions that allows interaction with- and modification of the generated code. There is also contextual information available for formatting diagnostic messages.

| Function | Note
|--|--
| line() | the current source code line number
| file() | the current source code file name
| module() | the current module the source code is part of
| name() | the name of current function or type being compiled

```csharp
msg = "Error in '{#file()}' at line {#line()}: {#name()} is invalid."
```

> What syntax/operator to use? `#` is a pragma and not a compiler function. `!#` is compile-time execution but the name could collide with custom functions. Using `@` could be good alternative?

```csharp
msg = "Error in '{@file()}' at line {@line()}: {@name()} is invalid."
```

---

Hints to the compiler how to compile code...
Syntax?
```csharp
#inline     // pragma (hint)
@inline()   // compiler function
{inline}    // extension/decorator
inlineFn: (p: U8): Bool => p = 42
```

| Hint | Description
|--|--
| `inline` | duplicate function body at each call site
| `align x` | line struct up at a memory address that is a multiple of specified value

---

> TBD

Test for field exists

```C#
MyStruct
    field1: U8

s = MyStruct
    ...

if s?field1     // does 'field1' exist at compile time?
if s?#field1
    ...
```
