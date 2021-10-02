# Variables

Variables store values of a specific type by name. The name is the [Identifier](../lexical/identifier.md) of the variable.

Here is an example of a literal value being used to initialize the variable `a`:

```C#
a = 42
```

A variable is never uninitialized, even if no explicit value is assigned:

```C#
a               // error! a variable must have type
a: U8           // a is of type U8 and has value 0 (default)
a: U8 = 42      // a is of type U8 and has value 42
a = 42          // a is of type U8 (inferred) and has value 42
a = 0x4242      // a is of type U16 (inferred) and has value 0x4242 (hex)
a = -12345      // a is of type I16 (inferred) and has value -12345 (dec)
s = "zlang"     // s is of type Str and has value 'zlang'
b = true        // b is of type Bool and has value true
```

If no explicit type is given the initialization value is used to infer the smallest type from it. The `Bit<T>` type is never considered.

These are not valid:

```C#
a: U8 = true    // error! Bool cannot be 'converted' to U8
a: U8 = "zlang" // error! Str cannot be 'converted' to U8
```

The names of a variable must be unique inside the root scope they're used in:

```C#
myFunc: ()          // root scope
    a: U8           // ok, a
    if true
        a: U8       // error! a is shadowed
```

Root scopes are usually function-scope or for 'global' variables it is file-scope.

---

## Pointer Variables

A pointer variable contains a memory location of the thing it points to.

A specific parameterized type is used to express pointers: `Ptr<T>`:

```C#
a = 42          // U8
p = a.Ptr()     // p of type Ptr<U8> points to a
```

A pointer of any variable can be obtained by using the `Ptr()` conversion function.

```C#
a = 42          // U8
p = a.Ptr()     // p of type Ptr<U8> points to a
b = p()         // b = 42

p() = 101       // a = 101, b = 42
```

See also [Pointers](./pointers.md).

---

## Immutable Variables

When variables are immutable they cannot be changed during their lifetime - their value is constant.

An immutable variable is initialized when declared (not assigned to later).

```C#
c: Imm<U8> = 10  // c has value 10 and cannot be changed

c = 42          // error! cannot change value
a = c           // now a is immutable as well (??)

v = 42          // mutable U8
c: Imm<U8> = v  // ok, immutable copy of v
```

See also [Immutable Types](types.md#Immutable-Types).

---

## Global Variables

Because only immutable variables can be exported, only immutable variables can be thought of as being accessible globally.

The lifetime of any variable can be global, in the sense that the variable maintains its state, but always within the scope of the file it is defined in.

> TBD: in file scope or module (multiple files) scope?

> TBD global variables that can only be accessed locally (function).

```csharp
// cannot see it uses global state in capture...
fn: (): U8
    // global state accessed locally
    ::global: U8 = 42       // syntax?
    return global

fn2: [global]()     // error!
    ...

// use capture to declare?
fn: [global: U8](): U8
    global += 1
    return global
```

---

## Context Variables

> TBD

See this as local dependency injection for functions.

This helps with passing necessary but non-informative parameters.

`with` keyword starts a scope that contain the specified type instances. A list of instances can be specified separated by a comma (default list separator).

> What happens when multiple of the same type are specified? Compile Error? Match Array\<T>?

```csharp
fn: (self: MyStruct, p: U8)
    ...
pred: (p: U8): Bool
    return p = 42

s = MyStruct
    ...
v = 42

with s, v
with (s, v)     // use '()' for lists?
    // matches parameters on type (U8)
    if pred()
        // specified U8 -> overrule context
        fn(101)
```

> Can `self` parameters also come from context? Is there a reason to prohibit that?

Nested `with` contexts are stacked with a reference to its parent. That means that existing types can be overridden with new values and type-lookup is done from most nested (or current) context up to the root context. The value of the first context that has the type registered, will be used.

Another use of the `with` keyword.

```csharp
MyType: (p: U8): MyType
    ...
MyFn: (self: MyType, str: Str)
    ...

t = MyType(42)
with t
    .MyFn("blabla")

// or
with MyType(42)
    .MyFn("blabla")
```

---

## Scoped Context

A context object instance passed to all functions implicitly that is cleared at a certain boundary / valid for a certain scope.

Can contain loggers, temp preallocated memory buffers and any application specific data.

```csharp
// some syntax for static bound functions (namespace or struct)
Context.LogInfo("Context demo.")

ctx = Context::Replace(Context
    Logger = MyLogger
)
// schedule pop context
defer Context::Replace(ctx)

// will use new context
fn(42)

// end of scope pops context (defer)
```

---

> use Lazy\<T> .NET type as is?

> Use Nullable\<T> as is?
