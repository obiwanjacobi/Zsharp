# Functions

A function is a block of code that can be called from another function. The function can take parameters and optionally return a value of any type.

A program starts when its entry point (main) function is called.

A function has a name that identifies it uniquely. See [Identifiers](../lexical/identifiers.md).

It is also distinguished by the use of parenthesis `()` when declared or called. Even when the function takes no arguments, the caller still uses the '()' to mark it as function.

```C#
MyFunction: ()        // function declaration
    fn_impl_here

MyFunction()        // function call
```

A more complex example would look like this:

```C#
repeat: (count: U8, char: U8): Str
    fn_impl_here

s = repeat(42, 'X')
```

Here is the full syntax of a function. Everything but the `()`'s are optional.

```csharp
functionName: InterfaceName [captures]<template>(parameters): returnType
```

`functionName`: (optional) All functions except lambdas (anonymous -inline- functions) have a name. Standard [Identifier](../lexical/identifiers.md) rules apply. Only lambdas do not need a name because they are declared inline at the location they're used.

`InterfaceName`: (optional) When basing the type of the function of off a function interface. In that case the declaration of the `types`, `parameters` and `returnType` may be repeated for readability. The function interface name is a type name so it starts with an upper case first letter.

`[captures]`: (optional) This captures variables external to the function for its execution. For 'normal' function these would be global variables. For lambda's these could be function local variables that are used inside the lambda. Captures are only specified on function declarations (implementation), not on function (type) interfaces. The name of the capture refers to a variable (or parameter) and that name is also used in the function's implementation. Comma separated.

`<template>`: (optional) Template Parameters that the template function uses in its implementation. Comma separated.

`(parameters)`: (optional) By-value parameters the function acts on. Comma separated.

`returnType`: (optional) The Type of the function result.

```csharp
// not showing implementation
fn: ()
fn: (): U8
fn: (p: U8)
fn: (p: U8): U8

fn: <T>(): T
fn: <T>(p: T)
fn: <T, R>(p: T): R

// capture on fn impl
fn: [c]<T>(p: T): Bool        // by val
fn: [c.Ptr()]<T>(p: T): Bool  // by ref

fn: InterfaceName
// interface impl with capture
fn: [c] InterfaceName
// repeated function type decl with capture
fn: InterfaceName [c.Ptr()]<T>(p: T): Bool
```

> How to differentiate from object construction? => Has no field names.

---

> TBD

Make the function syntax more like variable syntax? => Use an `=` to 'assign' the implementation to the function name.

```csharp
// single line, without return, use =>
isFortyTwo: (p: U8): Bool => p = 42

// multi line, with indent and return, use =
isFortyTwo: (p: U8): Bool =
    return p = 42

// function declarations have no '=' sign.
fnDecl: (p: U8)     // no _ needed
```

---

## Parameters

There is no other way of passing parameters to functions than by value. That means, that the parameter value is _copied_ from the caller site into the context of the function.

That also means that if a parameter is to be passed by reference, an explicit pointer to that value has to be constructed and passed to the function.

> The compiler can still use an immutable reference for optimizations. The by-value model is how you should think about it.

```C#
byref: (ptr: Ptr<U8>)     // pointer as by-ref parameter
    ...

v = 42
byref(v.Ptr())            // call with ptr to value
```

> TBD: it would be nice to be able to see if a variable or parameter was an literal value. Then specific logic could be applied in these cases. For instance, when a parameter is a literal, the result of the function could be made immutable?

Allow function overloads with some sort of special syntax for literal values?

### Optional Parameters

Optional function parameters can be specified using the optional symbol `?`.

```C#
hasParam: (p: U8?): Bool
    return p ? true : false
    return p    // error! implicit cast not allowed
    return p?   // but there is a special syntax
```

### Default Parameter Values

> Not supported.

Functions should have unique names with well-defined parameters.
Having default parameter values does not explain at the calling site what is happening.

> See note in [Template Parameters](./templates.md#Template-Parameter-Defaults) about supporting a default value for (template) parameters.

### Named Parameter

Function Parameters can be specified by name at the calling site.

```C#
namedFn: (p: U8, p2: U16)
    ...

namedFn(p = 42, p2 = 0x4242)    // ok, both named
namedFn(p2 = 0x4242, p = 42)    // ok, out of order, but named
namedFn(42, p2 = 0x4242)        // ok, p in order, rest named
namedFn(0x4242, p = 42)         // ok, unnamed is only one left
```

### Variable number of parameters

Not really supported but you can fake it with an Array: all of same type. We don't have an 'object' type that is the basis of all. (do we need to?)

```C#
varFunc: <T>(p: U8, varP: Array<T>)
    ...

// requires (easy) syntax for specifying
varFunc(42, [1, 2, 3, 4, 5, 6])
```

### Immutable Parameters

Gives the caller the guarantee that the parameter will not be changed.
Only useful for `Ptr<T>` types. All function parameters are passed by value.

```csharp
immFn: (p: Ptr<Imm<U8>>)
// shorter using type operators
immFn: (p: ^*U8)
```

### Out and ByRef Parameters

> Use a mutable Pointer.

```C#
// simulated out-parameter
Make42: (p: Ptr<U8>)
    p() = 42
```

### Illegal Parameter Types

> TBD: playing with the idea of making `Bool` an illegal parameter type for an exported function.

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

> Another potential problem is passing `Bit<n>` as parameter in that the number of bits will be rounded up to a byte boundary.

---

## Return values

Returning multiple values from a function is only possible using a (custom) structure type. There are no out parameters and no tuples.

```C#
MyStruct
    field1: U8
    field2: U16

MyFunc(p: U8, p2: U16): MyStruct
    return Mystruct
        field1 = p
        field2 = p2
```

> Compiler will check if a ptr is returned, it is not pointing at a stack variable.

Return values are also passed by value, so in the example above, two values (U8 and U16) would be copied to the caller.

The caller has to handle the return value (just like with Error). There is a syntax to explicitly ignore the return value.

```C#
retFunc: (): Bool
    ...

b = retFunc()       // ok, retval caught
retFunc()           // error! uncaught retval

_ = retFunc()       // ok, explicitly not interested in retval
```

> Could the compiler have an opinion about where the return statement is located? Only allow early exits inside and `if` and as last statement in the function. What about only one inside a loop?

### Error

The return type of a function can contain an error `Err<T>`, Refer to [Errors](errors.md) for more details.

### Optional

The return type of a function can be optional `Opt<T>`. Refer to [Optional](optional.md) for more details.

### Void

Z# doesn't have a Void type in the typical conventional sense. It adopts the functional `Unit` type that can have only one value (itself). That way there need to be no difference between functions that return nothing and functions that do return something. If a function has nothing to return, its return-type is implicit `Unit`.

We call this Unit type `Void`.

```csharp
MyFn: (p: U8) // return Void
    ...

v = MyFn(42)    // legal: v => Void
// can't do anything with 'v' though
```

Another scenario is with constrained union types.

```csharp
// this is actually an Opt<U8>
RetType: Void or U8
MyFn: (p: U8): RetType    // return Void or U8
    ...

v = MyFn(42)    // v => Unit or U8
x = match v
    Void => 0
    n: U8 => n
// x = 0 when return was Void
```

The example above should be handled the same as if the return type would be an `Opt<U8>`.

The true purpose is to not have to distinct between function with or without a return value, especially when taking pointers and/or lambda's (see below).

---

## Function Overloads

Function overloading means that there are multiple functions with the same name but different parameter (or return) types.

But in Z#, only type-bound functions can the `self` parameter be used to overload the function name. The type of `self` is the only thing allowed to change for functions with the same name.

> Not sure if this is actually required. More that resolvement will only be based on the self parameter's Type.

Give all other functions a unique name.

```csharp
fn(self: Struct1, p: U8)
fn(self: Struct2, p: U8)
fn(self: Struct1)  // error
```

One exception to this rule are the Type Constructor functions. See Also [Types](./types.md).

> TBD: should we also support covariant return types?

---

## Recursive Functions

A recursive function is a function that (eventually) calls itself.

Recursive functions are supported although small memory/stack machines will have to be careful of not overflowing the stack.

> TBD: Allow to specify a maximum depth?

> Can the compile analyze how deep the recursion will go?

> TBD: add explicit syntax to allow a function to be called recursively. Add syntax for calling `fn` in body of `fn` to guard against accidental type or function name mismatches. Can happen easily when overloading on self and calling the 'base'...

```csharp
{Recursive}     // decorator
#recursive      // pragma
recurseFn: (p: U8): U8
        // exit condition here...
        return recurseFn(p)     // no extra syntax
        return @recurseFn(p)    // specific syntax
```

---

## Type Bound (Self)

Using the `self` keyword on the (name of the) first parameter, a function can be bound to a type. In this example the function is bound to the MyStruct type.

```C#
boundFn: (self: Ptr<MyStruct>)
    ...

s = MyStruct
    ...

s.boundFn()
boundFn(s)
```

When calling a bound function, the 'self' parameter can be used as an 'object' using a dot-notation or simply passed as a first parameter. Normal function rules apply, so for a struct it is usually a good idea to declare a `Ptr<T>` in order to avoid copying and be able to change the fields of the structure. Matching type-bound functions to their types is done as follows:

|Var Type | Self Type | Note
|---|---|---
| T | T |
| T | Ptr\<T> | Function can write to var!
| T? | T? |
| T? | Ptr\<T?> | Function can write to var!
| Ptr\<T> | T |
| Ptr\<T> | Ptr\<T> |
| Imm\<T> | Imm\<T> |
| Imm\<T> | Ptr<Imm\<T>> |
| Ptr<Imm\<T>> | Ptr<Imm\<T>> |

> This means implicit conversions => something we don't want?
We may want this conversion in order to reduce noise of transforming self parameter types.

The `self` parameter can never be optional `Opt<T>`.

Any type can be used, for instance Enum types:

```C#
isMagicValue: (self: MyEnum): Bool
    return self = MyEnum.MagicValue

e = MyEnum.MagicValue

b = e.IsMagicValue()        // true
```

> TBD: Allow leaving of `()` when bound function has no other parameters?

```csharp
Struct
    ...
fn: (self: Struct)
    ...

s: Struct
s.fn        // calls fn(s)
```

We call this the poor-man's property syntax.

Auto fluent-functions on self type with void return type.

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
```

If return type is not `Void`, the actual return type is used to determine if the next function call is valid (self type). See also Fluent Functions (below).

### Overriding Self Bound Functions

> TBD: How would that work?

---

## Local Functions

A local function is a function that is defined inside another function and is local to that scope - it cannot be used (seen) outside the function its defined in.

In other aspects they are no different from other functions.

```C#
MyFunc: (): U8
    LocalFun: (p: U8): U8
        return p << 1;

    return LocalFun(42)
```

Local variables (or function parameters) can be captured by local functions using the capture `[ ]` syntax.

```csharp
OuterFn: (p: U8)
    localFn: [p](c: U8): Bool
        return p = c

    if localFn(42)  // use
        ...
```

> Can Local Functions be declared at the end of the containing function?

> Limit on how many nesting levels?

---

## Lambdas

A lambda is a nameless function declared inline at the place where it is called, usually through a function pointer callback on another function.

It follows the same makeup as a normal function except that there is no function name.

It all starts with the ability to create a (typed) function pointer.

```csharp
// alternate (anonymous) function interface syntax
// using a type (no names, just types)
fn: Fn<(U8): U8>    // complex Type
fn: Fn<U8, U8>      // one param, retval
fn: Fn<U8, Str, U8> // two params, retval, etc
fn: Act<U8>         // one param, no retval
fn: Fn<U8, Void>
fn: Act<U8, Str>    // two params, no retval, etc
fn: Fn<U8, Str, Void>
```

```csharp
// typical lambda syntax
ForEach<T>(self: Array<T>, fn: Act<T, U8>)

// ptr to fn will work
arr.ForEach(myCallback)
// like match, but different (no capture)
arr.ForEach((v, i) => log("At {i}: {v}"))
```

```csharp
CallBack: (p: U8) _
Call: (fn: Ptr<Callback>)
    ...

sum: U16
// capture by-ref (ptr)
Call([sum.Ptr()](p) => sum() = sum() + p)
// use indent to allow multiple lines
Call([sum.Ptr()](p)
    sum() = sum() + p
    ...
)
```

> We cannot use lambda's to make an anonymous 'object' like in JavaScript at this point. Do we want that?

```JS
return
    {
        fn1 = () => blabla;
    };
```

---

## Coroutines

Coroutines are functions that execute in parts. A different part is executed each time the function is called.

The `yield` keyword indicates that a part in the function code has finished and the function should be exited. When the function is called next, execution will begin right after the yield statement that exited it last time.

The `return` keyword works as normal and also resets the state of the coroutine. The next call to the function will start from the beginning.

There are three types of coroutines in respect to the function return value.

```C#
// multiple calls, no result
coroutine: (p: U8)
    yield
    yield
    yield
    return

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
    return p << 12
```

The Coroutine state is kept in hidden a parameter at the call site. It is needed for the correct function of the coroutine but does not show in its declaration.

```C#
coroutine: (state: Ptr, p: U8) // hidden state param

i = 42
s1 = 0           // (hidden) coroutine call state at root-scope
loop [0..3]
    coroutine(i, s1.Ptr())     // ref, yield/return updates state
    i = i + 2

// multiple coroutines
s1 = 0
s2 = 0
loop [0..3]
    coroutine(42, s1.Ptr())
    otherCoroutine(42, s2.Ptr())
```

---

## Events

No specific support for events. Use Function Interfaces and callback function pointers.

The standard library could provide a function interface for general event handling.

Example progress reporting with event callback:

```csharp
IntPercent: U8
    #value = [0..101]   // 0-100
ProgressEventArg
    progress: IntPercent
ProgressEvent: (self, arg: ProgressEventArg) _

// use
ReportProgress(self, ProgressEvent progressEvent)
    progress = current * 100 / total
    progressEvent(self, progress)
```

---

## Pure Functional

A pure function -without side-effects- can be recognized by the lack of mutable captures and the presence of immutable (only in-) parameters. It also has to have a return value.

```csharp
// has imm param but potentially writes to globalVar
sideEffect: [globalVar.Ptr()](p: Ptr<Imm<U8>>)
// takes two params and produces result - no side effects
pureFn: (x: U8, y: U8): U16
```

A higher order function is a function that returns a higher-order function.

```csharp
// a function that returns a function
pureFn: (arr: Arr<U8>): Fn<(U8): U8>
    ...
// call pureFn and call the function it returns
v = pureFn([1,2])(42)
```

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

arr = s.Add(42)     // chained calls can be spread over multiple lines
// split before . and use 2 indents (next indent + 1)
        .Build()
```

```C#
is(someVar).biggerThan(42)
does<SomeType>().implement<SomeInterface>()
can<SomeType>().BeCastedTo<DiffType>()
does<SomeType>().implementMethod(MethodName)
```

Allow by default using the 'fluent' self object if no return type

```csharp
add: (self: Calc, v: U8)
sub: (self: Calc, v: U8)

c = Calc
// only works with self-dot-syntax
c.add(4).sub(2)
// with scope
c.add(4)
    .sub(2)
```

> Auto-Fluent syntax? `Build(p: MyStruct)::Into(target: Stream)`

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

## Captures

Captures are snapshots (copies) or references to contextual state -like local variables- that accompany a child context.

Function can use captures to be able to reference global variables in their body.

Lambda's can use captures to be able to reference data in their vicinity to use during execution of the lambda.

An additional syntax is considered for capturing dependencies of any code block. This may be a valuable feature when refactoring code.

```csharp
v = 42
// following code is dependent on v
[v]
    // use v here
```

Captures also may be used as a synchronization mechanism for shared data. At the start of a capture a copy is made of the data and the code (function) works with that copy. The actual value may be changed (by an interrupt) in the meantime.

In case of a mutable capture, it's value is written back to the original storage when the block of code is completed.

> How do we allow to opt in for all these different capture behaviors?

> We do need a mechanism to handle conflicts when writing back captured data? Or is this directed by using the correct Data type wrapper (`Atom<T>`)?

> Perhaps as an optimization, immutable captures (in general) could be passed as references to a parent stack frame?

Read-only capture are never a problem, the problem exists when using pointer-captures that are written to sometime during the execution of the function/lambda or code block.

We might not always want to make a hidden state object to store their 'captures'.

Function-captures may want to use different mechanism to synchronize access to shared/global state. We could use data type wrappers like `Atom<T>` to indicate this strategy. By default a global variable state is read at first access by the function and compared with the actual value before a new value is written at the end of the function (optimistic locking).

Lambda-captures may outlive the function they're declared in. Not sure how pointer-captures would work in that scenario?

Block-captures work as functions would.

---

## Piping Operator

To make nested function calls more readable. More 'functional'.
Would make line-breaks in long statements (chains) a lot more readable?

```csharp
a = fn1(fn2(fn3(42)))
b = fn3(42) |> fn2() |> fn1()
```

Evaluate LeftHandSide, parse RightHandSide and inject left result into right parse tree.

Subsequent function calls (after `|>`) will have their 1st param missing. That looks a bit strange.

Does this only work for functions? (concatenation?)

```csharp
[0..5] |> fn()  // passed in array
```

Could also have a 'backward' piping operator? `<|` going the other way...

```csharp
fn1() <| fn2() <| fn3()
```

As concatenation?

```csharp
// C++ style output?
yourName = "Arthur";
outStream <| "Hello " <| yourName

// C++ style input?
inStream |> yourName
```

---

Interpret the function parameters `(param: U8)` as a tuple. That means that all functions only have only one actual param, which is a single tuple and is passed by reference (as an optimization), but by value conceptually.

All the parameters need to be read-only (which is a bit odd because they may not use the `Imm<T>` type). This does not mean you cannot pass a pointer and change the content that it points too - that still works.

What is the overhead in building the tuple at the call-site?

Function params are specified in order at call-site, we are steering away from by-order for tuples/deconstruction. That would mean that function parameters are to be specified by name only, which can get very verbose.

---

simulate properties? thru type-bound functions?
Get\<T>/Set\<T>/Notify\<T>/Watch\<T[]>

tag interrupt service routines (for analysis - volatile) as a simplified interface?
functions that do not return?

intrinsic functions (operator implementations) - extensions?

top-level function calls/one-time initialization at first access of module.

declarative code: see if we can find a syntax that could would make it easy to call lts of functions in a declarative style. Think of the code that is needed to initialize a GUI with all its controls to create and properties to set.
https://github.com/apple/swift-evolution/blob/main/proposals/0289-result-builders.md


How to define the entry point of a program? (Main)
Decorator? Fixed Name?
