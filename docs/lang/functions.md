# Functions

A function is a block of code that can be called from another function. The function can take parameters and optionally return a value of any type.

A program starts when its entry point (main) function is called.

A function has a name that identifies it uniquely. See [Identifiers](../lexical/identifiers.md).

It is also distinguished by the use of parenthesis `()` when declared or called. Even when the function takes no arguments, the caller still uses the '()' to mark it as function (-perhaps not).

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

`returnType`: (optional) The Type of the function result. `Void` if not specified.

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

> How to differentiate `fn: InterfaceName` from object construction? => Has no field names.

---

> TBD

Make the function syntax more like variable syntax? => Use an `=` to 'assign' the implementation to the function name.

```csharp
// single line, without return, use ->
isFortyTwo: (p: U8): Bool -> p = 42

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

Function Pointer as function argument syntax:

```csharp
filter: (predicate: (p: U8): Bool)
    ...
filter: (predicate: Fn<(p: U8): Bool>)
    ...
filter: (predicate: Fn<U8, Bool>)
    ...
```

### Optional Parameters

Optional function parameters can be specified using the optional symbol `?`.

```C#
hasParam: (p: U8?): Bool
    return p ? true : false
    return p    // error! implicit cast not allowed
    return p?   // but there is a special syntax
```

> TBD: cancel calling a function when parameter is not available?

```csharp
fn: (p: U8)
    ...

v: Opt<U8>  // not set

fn(v?)      // do not call function if v is not set.
?fn(v)

// shorthand for
if v?
    fn(v)
```

### Default Parameter Values

> Not supported.

Functions should have unique names with well-defined parameters.
Having default parameter values does not explain at the calling site what is happening.

> See note in [Template Parameters](./templates.md#Template-Parameter-Defaults) about supporting a default value for (template) parameters.

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
immFn: (p: *^U8)
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

> Another 'issue' is passing `Bit<n>` as parameter in that the number of bits will be rounded up to a byte boundary.

---

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
MyFunc(p: U8, p2: U16): (field1: U8, field2: U16)
    return {
        field1 = p
        field2 = p2
    }
```

> Compiler will check if a ptr is returned, it is not pointing at a stack variable. (not a concern in .NET)

Return values are also passed by value, so in the examples above, the structure reference is copied to the caller.

The caller has to handle the return value (just like with Error). There is a syntax to explicitly ignore the return value.

```C#
retFunc: (): Bool
    ...

b = retFunc()       // ok, retval caught
retFunc()           // error! uncaught retval

_ = retFunc()       // ok, explicitly not interested in retval
```

> Could the compiler have an opinion about where the return statement is located? Only allow early exits inside and `if` and as last statement in the function. What about only one inside a loop?

> TBD: could we also support covariant return types (function overloads)?

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

v = MyFn(42)    // legal?: v => Void
// can't do anything with 'v'
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

Function overloading means that there are multiple functions with the same name but different parameter (or return) types. The compiler picks the best fit for what overloaded function is actually called.

But in Z#, only type-bound functions can the `self` parameter be used to overload the function name. The type of `self` is the only thing allowed to change for functions with the same name.

> Not sure if this is actually required. More that resolvement will only be based on the self parameter's Type.

> Would be a problem for interop with existing .NET code. Not having these restrictions makes interacting with existing .NET code a lot less hassle.

> Would also obstruct partial function application.

Give all other functions a unique name.

```csharp
fn(self: Struct1, p: U8)
fn(self: Struct2, p: U8)
fn(self: Struct1)  // error
```

One exception to this rule are the Type Constructor functions. See Also [Types](./types.md).

---

## Recursive Functions

A recursive function is a function that (eventually) calls itself.

> TBD: Allow to specify a maximum depth?

> Can the compiler analyze how deep the recursion will go?

> TBD: add explicit syntax to allow a function to be called recursively. Add syntax for marking `fn` as recursive to guard against accidental type or function name mismatches. Is it a function Type annotation or a function Name annotation?

```csharp
{Recursive}                 // decorator
#recursive                  // pragma
recurseFn: @(p: U8): U8     // syntax (on Type)
@recurseFn: (p: U8): U8     // syntax (on Name)
rec recurseFn: (p: U8): U8     // syntax keyword
        // exit condition here...
        return recurseFn(p)     // no extra syntax on call
```

> What if multiple functions are involved in the recursion? All should be marked?

> What happens to captures in a recursive function?

---

## Function Aliases

A new name can be assigned to an existing function, called an alias.

```csharp
fn: (p: U8)
    ...
aliasFn= fn

// calls fn(42)
aliasFn(42)
```

Aliases are syntactic sugar and are resolved at compile time.

The alias syntax is also used for function composition. In that case the function body of the composite function (alias) is not compiled into code but used as a composition template.

```csharp
fn: (p: U8, s: Str)
    ...

// alias with body => composition
fnPartial= (p: Str)
    fn(42, p)

// calls fn(42, "42")
fnPartial("42")
```

---

## Type Bound (Self)

Using the `self` keyword as the (name of the) first parameter, a function can be bound to a type. In this example the function is bound to (a pointer to) the MyStruct type.

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
fn: (self: Struct): U8
    ...

s: Struct
v = s.fn    // calls fn(s)
```

We call this the poor-man's property syntax. Do we require the function name to begin with `get_`/`set_` to match .NET properties? Or can we infer them?

> Allow specifying the self-Type explicitly for readability?

```csharp
Struct
    ...
fn: (self: Struct): U8
    ...

s: Struct
v = Struct.fn(s)
```

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

> TBD: Allow attaching existing functions to a struct??

```csharp
Struct1
    fld1: U8
// stand alone fn
fn: (p: U8): U8
    ...
// function alias? inline function?
fnStruct: (self: Struct1) = fn(self.fld1)
```

### Overriding Self Bound Functions

> TBD: How would that work?

Type resolution is based on the type of the instance (self). If there is no function available for the (more) specialized type, its parent (derived) type is used. If no function is available at all it is an error. This is compile-time resolution of polymorphism.

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
{#ObjectFunction}
fn: (self: Struct1)
    ...

// special '()' operator impl.
(): (self: Struct1)
    ...
FunctionCall: (self: Struct1)   // or operator by name
    ...
```

---

## Type Constructor and Conversion Functions

A function with the same name as a (struct) type is considered a Type Constructor function. A conversion function is considered a variation of a type construction function.

The return type of the function is the type being constructed. A Type constructor or conversion function can have any number of parameters of any type including the type being constructed (which makes it a copy-constructor).

If both the return type as well as the first parameter type are the same and immutable, the constructor function will be called whenever the 'with' syntax (not the context variables) is encountered for that immutable type. See [Immutable Types](types.md#Immutable-Types).
If multiple overloads exist, standard overload reolution is applied to choose the correct function to call.

More information on [Type Constructors](types.md#Type-Constructors) and [Conversions](conversions.md).

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

> Limit on how many nesting levels? Yes, no local functions in local functions.

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

> How does capture work inside loops?

```csharp
import
    Fn
    Print

// Fn => (): Void
l = List<Fn>(10)
loop c in [0, 10]
    l.Add([c]() -> Print(c))
for fn in l
    fn();   // what does it print?
```

`[c]` is capturing by value, so it should print 0-9.

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

There are three types of coroutines in respect to the function return value.

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
    return p << 12
```

The Coroutine state is kept in hidden a parameter at the call site. It is needed for the correct function of the coroutine but does not show in its declaration.

> Do we need syntax to clearly identify a coroutine?

> What happens when calling the same co-routine (state) with different parameters?

```C#
coroutine: (state: Ptr, p: U8) // hidden state param

i = 42
callings1 = 0           // (hidden) coroutine call state at root-scope
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

> Do we implement co-routines with capture that captures the parameters -so the can't change between calls- and maintains its execution state...?

Coroutines should be lazily evaluated. This should work and only execute the code inside the while loop when the next call to the coroutine is made.

```csharp
allInts: (): Int32
    i = 0
    while i < Int32#max
        yield i
        i += 1
```

The state of the function is captured (closure) specific for each call-site.

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
ProgressEvent<T>: (self: T, arg: ProgressEventArg) _

// use
ReportProgress(self, ProgressEvent progressEvent)
    progress = current * 100 / total
    progressEvent(self, progress)
```

---

## Weak Functions

> TBD not sure if this will fly

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

## Pure Functional

A pure function is a function that returns the exact same result given the same inputs without any side effects.

A pure function -without side-effects- can be recognized by the lack of (mutable?) captures and the presence of immutable (only in-) parameters. It also has to have a return value.

> This is not always true (a database read function may return different results for the same inputs - even if we explicitly capture the non-mutable database connection) so we may need to introduce special syntax to indicate pure functional functions...

```csharp
// has imm param but potentially writes to globalVar
sideEffect: [globalVar.Ptr()](p: Ptr<Imm<U8>>)
// takes two params and produces result - no side effects
pureFn: (x: U8, y: U8): U16

// seems ok because capture is immutable
// but what if it is a database connection?
// then the function can still return different results for the same args
pureFnMaybe: [globalVar](x: U8, y: U8): U16

// special syntax to promise purity?
// on the return type?
pureFnMaybe: [globalVar](x: U8, y: U8): Pure<U16>
// with a 'pure' keyword?
pure pureFnMaybe: [globalVar](x: U8, y: U8): U16

```

A higher order function is a function that takes or returns another function (or both).

```csharp
// a function that returns a function
pureFn: (arr: Arr<U8>): Fn<(U8): U8>
    ...
// call pureFn and call the function it returns
v = pureFn([1,2])(42)
```

> TBD: Syntax for function partial application, composition and currying?

> function composition, partial application and parameter currying should be done inline and at compile time ideally. This would generate more code, but perform better at runtime - as opposed to linking existing functions together. Not sure how to handle external functions that are already compiled.

See also [Piping Operator](#Piping-Operator).

> Composition and value piping are different concepts. Function Composition is building functions from other functions at compile time. Value piping chains (result) values between multiple function calls (result of one function is parameter for next function).

Due to our choices in syntax most of these functional principles has to be spelled out. We could make an exception for not having to specify the `()` [when there is only one (or no) parameter?].

```csharp
add: (p1: U8, p2: U8): U16
    return p1 + p2
// partial application by hand
add42: (p: U8): U16
    return add(42, p)
```

By returning a local function it becomes anonymous. Any references to external variables / parameters need to be captured.

```csharp
fn: (p1: U8, p2: Str): Fn<(U8): U8>
    internalFn: [p2](p1: U8): U8
        return p2.U8() + p1
    return internalFn
```

> TBD: a syntax that allows function composition in a way that inlined at compile time. Basically a template function with functional template parameters.

```csharp
fn: (p: U8, s: Str)
    ...
// use alias syntax on composed function
compFn= (p: U8)
    fn(p, "42")

// call composite function
compFn(42)
// actual code compiled:
// fn(42, "42")
```

> Can you take a reference/pointer to a composite function? If you do, you create an actual function stub with the composition compiled.

> TODO: look into monads. I don't understand them yet.

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
// only works with self-dot syntax
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

Captures also may be used as a synchronization mechanism for shared data. At the start of a capture a copy is made of the data and the code (function) works with that copy. The actual value may be changed (by another thread) in the meantime.

In case of a mutable capture, it's value is written back to the original storage when the block of code is completed.

That would also suggest that capture blocks themselves could be multi-threading / execute separately from other parts of the function if the dependencies would allow it. Not sure if this 'feature' would be desirable for it would make reasoning about the code harder.

> How do we allow to opt-in for all these different capture behaviors?

> We do need a mechanism to handle conflicts when writing back captured data? Or is this managed by using the correct Data type wrapper (`Atom<T>`)?

> Perhaps as an optimization, immutable captures (in general) could be passed as references to a parent stack frame?

Read-only capture are never a problem, the problem exists when using pointer-captures that are written to sometime during the execution of the function/lambda or code block.

We might not always want to make a hidden state object to store their 'captures'.

Function-captures may want to use different mechanism to synchronize access to shared/global state. We could use data type wrappers like `Atom<T>` to indicate this strategy. By default a global variable state is read at first access by the function and compared with the actual value before a new value is written at the end of the function (optimistic locking).

Lambda-captures may outlive the function they're declared in. Not sure how pointer-captures would work in that scenario?

Block-captures work as functions would.

```csharp
x = 42
fn: [x](p: U8): Bool
    return x = p

// no capture has to be specified in call
sameAsX = fn(42)   // true
```

### Capture Aliases

Like all Aliases, using the assignment operator will rename the capture for the function scope.

```csharp
x = 42
fn: [x=y](p: U8): Bool
    return y = p
```

---

## Piping Operator

To make nested function calls more readable. More 'functional'.
Would make line-breaks in long statements (chains) a lot more readable?

```csharp
a = fn1(fn2(fn3(42)))
b = fn3(42) |> fn2() |> fn1()
// functional syntax
b = 42 |> fn3 |> fn2 |> fn1
```

Subsequent function calls (after `|>`) will have their 1st param missing. That looks a bit strange (but no different than bound functions?). `()` can be omitted when a function has zero or one parameter?

```csharp
[0..5] |> fn()  // passed in array
```

Could also have a 'backward' piping operator? `<|` going the other way...

```csharp
fn1() <| fn2() <| fn3() <| 42
```

Does this only work for functions? (string and list concatenation?)

```csharp
// C++ style output?
yourName = "Arthur";
outStream <| "Hello " <| yourName

// C++ style input?
inStream |> yourName
```

... or use a different syntax?

```csharp
// C++ style output?
yourName = "Arthur";
outStream +| "Hello " +| yourName

// C++ style input?
inStream =| yourName
```

Concat lists?

```csharp
l1 = (1, 2, 3, 4)
l2 = l1 |> (5, 6, 7, 8)
// l2 = (1, 2, 3, 4, 5, 6, 7, 8)
```

---

## Operator Functions

All operators are implemented as functions. The operator is an alias for the actual function.

> Will the compiler supply standard implementation for common operators on custom types? (C++ spaceship operator)

> Value based equivalence out of the box for custom types?

All operator functions will be tested by the compiler if they confirm to the correct operator rules.

For more information refer to [Lexical Operators](../lexical/operators.md).

### Custom operators

> Can custom operators be implemented? (like in F#)

```csharp
// '.>>.' is the operator. () used to escape the chars.
(.>>.): <T>(left: T, right: T): T
    ...
```

> How are the operator rules identified the compiler checks implementations for? We could have some decorators for common operator behaviors.

---

## Compile-time Functions

Allows the function call and execution to take place only at compile time. The function will not be part of the binary.

```csharp
// #! => compile time function (not in binary)
#! compileTimeFn: (): U8
    return 42

// use
a = compileTimeFn()
// results in (in binary)
a = 42
```

See also [Compile-Time Code](../compiler/meta.md#Compile-Time-Code)

---

## Generator Functions

A function that can be called to generate simple string based code at compile time. Use extensions to generate code using the compiler AST.

```csharp
#! genFn: <#T>(c: U8)    // as template
    // use '#' to indicate use of a template param
    // use a special '#<<' syntax?
    #<< Stub#T : #T
    loop c
        #<<     fld#c: Str
    // or a compiler pragma?
    #gen Stub#T : #T
    loop c
        #gen     fld#c: Str
    
    // implicit return value?

// function returns generated text?
txt = #genFn<MyStruct>(2)
// StubMyStruct : MyStruct
//     fld1: Str
//     fld2: Str
```

These functions only run during the build and there for need some orchestration. Perhaps extend the syntax in the [assembly](../modules/assembly.md) file, although the generator functions should be run before build composition in order to include their output.

Some compiler functions to manipulate files would come in handy.

```csharp

```

---

## Asynchronous Functions

> .NET compatibility for async/await.

Closures / Futures / Promises?

```csharp
// same async/await keywords as in C#?
// Use .NET Task<T>
async fnAsync: (): Task<U8>
    x = await workAsync()
    return x + 42

// async is implicit by using await?
// should be ok
fnAsync: (): Task<U8>
    x = await workAsync()
    return x + 42

// await is implicit by using async?
// nah - not enough control
async fnAsync: (): Task<U8>
    x = workAsync()
    return x + 42

// multiple tasks parallel
fnAsync: (): Task<U8>
    t1 = work1Async()
    t2 = work2Async()
    return await t1 + await t2
```

---

Interpret the function parameters `(param: U8)` as a tuple. That means that all functions only have only one actual param, which is a single tuple and is passed by reference (as an optimization), but by value conceptually.

All the parameters need to be read-only (which is a bit odd because they may not use the `Imm<T>` type). This does not mean you cannot pass a pointer and change the content that it points too - that still works.

What is the overhead in building the tuple at the call-site?

We could auto-generate a struct for each function's parameters and allow that struct to be created, initialized and used as the function's only parameter. Although that would encourage functions with a large number of parameters - something we don't want...

Associate a parameters structure with a function.

```csharp
// function with parameters
fn: (p: u8, s: Str)
    ...

// compiler based
p = fn#parameters
    p = 42
    s = "42"

// as real struct
// 'fn' is namespace?
p = fn.Parameters
    p = 42
    s = "42"

// call
fn(p)
// same as
fn(42, "42")
```

Will there be an automatic overload of `fn` (using an immutable ptr to the parameter structure instance) or does the compiler unpack the structure at the call site to call the original function?

> Will overloaded functions share one parameter structure with all the parameters - or - each have an individual parameter structure?

> The compiler generated option should allow being mapped from a real object.

---

Function params are specified in order at call-site, we are steering away from by-order for tuples/deconstruction. That would mean that function parameters are to be specified by name only, which can get very verbose.

Perhaps a 'service' function type uses this principle but calls it a 'message' (like gRPC). Would also return a message in that case. Implementation could be gRPC for interop.

---

- simulate properties? thru type-bound functions?
Get\<T> / Set\<T> / Notify\<T> / Watch\<T[]>

- extensions on intrinsic functions (operator implementations)?

- inline functions? use alias syntax?

- top-level function calls/one-time initialization at first access of module (static constructor).

- How to define the entry point of a program? (Main)
Decorator? Fixed Name?

---

declarative code: see if we can find a syntax that would make it easy to call lots of functions in a declarative style. Think of the code that is needed to initialize a GUI with all its controls to create and properties to set.
https://github.com/apple/swift-evolution/blob/main/proposals/0289-result-builders.md

> not having to use `()` on function calls with one parameter could be a help here.

---

Allow functions that have no literal implementation but a generator is called during compilation to supply the implementation. Works with decorators and weak functions references? (Same as Roslyn Source Generators)

```csharp
{FnGenerator}
Fn: <T>(): T _
```

This could be how mapping gets implemented.

---

> TBD

Temporal coupling to function execution (scheduler).

Run this function every 30 seconds.
Call this function after this timeout has elapsed.
