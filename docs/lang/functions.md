# Functions

A function is a block of code that can be called from another function. The function can take parameters and optionally return a value of any type.

A program starts when its entry point (main) function is called.

A function has a name that identifies it uniquely. See [Identifiers](../lexical/identifiers.md).

Here is the full syntax of a function:

```txt
functionName: InterfaceName [captures]<template>(parameters): returnType
```

`functionName`: (optional) All functions except lambdas (anonymous -inline- functions) have a name. Standard [Identifier](../lexical/identifiers.md) rules apply. Only lambdas do not need a name because they are declared inline at the location they're used.

`InterfaceName`: (optional) When basing the type of the function of off a function interface. In that case the declaration of the `types`, `parameters` and `returnType` may be repeated for readability. The function interface name is a type name so it starts with an upper case first letter.

`[captures]`: (optional) This captures variables external to the function for its execution. For 'normal' function these would be global variables. For lambda's these could be function-local variables that are used inside the lambda. Captures are only specified on function declarations (implementation), not on function (type) interfaces or at the call site. The name of the capture refers to a variable (or parameter) and that name is also used in the function's implementation. Comma separated.

`<template>`: (optional) Template or Generic Parameters that the function uses in its implementation. Type parameters must start with a upper case first letter. Template parameters are prefixed with a `#`. Comma separated.

`(parameters)`: (optional) By-value parameters the function acts on. Comma separated.

`returnType`: (optional) The Type of the function result. `Void` if not specified.

> TBD: we may be able to drop the '`:`' before the return type - or use `->`?.

```csharp
// not showing implementation
fn: ()
fn: (): U8
fn: (p: U8)
fn: (p: U8): U8

fn: <#T>(): T
fn: <#T>(p: T)
fn: <#T, #R>(p: T): R
fn: <G>(): G
fn: <G>(p: G)
fn: <G, R>(p: G): R

// capture on fn impl
fn: |c|<T>(p: T): Bool        // by val
fn: |c.Ref()|<T>(p: T): Bool  // by ref

fn: InterfaceName
// interface impl with capture
fn: InterfaceName |c|
// repeated function type decl with capture
fn: InterfaceName |c.Ref()|<T>(p: T): Bool

// TBD
// function type from object interface
fn: InterfaceName.funcName
// function name and type from object interface
fn: InterfaceName&  // syntax?
```

> How to differentiate `fn: InterfaceName` from struct definition? => Has no field names.

> TBD: change the syntax to reflect how we use variables? `fn: (p: U8): U8 = ...`. That would mean only adding the `=`. If we have a constant/immutable assignment... Perhaps also change the location of the capture? capture is part of the code block?

```csharp
fn: () =    // like variable syntax?
    ...

x: U8 = 42
fn: () =
    |x|         // capture just like normal code?
        ...     // extra indent...

// infer function type?
// TBD: lambda like syntax (arrow function)
fn := (p) //: Bool -> inferred from return statement
    return (p = 42) // p: U8 -> inferred from '42'
```

---

## Parameters

There is no other way of passing parameters to functions than by value. That means, that the parameter value is _copied_ from the caller site into the context of the function.

> TBD: What about dotnet _out_ and _ref_ parameters?

That also means that if a parameter is to be passed by reference, an explicit pointer to that value has to be constructed and passed to the function.

> The compiler can still use an immutable reference for optimizations. The by-value model is how you should think about it.

> `Ptr<T>` will probably be targeted toward representing an unsafe C# memory pointer. `Ref<T>` will be used to represent a managed reference (not the reference type).

```C#
byref: (ptr: Ptr<U8>)     // pointer as by-ref parameter
    ...

v := 42
byref(v.Ptr())            // call with ptr to value
```

Function Pointer as function argument syntax:

```csharp
filter: (predicate: (p: U8): Bool)
    ...
filter: (predicate: Fn<(p: U8): Bool>)
    ...
filter: (predicate: Fn<U8, Bool>)
    ...
```

> TBD: it would be nice to be able to see if a variable or parameter was an literal value. Then specific logic could be applied in these cases. For instance, when a parameter is a literal, the result of the function could be made immutable?
But that should probably be an overload of the function. And then we could use a specific type something like `Lit<T>` for the parameter type.

```csharp
// normal function
fn: (p: U8): U8
    ...

// overload for mutable (out) argument
fn: (p: Mut<U8>): U8
    ...

// overload for constant/literal argument
fn: (p: Lit<U8>): U8
    ...

u: U8 = 42
v := fn(u)   // calls normal function

w: Mut<U8> = 42
x := fn(w)   // calls mutable overload

z := fn(42)  // calls the constant overload
```

- Require specifying the parameter name when a literal is used?

> TBD: Omit parameter type when they multiple parameters share the same type?

```csharp
// p1: U8, p2: U8, p3: Str
add: (p1, p2: U8, p3: Str): U16
    ...
```

---

### Optional Parameters

Optional function parameters can be specified using the optional symbol `?` or `Opt<T>`.

```C#
hasParam: (p: Opt<U8>): Bool
hasParam: (p: U8?): Bool
    return p ? true : false
    return p    // error! implicit cast not allowed
    return p?   // but there is a special syntax
```

> TBD: cancel calling a function when parameter is not available? (`Opt<T>` chaining)

```csharp
fn: (p: U8)
    ...

v: Opt<U8>  // not set

// do not call function if v is not set.
fn(v?)  // conflicts with bool operator '?'
fn(?v)  // control on which param
?fn(v)  // any and all params

// shorthand for
if v?
    fn(v)
```

---

### Default Parameter Values

Assign a default value to a function parameter.
Function parameters with defaults cannot be in front of parameters without a default value assigned.

```csharp
fn: (p: U8, q: U8 = 101)
    ...

fn(42)  // fn(42, 101)
```

> TBD

Use other parameter names as defaults

```csharp
fn: (p: U8 = 42, q: U8 = p)
    ...

// reference capture as default value
fn: |x|(p: U8 = 42, q: U8 = x)
    ...
```

> TBD

Just like with variables omit the type?

```csharp
fn: (p = 42, s = "42")
    ...
fn: (p := 42, s := "42")
    ...
```

---

### Named Parameter

Function Parameters can be specified by name at the call site.

```C#
namedFn: (p: U8, p2: U16)
    ...

namedFn(p = 42, p2 = 0x4242)    // ok, both named
namedFn(p2 = 0x4242, p = 42)    // ok, out of order, but named
namedFn(42, p2 = 0x4242)        // ok, p in order, rest named
namedFn(0x4242, p = 42)         // ok, unnamed is only one left
```

---

### Variable number of parameters

Not really supported but you can fake it with an Array: all of same type. For .NET interop an `Any` (object) type is available.

> TBD `Params<T>` type as a way to specify an array as parameters... (C#: `params object[] p`)

```C#
varFunc: (p: U8, varP: Array<Any>)
    ...
varFuncTempl: <#T>(p: U8, varP: Array<T>)
    ...
varFuncParams: (p: U8, varP: Params<Any>)
    ...
varFuncSpread: <#T>(p: U8, ...varP: Array<T>)
    ...

// different types allowed for 'Any'
varFunc(42, (1, 3.14, "42"))

// same types (or derived) for template
varFuncTempl(42, (1, 2, 3, 4, 5, 6))

// different types as parameters
varFunc(42, 1, 3.14, "42")
```

---

### Mutable Parameters

Gives the caller an indication that the (out) parameter will be changed.

```csharp
immFn: (p: Mut<U8>)
// shorter using type operators
immFn: (p: ^U8)
```

---

### Out and ByRef Parameters

There are several wrapper types that can be used for this.

```C#
// in parameter
fn: (p: U8)
    ...
// mutable parameter
fn: (p: Mut<U8>)
    ...
// out parameter
fn: (p: Out<U8>)
    ...
// in/out parameter
fn: (p: Ref<U8>)
    ...
```

> TBD: If we assume immutable types by default and have a `Mut<T>` for mutable data, then a `Mut<T>` would represent an out parameter, `Ref<T>` a ref parameter and a normal type as a constant parameter.

Alternative for out parameters is to return it as a return value.
If multiple out parameter and/or return values exist, the return type becomes a tuple.

```csharp
// import
// C# bool TryParse(string, out int)

result := TryParse("42")
b, i := result
// b = true
// i = 42

// - or ? -
option := TryParse("42")
// option.Value = 42
// otherwise 'nothing'
```

```csharp
// immutable / constant parameter
fn: (p: U8)
    ...
// out parameter
fn: (p: Mut<U8>)
    ...
// ref (in/out) parameter
fn: (p: Ref<U8>)
    ...
```

That would make `Ref<T>` be also a `Mut<T>`?
How does `ref` differ between Value and Reference types?

---

### Illegal Parameter Types

> TBD: playing with the idea of making `Bool` an illegal parameter type for an exported (public) function.

```csharp
illegalFn(b: Bool)
    ...

// use
illegalFn(true)     // doesn't say much on intent
illegalFn(false)
```

Perhaps allow it but demand naming the parameter?

```csharp
allowedFn(editable: Bool)
    ...

// use with named parameter
allowedFn(editable = true)
allowedFn(editable = false)
```

---

### Parameter Arguments

> TBD

(Arguments would be at the calling site -invocation- not the function implementation side.)

Make a distinction in accessing parameters in the function implementation.

The goal is to separate parameter references from local variables (or captures).

```csharp
fn: (p: U8): Str
    return Str(Args.p)      // local Args namespace?
    return Str(fn.p)        // nested inside function name(space)
```

---

### Parameter Containers

Use anonymous types or Tuples, Structs and Maps (Dictionaries) as a parameter container.

```csharp
fn: (p: U8, s: Str)
    ...

// anonymous type
a := { s := "42", p := 42 }
fn(...a)

// map (syntax undetermined)
m := { p := 42, s := "42" }
fn(...m)

// does that also work with any struct?
MyStruct
    p: U8
    s: Str
    i: I64

s: MyStruct =
    ...

fn(...s) // ok as long as all parameters (without defaults) are covered.
// s.i is ignored
```

A template trick to allow any struct with the correct properties to be passed as function arguments.

```csharp
fn: <#A>(A args)
    x := args.param1
    y := args.param2

MyStruct
    param1: U8
    param2: Str
    other: I64

s = MyStruct
    ...

// Type A (MyStruct) must have param1 and param2 fields
fn(s)
```

---

> TBD

Interpret the function parameters `(param: U8)` as a tuple. That means that all functions have only one actual param, which is a single tuple and is passed by reference (as an optimization), but by value conceptually.

All the parameters need to be read-only. This does not mean you cannot pass a pointer and change the content that it points too - that still works.

We could auto-generate a struct for each function's parameters and allow that struct to be created, initialized and used as the function's only parameter. Although that would encourage functions with a large number of parameters - something we don't want...

Associate a parameters structure with a function.

```csharp
// function with parameters
fn: (p: u8, s: Str)
    ...

// compiler based
p := fn#parameters
    p = 42
    s = "42"

// as real struct
// 'fn' is namespace?
p := fn.Parameters
    p = 42
    s = "42"

// call
fn(p)
fn(...p)    // spread?
// same as
fn(42, "42")
```

> Also support list (ordered), map (key-value) and stack as a parameters object - or easy conversions?

Will there be an automatic overload of `fn` (using an immutable ptr to the parameter structure instance) or does the compiler unpack the structure at the call site to call the original function?

> Will overloaded functions share one parameter structure with all the parameters - or - each have an individual parameter structure? (can we overload structs?) => No

> The compiler generated option should allow being mapped from a real object.

Perhaps a 'service' function type uses this principle but calls it a 'message' (like gRPC). Would also return a message in that case. Implementation could be gRPC for interop.

---

### Implicit Parameters

> TBD

- Implicit arguments at call site (context/capture)?
- Implicit parameters at declaration? - no, decl is always explicit; capture, or context params

- Related to context variables?
- What is the difference between Implicit/Context parameters and Captures?
- What syntax to use?

```csharp
// declarations
fn: (p: U8, c: Context): U8 // as last parameter
fn: (p: U8, _c: Context): U8 // '_' means hidden
fn: (p: U8)(c: Context): U8 // explicit syntax

// invocation
c: Context =
    ...
fn(42, c)   // explicit
fn(42)   // implicit
```

> TBD For implicit parameter on the call site we can use the `with` keyword or a capture to indicate what instances to use for the implicit parameters. See also [Context Variables](variables.md#context-variables).

---

### Parameter Validation

Some types could have default validations without the code having to explicitly ask for it. These validations are not implicit because they are suggested in the code, just not explicitly stated.

- `null` (interop): automatically validate not-null based on ref-nullability.
- `Enum` (interop): validate standard .NET Enum values to lie inside the defined range.
- more?

> TBD

Some way to do basic parameter validation in a structured manner.

```csharp
fn: |c|<T>(p1: U8, p2: Str): Bool
    // same rules as for types?
    #p1 =< 42
    #p2.Length < 100
    ...
```

These validation rules could then be emitted as a separate function that can be inlined at the call site to prevent 'expensive' calls only to find out the params were not valid.
This is only useful when the calls are expensive enough, so not for normal in process calls that require no dispatching, marshalling or any other type of processing. Then the validation is done inside the function.

### Parameter Inference

> TBD Can the parameters (and return type) of a function be inferred from the function body and/or the specified arguments?

```csharp
//inferredFn: (p: U?): Bool
inferredFn :=
    return p = 42

inferredFn(4242)
```

> How will the size of `p` be determined?

> How will the parser know the difference between a variable and a function? (use val/var keyword)

## Return values

Returning multiple values from a function is only possible using a (custom) structure type or a tuple/anonymous structure.

```csharp
MyStruct
    field1: U8
    field2: U16

// use an explicit struct for retval
MyFunc(p: U8, p2: U16): MyStruct
    return Mystruct
        field1 = p
        field2 = p2
```

```csharp
// use a tuple/anonymous struct for retval
MyFunc(p: U8, p2: U16): (field1: U8, field2: U16)       // list notation
    field1 = p      // this suggests named return values are mutable :-(
    field2 = p2
    return field1, field2

MyFunc(p: U8, p2: U16): {field1: U8, field2: U16}       // object notation
    return {
        field1 = p
        field2 = p2
    }

// unnamed return fields?
MyFunc(p: U8, p2: U16): (U8, U16)
    return p, p2

// not a tuple?
MyFunc(p: U8, p2: U16): U8, U16
    return p, p2
```

The caller has to handle the return value (just like with Error). There is syntax to explicitly ignore the return value.

```C#
retFunc: (): Bool
    ...

b := retFunc()       // ok, retval caught
retFunc()           // error! uncaught retval

_ = retFunc()       // ok, explicitly not interested in retval
```

For fluent interfaces where the return value is the same as the `self` type, not handling the return value is not an error.

> Could the compiler have an opinion about where the return statement is located? Only allow early exits inside and `if` and as last statement in the function. What about only one inside a loop?

> TBD: Want to support covariant return types (function overloads)? => Yes

> TBD: A named return value (as in a tuple) where this name can be referenced from within the function implementation. Maybe even as a way to assign a value. It could exist as an implicit local variable in the function scope. How would this interact with the `return` keyword. Would this work better with a return expression - where the last expression/value determine the return value.

```csharp
fn: (p: U8): (retval: U8)
    retval = p          // this would set the return value
fn: (p: U8): (retval: U8)
    p           // return expression
fn: (p: U8): (retval: U8)
    return p    // return statement

fn: (p: U8): (retval1: U8, retval2: Str)
    retval1 = p
    retval2 = Str(p)
fn: (p: U8): (retval1: U8, retval2: Str)
    (p, Str(p))             // return expression
fn: (p: U8): (retval1: U8, retval2: Str)
    return (p, Str(p))      // return statement
```

---

### Error

The return type of a function can contain an error `Err<T>`, Refer to [Errors](errors.md) for more details.

---

### Optional

The return type of a function can be optional `Opt<T>`. Refer to [Optional](optional.md) for more details.

---

### Void

> In light of .NET interop we need to rethink this.

Z# doesn't have a Void type in the typical conventional sense. It adopts the functional `Unit` type that can have only one value (itself). That way there need to be no difference between functions that return nothing and functions that do return something. If a function has nothing to return, its return-type is implicit `Unit`.

We call this Unit type `Void`.

```csharp
MyFn: (p: U8) // return Void
    ...

v := MyFn(42)    // legal?: v => Void   => nah
// can't do anything with 'v'
```

Another scenario is with constrained union types.

```csharp
// this is actually an Opt<U8>
RetType: Void or U8
MyFn: (p: U8): RetType    // return Void or U8
    ...

v := MyFn(42)    // v => Void or U8
x := match v
    Void => 0
    n: U8 => n
// x = 0 when return was Void
```

The example above should be handled the same as if the return type would be an `Opt<U8>`.

The true purpose is to not have to distinct between function with or without a return value, especially when taking pointers and/or lambda's (see below).

How to interop with actual dotnet void methods?

---

## Function Overloads

Function overloading means that there are multiple functions with the same name but different parameter (or return) types. The compiler picks the best fit for what overloaded function is actually called.

```csharp
fn: ()
fn: (p: U8)
fn: (p: U8, s: Str): Bool
```

Self/Type-bound functions can also be overloaded - by type and/or by parameters.

```csharp
fn: (self: Struct1)
fn: (self: Struct1, p: U8)
fn: (self: Struct2, p: U8)
```

[Type Constructor functions](./types.md#Type-Constructors) can also be overloaded.

> TBD

Compose overloads by combining existing functions.

```csharp
intToString: (i: I32): Str
    ...
boolToString: (b: Bool): Str
    ...

// a list of overloads (syntax?)
toString := (intToString, boolToString)

s := toString(42)        // intToString
s := toString(true)      // boolToString
```

---

## Function Type

A function type is a type that describes the signature of a function.

```csharp
fn: (p: U8): U8     // FunctionType = '(U8): U8'
fn: <G>(p: U8, s: G): Bool // FunctionType = '<G1>(U8, G): Bool'
fn: <G, #T>(p: T, s: G): Bool // FunctionType = '<G1T1>(T, G): Bool'
```

Generic and template types will be encoded into the function type.
Function types with Template type parameters are open types and cannot be instantiated.

Function types can be declared separately from the function implementation.

```csharp
// syntax?
ft := Fn<(U8): U8>
```

Function Types can be derived from and specialized.

```csharp
// syntax?
ft := Fn<(U8): U8>
ftd := Fn<(U8): U8> : ft
```

---

## Recursive Functions

A recursive function is a function that (eventually) calls itself.

> TBD: Allow to specify a maximum depth?

> Can the compiler analyze how deep the recursion will go?

> TBD: add explicit syntax to allow a function to be called recursively. Add syntax for marking `fn` as recursive to guard against accidental type or function name mismatches.

Is it a function Type annotation or a function Name annotation?

```csharp
[[Recursive]]               // decorator => No
#recursive                  // pragma => No
recurseFn: @(p: U8): U8     // syntax (on Type) => No
@recurseFn: (p: U8): U8     // syntax (on Name)
rec recurseFn: (p: U8): U8     // syntax keyword
recurseFn: Rec<(p: U8): U8>    // Wrapper on function type => No
        // exit condition here...
        return recurseFn(p)     // no extra syntax on call?
```

> What if multiple functions are involved in the recursion? All should be marked?

> What happens to captures in a recursive function? The point of capture should not surprise with 'wrong' values.

---

## Function Aliases

A new name can be assigned to an existing function, called an alias.

```csharp
fn: (p: U8)
    ...
aliasFn = fn

// calls fn(42)
aliasFn(42)
```

Aliases are syntactic sugar that are resolved at compile time.

---

## Expression Body Functions

Syntax is similar to the alias-syntax but not quite.

```csharp
// return type is inferred
add: (x: U8, y: U8) = x + y

a := add(42, 101)
```

> Allow more complex expressions?

```csharp
// return type is inferred
add: (x: U8, y: U8) =
    if x > y
        return -(x + y)
    return x + y
```

This suggests that a code block (function body) is also an expression...?

---

## Type Bound (Self)

Using the `self` keyword as the (name of the) first parameter, a function can be bound to a type. In this example the function is bound to (a pointer to) the MyStruct type.

```C#
boundFn: (self: MyStruct)
    ...

s : MyStruct =
    ...

s.boundFn()
boundFn(s)
```

Alternate syntax?

```csharp
// implicit self
MyStruct.boundFn: (p: U8)
    // still use the 'self' keyword
    self.fld1 = p
    // or a shortened version?
    .fld1 = p
```

```csharp
// isolate explicit self from params
boundFn: (self: MyStruct)(p: U8)
    self.fld1 = p
    // or a shortened version?
    .fld1 = p
```

---

### Self Type Navigation

Safe navigation over multiple references.

Syntax to test for an Optional to have a value.

```csharp
boundFn: (self: MyStruct): Opt<MyStruct>
    ...

s = MyStruct
    ...

// syntax TBD
_ = s?.boundFn()?.boundFn()
_ = s&.boundFn()&.boundFn()
```

---

### Self Type Conversion

When calling a bound function, the 'self' parameter can be used as an 'object' using a dot-notation or simply passed as a first parameter. Matching type-bound functions to their types is done as follows:

> TBD

| Var Type | Self Type | Note
|---|---|---
| T | T |
| T | Ptr\<T> | Function can write to var!
| T? | T? |
| T? | Ptr\<T?> | Function can write to var!
| Ptr\<T> | T |
| Ptr\<T> | Ptr\<T> |
| Mut\<T> | Mut\<T> |
| Mut\<T> | Ptr\<Mut\<T>> |
| Ptr<Mut\<T>> | Ptr<Mut\<T>> |

> This means implicit conversions => something we don't want?
We may want this conversion in order to reduce noise of transforming self parameter types.

The `self` parameter can never be optional `Opt<T>`.

Any type can be used, for instance Enum types:

```C#
isMagicValue: (self: MyEnum): Bool
    return self = MyEnum.MagicValue

e := MyEnum.MagicValue

b := e.isMagicValue()        // true
```

---

### Immutable Self

> TBD: The switch from `Imm<T>` to `Mut<T>` takes away a good way to express self-const-ness.
Is there another way to indication (non)const-ness?

A function can publish its 'const-ness' by using an immutable self type.

```csharp
// this function will not change (the content of) self
constFn: (self: Imm<MyStruct>, p: U8): Str  // Imm<>
constFn: (self: MyStruct, p: U8): Str       // Mut<>

// this function will change (the content of) self
nonConstFn: (self: MyStruct, p: U8): Str        // Imm<>
nonConstFn: (self: Mut<MyStruct>, p: U8): Str   // Mut<>
```

> How does mutability of self relate to byValue parameter passing?

A function that does not change self - or any other param for that matter - cannot call any other function on that parameter that DOES change its value. const-functions can only call other const-functions.

> TBD

It would be nice to see a difference on the caller site if a function is mutating the self instance or not. This would make understanding code easier.
The idea is similar to that .NET requires out parameter using the `out` keyword on the caller site.

```csharp
constFn: (self: MyStruct, p: U8): Str
    ...
nonConstFn: (self: Mut<MyStruct>, p: U8): Str
    ...

s := Mut<MyStruct>
    ...

a := s.constFn(42)     // will not change s
b := s.nonConstFn(42)  // will change s

// syntax: to indicate the object is being changed?
b := s<.nonConstFn(42) // '<' also used in mapper and object concatenation
b := s*.nonConstFn(42) // '*' don't associate numeric operators? Pointer?
b := s!.nonConstFn(42) // '!' it's not an error, but it does draw attention
b := s&.nonConstFn(42) // '&' as a reference, or use the Ref<T> type?
b := Ref<s>.nonConstFn(42) // use the Ref<T> type?
```

---

### Bound Property

> TBD: Allow leaving of `()` when bound function has only one or no other parameters?

```csharp
Struct
    ...
getX: (self: Struct): U8
    ...
setX: (self: Struct, p: U8)
    ...

s: Struct
v := s.getX      // calls getX(s)
s.setX v        // calls setX(s, v)
```

We call this the poor-man's property syntax. Do we require the function name to begin with `get_`/`set_` to match .NET properties? Or can we infer them?

For non-bound functions:

```csharp
fn: (): U8
    ...

// no param => no parens
a = fn
```

> How does this differ from taking a reference to a function (function pointer)? => it doesn't unless we change taking-a-function_reference syntax.

```csharp
fn: (p: U8): U8
    ...

// one param => no parens
a := fn 42
```

Functions and variables can have the same name. If there are no `()` for a function call, how to distinguish between the two?

```csharp
x: (): U8
    return 42

x := 42

// x is function or variable?
q := x       // Error! x is ambiguous
// fix for function
q := x()
// fix for variable
q := x   // can't!
```

---

> TBD: Auto fluent-functions on self type with void return type.

```csharp
Struct
    ...
fn1: (self: Struct)
    ...
fn2: (self: Struct)
    ...
fn3: (self: Struct)
    ...
fn4: (self: Struct)
    ...

s: Struct
s.fn1()     // normal function call
    .fn2()  // continue with indent
    .fn3()
    .fn4()

use s       // use 'use' keyword
    .fn1()  // continue with indent
    .fn2()
    .fn3()
    .fn4()
```

If return type is not `Void`, the actual return type is used to determine if the next function call is valid (self type). See also Fluent Functions (below).

---

### Bound Function Extensions

Attaching existing functions to a struct.

```csharp
Struct1
    fld1: U8
// stand alone fn
fn: (p: U8): U8
    ...
// expression body syntax for inline bound function
fnStruct: (self: Struct1) = fn(self.fld1)

s: Struct1
    ...

// calls fn(s.fld1)
x := s.fnStruct()
```

> `.NET`: When the type of the `self` parameter is being compiled, the function is generated as a class method. If the `self` type is external the function is generated as an extension method.

---

### Overriding Self Bound Functions

> TBD: How would that work?

Type resolution is based on the type of the instance (self). If there is no function available for the (more) specialized type, its parent (base) type is used. If no function is available at all it is an error. This is compile-time resolution of polymorphism.

We could also have a template that would select the correct type of function to be called at compile-time. Several options exist.

> To have 'real' polymorphism, overload resolution needs to take place at runtime. .NET uses method tables linked to the (type of) instance of the object. We lack that explicit relation and we would need a dispatch function that determines the instance type and knows what functions to call on it. We could compile these self-bound functions into an object member representation in .NET, though...

Manual polymorphism would make the call based on a specific list of functions.

> How would you reference functions with the same name but different `self` types...?

```csharp
fn: (self: Struct1)
    ...
fn: (self: Struct2)
    ...
fn: (self: Struct3)
    ...

s = Struct2
    ...

// invented syntax for
// - naming a function with different self types
// - a functional operator to choose 'fn' from the list
(fn:<Struct1>, fn:<Struct2>, fn:<Struct3>) >>? s.fn()
// would call: 'fn: (self: Struct2)'

// have a dedicated function for selecting the correct fn to call
Visit(s, (fn:<Struct1>, fn:<Struct2>, fn:<Struct3>))
```

If `fn: (Struct2)` was not in the list, resolution would proceed based on how these structs were derived.

This resolution takes place at run-time. That also allows the function list to be built up dynamically.

---

### Function Object

A function object is where an object can be called as a function with the `()` operator.

```csharp
Struct1
    fld1: U8
    fld2: Str

s = Struct1
    ....

s()     // how??

// normal function tagged as object function
[[#ObjectFunction]]
fn: (self: Struct1)
    ...

// special '()' operator impl.
// double single quotes to escape special chars
''()'': (self: Struct1)
    ...
FunctionCall: (self: Struct1)   // or operator by name
    ...
```

> This is used for dereferencing a `Ptr`. The `Ptr` type has a special `()` operator that calls the `Deref` function.

---

## Type Constructor and Conversion Functions

A function with the same name as a (struct) type is considered a Type Constructor function. A conversion function is considered a variation of a type construction function.

The return type of the function is the type being constructed. A Type constructor or conversion function can have any number of parameters of any type including the type being constructed (which makes it a copy-constructor).

If both the return type as well as the first parameter type are the same and immutable, the constructor function will be called whenever the 'with' syntax (not the context variables) is encountered for that immutable type. See [Immutable Types](types.md#Immutable-Types).
If multiple overloads exist, standard overload resolution is applied to choose the correct function to call.

More information on [Type Constructors](types.md#Type-Constructors) and [Conversions](conversions.md).

```csharp
// default ctor (to signal public creation?)
MyType: (): MyType
// ctor with params
MyType: (p: U8): MyType
// ctor enable deriving a type
MyType: (self: MyType)
// 'with' syntax support
MyType: (self: MyType, merge: Opt<MyType>): MyType
MyType: (self: MyType, merge: Mut<Opt<MyType>>): MyType
// conversion
ThatType: (self: MyType): ThatType
ThatType: (self: MyType, p: U8): ThatType
```

---

## Infix Functions

A function that complies with specific requirements can be used in 'infix' notation, that is: in between its arguments.

An infix function:

- must have a `self` parameter
- must have exactly one addition parameter
- must have a return type

```csharp
plus: (self: U8, p: U8): U16
    return self + p

a := 42
x := a plus 101      // infix
x := a.plus(101)     // bound
x := plus(a, 101)    // flat

// chain
x := a plus 101 plus 12 plus 97 plus 4
```

Also valid

```csharp
plus: (self: U8, arr: Array<U8>): U16
    ...

a := 42
x := a plus (101, 12, 97, 4)     // array param
```

Note that this is different from the poor-mans property syntax where a getter has only a `self` parameter and a return type and a setter has a `self` and one parameter but no return type.

---

## Local Functions and Types

A local function is a function that is defined inside another function and is local to that scope - it cannot be used (seen) outside the function its defined in.

In other aspects they are no different from other functions.

```csharp
MyFunc: (): U8
    LocalFun: (p: U8): U8
        return p << 1

    return LocalFun(42)
```

Local variables (or function parameters) can be captured by local functions using the capture `| |` syntax.

```csharp
OuterFn: (p: U8)
    localFn: |p|(c: U8): Bool
        return p = c

    if localFn(42)  // use
        ...
```

Local Functions can be declared at the end of the containing function. It is not allowed to declare local functions inside local functions.

Local Types are types (Enums, Structs etc.) declared in the local scope of the function and are invisible outside of that function scope.

> The syntax for all local declarations is exactly the same as for other declarations.

```csharp
fn: ()
    MyLocalType
        fld1: U8
        fld2: Str

    lt = MyLocalType
        fld1 = 42
        fld2 = "42"
    
    ...
```

> TBD: Allow local functions? and types to be made public. See also the discussion about calling public nested functions in [Fluent Functions](#Fulent-Functions) and the `.>` operator.

For public nested types, no special operator would be necessary. There is a question of how public nested (in a function) types would be represented in .NET...

---

## Lambdas

A lambda is a nameless function declared inline at the place where it is called, usually through a function pointer callback on another function.

It follows the same makeup as a normal function except that there is no function name.

```csharp
// typical lambda syntax
ForEach<T>(self: Array<T>, fn: Act<T, U8>)

// ptr to fn will work
arr.ForEach(myCallback)
// like match, but different (no capture)
arr.ForEach((v, i) -> log("At {i}: {v}"))
```

```csharp
CallBack: (p: U8) _
Call: (fn: Ptr<Callback>)   // without Ptr<T>?
    ...

sum: U16
// capture by-ref (ptr)
Call([sum.Ptr()](p) -> sum() = sum() + p)
// use indent to allow multiple lines
Call([sum.Ptr()](p)
    sum() = sum() + p
    ...
)
```

> Lambda's should use the same syntax (for Funtion Type) as used for normal functions.

Lambda could be a nameless (function type) definition that can have parameters being applied to it...

```csharp
// (p: U8): Str 
(p) -> Str(p)       // how to apply 42?
((Fn)(p) -> Str(p))(42)         // WTF!
Fn<(p): Str> -> Str(p)(42)      // ??
```

Use short syntax when single parameter:

```csharp
Person
    Name: Str
    Age: U8

list: Array<Person> = (...)

// use '_' discard as replacement for parameter (name)?
list.Select _.Name
// return tuple
list.Select { _.Name, _.Age }
```

---

> We cannot use lambda's to make an anonymous 'object' like in JavaScript at this point. Do we want that?

```JS
return
    {
        fn1 = () -> blabla
    };
```

---

## Coroutines

Coroutines are functions that execute in parts. A different part is executed each time the function is called.

The `yield` keyword indicates that a part in the function code has finished and the function should be exited. When the function is called next, execution will begin right after the yield statement that exited it last time.

The `return` keyword works as normal and also resets the state of the coroutine. The next call to the function will start from the beginning.

There are three (four) types of coroutines in respect to the function return value.

```C#
// multiple calls, no result
coroutine: (p: U8)
    yield
    yield
    yield
    return  // optional

// multiple calls, one result
coroutine: (p: U8): U16?
    yield _
    yield _
    yield _
    return p << 12

// multiple calls each with result
coroutine: (p: U8): Iter<U16>
    yield p
    yield p << 4
    yield p << 8
    return p << 12  // optional

// single call with multiple results
coroutine: (p: U8): Iter<U16>
    yield p
    yield p << 4
    yield p << 8
    return p << 12  // optional

```

> `.NET`: C# only supports the last option.

The Coroutine state is kept in hidden a parameter at the call site. It is needed for the correct function of the coroutine but does not show in its declaration.

> Do we need syntax to clearly identify a coroutine?

> What happens when calling the same co-routine (state) with different parameter values?

```csharp
coroutine: (state: Ptr, p: U8) // hidden state param

i :=^ 42            // mutable
s1 := 0             // (hidden) coroutine call state at call site
loop [0..3]
    coroutine(s1.Ptr(), i)     // ref, yield/return updates state
    i = i + 2
```

```csharp
// multiple coroutines
s1 := 0
s2 := 0
loop [0..3]
    coroutine(s1.Ptr(), 42)
    otherCoroutine(s2.Ptr(), 42)
```

> Do we implement co-routines with capture (as an object) that captures the parameters -so the can't change between calls- and maintains its execution state...?

Coroutines should be lazily evaluated. This should work and only execute the code inside the while loop when the next call to the coroutine is made.

```csharp
allInts: (): I32
    i = 0
    while i < I32#max
        yield i
        i += 1
```

The state of the function is captured (closure) specific to each call-site.

---

## Event Handlers

No specific support for events. Use Function Interfaces and callback function pointers.

The standard library could provide a function interface for general event handling.

Example progress reporting with event callback:

```csharp
IntPercent: U8
    #value = [0..101]   // 0-100
ProgressEventArg
    progress: IntPercent
ProgressEvent<T>: (self: T, arg: ProgressEventArg) _

// use
ReportProgress(self, ProgressEvent progressEvent)
    progress = current * 100 / total
    progressEvent(self, progress)
```

> `.NET`: delegates and events?

In `.NET` a `delegate` is an encapsulation of a function pointer with an optional object (this) reference.
An `event` is a list of registered `delegate`s that all will be called when the event is raised.

```csharp
// .NET 'event' represented as a type
Event<T>

// function pointer to 'handler(any: Any, args: EventArgs)'
Delegate = Fn<(Any, EventArgs)>
event = Event<Delegate>

event.Add(ptrToHandler)
event.Raise(any, args)
event.Remove(ptrToHandler)
```

> TBD:

Perhaps a Reactive approach would be a nice solution for handling events in general?

Use RX.NET?

```csharp
// eventSource is an object that produces new data
eventSource.Subscribe(eventHandler)
    .where(...)

```

---

## Weak Functions

Weak functions are function declarations that allows external code to implement the function. It is a forward declaration that does not needs to be resolved.

If the weak function cannot be resolved it and its call sites are removed without compile errors.

.NET/C# calls this partial methods and has a whole bunch of restrictions on them (some have been lifted in C#9.0).

```csharp
weakFn: () _

// not implemented => removed
weakFn()
```

```csharp
weakFn: () _

// implementation
weakFn: ()
    return

// implemented => called
weakFn()
```

> There is no difference between a function declaration (ending in `_`) and a weak function definition. How do we distinct between weak functions and undefined function (references)?

---

## Fluent Functions

Fluent functions are possible with type-bound functions that return self or another types where another set of bound function is available for.

```C#
FnState1
    ...
FnState2
    ...

add: (self: FnState1, v: U8): FnState2
    ...
build: (self: FnState2): Array<U8>
    ...

s = FnState1        // instantiate root struct
    ...

arr := s.Add(42)     // chained calls can be spread over multiple lines
// split before . and use 2 indents (next indent + 1)
        .Build()
```

```C#
add(42).to(arr)
is(someVar).biggerThan(42)
does<SomeType>().implement<SomeInterface>()
does<SomeType>().implementMethod(MethodName)
can<SomeType>().BeCastedTo<DiffType>()
```

Allow by default using the 'fluent' self object if no return type

```csharp
add: (self: Calc, v: U8)
sub: (self: Calc, v: U8)

c = Calc
    ...

// only works with self-dot syntax
c.add(4).sub(2)
// with scope
c.add(4)
    .sub(2)

// return type of add is void!
// need a new operator?
c.add(4)>.sub(2)

// capture?
|c|
    .add(4)
    .sub(2)

// use / with
use c
with c
    .add(4)
    .sub(2)

```

> TBD: Auto-Fluent syntax? `Build(p: MyStruct).>Into(target: Stream)`

```csharp
baseFn: (p: Str)
    // export local function makes it available
    pub nestedFn: (p: U8): Bool
        ...

s := "42"
// have to 'instantiate' the parent function...
b := baseFn(s).>nestedFn(42)
```

We do need a new operator `.>` because the standard `.` would indicate the (nested) function is called on the return value of the parent function.

---

## Double Dispatch / Visitor Pattern

A demonstration on function overloading and resolving them: does the visitor pattern work?

```C#
// declare data structure to visit
MyStruct1
    ...
MyStruct2
    ...
MyStruct
    field1: MyStruct1
    field2: MyStruct2

// instantiate data structure to visit
s = MyStruct
    ...

// declare visitor struct
Visitor
    ...

// instantiate visitor struct
v = Visitor
    ...

// 2 visit functions for different data structs
// these functions could also be on an interface
visit: (self: Visitor, p: MyStruct1)
    ...
visit: (self: Visitor, p: MyStruct2)
    ...

// accept a visitor
accept: (self: MyStruct, v: Visitor)
    v.visit(field1)
    v.visit(field2)

```

---

## Operator Functions

All operators are implemented as functions. The operator is an (implicit) alias for the actual function.
The compiler supplies a set of standard implementations for common operators on common types.
All operator functions will be tested by the compiler if they confirm to the correct operator rules.

For more information refer to [Lexical Operators](../lexical/operators.md).

Any type can implement an operator-function.

Implement operators by tagging regular functions with the operator symbols.
(This seems to be the best way to also interop with normal .NET / C#.)

```csharp
// must follow the infix function rules.
[[BinaryOperator(">>|")]]
MyWeirdOperator: <T>(self: T, other: T): T
    ...

a := 42
x := a >>| 101   // calls MyWeirdOperator

// How to do unary or ternary operators?
[[UnaryOperator("-")]]
MyUnaryOperator: <T>(other: T): T

[[TernaryOperator(">>|", "|<<")]]
MyTernaryOperator: <T>(self: T, other: T, third: T): T
```

The code decorators for operators also provide information on the operator laws. See [Operator overloads](../compiler/checked.md#operator-overloads).

> TBD: Value based equivalence out of the box for custom types?

---

### Custom operators

> Can custom operators be implemented? (like in F#)

```csharp
// .>>. is the operator. '' used to escape the chars.
''.>>.'': <T>(left: T, right: T): T
    ...
```

> How are the operator rules identified the compiler checks implementations for? We could have some decorators for common operator behaviors.

---

## Top-Level Functions

Any function can be executed as a module is first accessed by placing the call at the top-level (scope) in a module file.

Code in the top-level (scope) will be executed in order of appearance (top to bottom). This code is placed in a static C# constructor for the module type.

```csharp
#module MyMod

x := 42
// will be called at first access
initFn(x)

initFn: (p: U8)
    ...
```

---

## Compile-time Functions

Allows the function call and execution to take place only at compile time. The function will not be part of the binary.

```csharp
// #! => compile time function (not in binary)
#! compileTimeFn: (): U8
    return 42

// use
a := compileTimeFn()
// results in (in binary)
// a = 42
```

See also [Compile-Time Code](../compiler/meta.md#Compile-Time-Code)

> TBD

Allow a compile time function to return a lambda that is used at runtime.
This would enable to build (at compile time) a list work to do at runtime.
Not sure how this will work technically.

```csharp
#! compileTimeFn: (): Fn<U8>
    return () -> 42

fn := compileTimeFn()
a := fn()    // 42
```

---

## Generator Functions

A function that can be called to generate simple string based code at compile time. Use extensions to generate code using the compiler AST.

```csharp
#! genFn: <#T>(c: U8)    // as template
    // use '#' to indicate use of a template param
    // use a special '#<<' syntax?
    #<< Stub#T : #T
    #<$ Stub#T : #T
    loop c
        #<<     fld#c: Str
    // or a compiler pragma?
    #gen Stub#T : #T
    loop c
        #gen     fld#c: Str
    
    // implicit return value?

// function returns generated text?
txt := #genFn<MyStruct>(2)
// StubMyStruct : MyStruct
//     fld1: Str
//     fld2: Str
```

These functions only run during the build and therefor need some orchestration. Perhaps extend the syntax in the [assembly](../modules/assembly.md) file, although the generator functions should be run before build composition in order to include their output.

Some compiler functions to manipulate files would come in handy.

> A generator function (flavor) that uses Linq Expressions to generate code at runtime?

---

## Asynchronous Functions

> `.NET` compatibility for async/await.

Use `Async<T>` as return type to indicate an `async` function (state machine). Any call to a function with an `Async<T>` or `Task<T>` return value is awaited implicitly inside the async-context unless the receiving variable type is of type `Task<T>`. If the return value is Task -without a payload type- the call is awaited if no return value is captured in a variable.

Use a `Task<T>` or normal `T` return type to indicate a non-`async` (sync) context. Any call in this sync-context to a function with an `Async<T>` or `Task<T>` return value is NOT awaited.

`Async<T>` is syntactic sugar for using the (C#) `async` keyword and the `Task<T>` return type.

```csharp
// Alternative syntax
fnAsync: Async<(): U8>  // wrapper type on function type

// C#: async Task<Byte> fnAsync()
fnAsync: (): Async<U8>
    // don't need await (implicit)
    x := workAsync()
    return x + 42

// await is implicit by using Async
// not if Task<T> return type was used in fnAsync
fnAsync: (): Async<U8>
    x: U8 = workAsync()
    return x + 42

// wrapping an async function without async state machine
fnAsync: (): Task<U8>
    // no async, not awaited
    return workAsync(42)

// multiple tasks parallel
// Use explicit Task<T> for return values variables to NOT await
fnAsync: (): Async<U8>
    t1: Task<U8> = work1Async()
    t2: Task<U8> = work2Async()
    // C#: 'await t1 + await t2' (Task.WhenAll)
    return t1 + t2
```

`await` is implicit when 'casting' from `Task<T>` to `T`.
A capture on a task is implicit await on first use:

```csharp
t1: Task<U8> = workAsync()
[t1]
    x := 42 + t1     // await here
```

```csharp
t1: Task<U8> = work1Async()
t2: Task<U8> = work2Async()
[t1, t2]                // Task.WhenAll
    x := t1 + t2         // await twice
```

> I don't like the use of a(n awaited) Task as if it were a value...

```csharp
// async functions
asyncFn1: (p: U8): Task<Str>
    ...
asyncFn2: (): Task
    ...
asyncFn3: (s: Str): Task<Bool>
    ...

result = InvokeAsync(() -> asyncFn1(42), asyncFn2, () -> asyncFn3("42"))
r1, _, r3 := ...result // deconstruct result

// parallel operator?
r1, _, r3 :=>> (asyncFn1(42), asyncFn2(), asyncFn3("42"))

// the results are of type Error<T> where T is the return type of the called function.
// Error<T>: Exception or T
```

> Can we make a `Future<T>` type wrapper that is a bit like `Lazy<T>` where the result is awaited when retrieved?

```csharp
asyncFn: (p: U8): Async<U8>
    ...
// called async and attached to Future<T>
f: Future<U8> = asyncFn(42)
// do other stuff
completed := f.HasCompleted  // Bool
v = f.Value                 // awaited here by future
```

The `Future<T>` here looks and behaves the same as a .NET `Task<T>`...

> Is there a way to wrap sync code into an async-await compatible pattern and to wrap the async-await sequence into a sync-call?

That way the call-site would determine how to execute the function.

```csharp
// sync function
syncFn: (p: U8): U8
    ...
// should work the same in an async-context
syncContext: (): U8
    // call sync function as async task
    t: Task<U8> = #async syncFn(42)         // pragma?
    t: Task<U8> = InvokeAsync(() -> syncFn(42))   // as a lambda
    [] async    // async capture block
        v = syncFn()

    // syncFn is running on a different thread
    // do other stuff
    // await result (U8)
    return t
```

```csharp
// async function
asyncFn: (p: U8): Async<U8>
    ...
// in an async-context normal async-await mechanism is used
syncContext: (): U8
    // implicit wrapping/conversion?
    return asyncFn(42)
    // or explicit '.Sync()' conversion needed
    return asyncFn(42).Sync()
    return #sync asyncFn(42)
```

Using `Sync()` and `Async()` conversion functions mitigates the problem of function coloring (functions have to be called/handled different depending on their traits).

> Compiler should warn for sync/async call-chains that switch multiple times (very inefficient).

> `IAsyncEnumerable<T>` does not use a `Task<T>` as a return value: `public async IAsyncEnumerable<string> GetNamesAsync()`

> TBD

If we change the function definition syntax we could make async functions without using a special return type.

```csharp
// new function syntax
syncFn: (p: U8) -> U8   // syncFn: (p: U8): U8
    ...
asyncFn: (p: U8) => U8  // asyncFn: (p: U8): Async<U8>
    ...

// we could even...
syncParallelFn: (p: U8) ->> U8  // some sort of sync/parallel function?
    ...
asyncParallelFn: (p: U8) =>> U8  // some sort of async/parallel function?
    ...
```

---

### Async Task Cancellation

Use normal .NET `CancellationToken`.

But can we find a way to pass the cancellation token down the call hierarchy without having to specify it explicitly?

Use Context Variables?

```csharp
task: (p: U8, s: Str, c: CancellationToken): Async
    with c
        // c passed as CancellationToken
        task(p, s)

cts := CancellationSource

with cts.Token
    // c passed as CancellationToken
    task(42, "42");
```

> TBD: If we let function-types derive than we could make a base function-type that has an implicit parameter for the cancellation token and derive any async function type from it.

---

### Promise & Future

A Promise is returned from a 'producer' function that will hold the result eventually.
A Future is used by the consuming call site to access the results inside the Promise.

> Do we need this?

See `Future<T>` example above.

---

## Function Traits

> TBD

Function 'traits' that are part of the Function Type.

- Thread safe / Single threaded
- Pure function (no side effects)
- Recursive function
- Ignore unused retvals (for fluent functions)
- Async (expressed in the return type)
- Blocking / non-blocking (Async/co-routine)
- Constant / Immutable self (expressed in the self parameter)
- Generic
- Dynamic (for Types mainly?)
- Static blueprint: a collection of types and functions that defines an architectural interface.

Are Template and/or Generic parameters part of the Function Type?
Template params => no, Generic params => yes.

Type (and in some cases Function) traits can also be used to constraint template (and generic) parameters.

Adding custom traits to a function (and Type) and requiring specific traits in functions could setup a nice guidance framework for writing code / libraries.

```csharp
// trait definition syntax: TBD

// Step trait definition
Step: Trait
    #Async  // a step must be async

// Workflow trait definition
Workflow: Trait
    #Async
    // how to indicate own traits (Async)
    // and code-content traits (Step)??
    #Step       // requiring Step trait

// random function, without traits
fn: (s: Str): Async

// step function
[[Step]]
sp: (s: Str): Async
    ...

// workflow function
[[Workflow]]
wf: (p: U8): Async<Bool>
    sp(p.Str())
    fn(p.Str())     // Error! Required 'Step' trait not present.
```

```csharp
[[Workflow]]
wfError: (p: U8): Bool   // Error! Workflow trait requires 'Async'
    ...
```

Can traits be taken further? Research C++ concepts.

Defining traits also declare on what type of syntax element they can be applied to (multiple).

- Type
- Struct
    - Field
- Enum
    - Option (Field)
- Function
    - Template/Generic Parameter
    - Parameter
    - Return Type

> Is there a Z# assembly-level trait?

---

## TBD

Declarative code: see if we can find a syntax that would make it easy to call lots of functions in a declarative style. Think of the code that is needed to initialize a GUI with all its controls to create and properties to set.
https://github.com/apple/swift-evolution/blob/main/proposals/0289-result-builders.md

> not having to use `()` on function calls with one parameter could be a help here.

---

Allow functions that have no literal implementation but a generator is called during compilation to supply the implementation. Works with decorators and weak functions references? (Same as Roslyn Source Generators)

```csharp
[[MyFnGenerator]]
Fn: <T>(): T _
```

This could be how mapping gets implemented.

---

> TBD

Temporal coupling to function execution (scheduler).

Run this function every 30 seconds.
Call this function after this timeout has elapsed.

---

> TBD

Type Functions

Functions that live inside the namespace of a type.
Static methods in C#.

To avoid a clash with alternate syntax for bound functions - where the typename indicates the type of the `self` parameter. Not needed if the syntax can be parsed and the symbols resolved.

```csharp
MyStruct
    ...

// use 'namespace' syntax?
MyStruct.Fn: (p: U8): Bool
    ...

// call
b := MyStruct.Fn(42)
```

---

> TBD

Scope overridden functions.

The ability to relink the definition of a function within a specific scope.

```csharp
fn: (p: U8): Bool
    ...
myFn: (p: U8): Bool
    ...

b := fn(42)      // calls fn

fn <= myFn      // syntax?  a local alias that overrides existing symbol
b := fn(42)      // calls myFn
```

---

> TBD

What is the syntax for an empty function.

```csharp
// empty function (indent)
emptyFn: (p1: U8)  // No ret val
    _

// function interface
protoFn: (p1: U8): Str _
```

---

> TBD

- simulate properties? thru type-bound functions?
Get\<T> / Set\<T> / Notify\<T> / Watch\<T[]>

- Look into F#'s computational expressions. Basically extensible grammar. <https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions
