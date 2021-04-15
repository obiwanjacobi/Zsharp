# Templates

Templates are processed at compile time. A template has one or more template parameters inside `< >`.

## Template Structures

```C#
MyStruct<T>
    f: T

s = MyStruct<U8>
    f = 42
```

This will also work:

```C#
MyStruct<T>: T
    ...

s = MyStruct<OtherStruct>
```

---

## Template Functions

```C#
typedFn: <T>(p: T)
    ...

// type inferred
typedFn(42)
// type explicit
typedFn<U8>(42)
```

```csharp
// return values
typedRet: <T>(): T
    ...

x = typedRet<U8>()
y: U8 = typedRet()  // type forwarding?
z = typedRet()      // Error! cannot determine type
```

---

## Template Parameters

Template parameters are applied at compile time. A parameter name (first char) _MUST_ be capitalized when it is used as a Type.

> If Templates are nested `MyStruct<Array<U8> >` there __must__ be a space between each closing `>` angle bracket. Otherwise the current parser will interpret it as a `>>` bit shift right operator.

### Restricting Template Parameters

You might want to use a template with type restriction instead of a normal functions or struct with just the type, in order to keep the specific type without having to cast. For instance in case of a function return type or a structure field.

```C#
MyStruct
    ...
OtherStruct: MyStruct       // derive from MyStruct
    ...

typedFn: <T: MyStruct>(p: T)  // accept type (derived from) MyStruct
    ...

o = OtherStruct             // instantiate
    ...

typedFn(o)                  // type inferred and checked
typedFn<MyStruct>(o)        // base type explicit
typedFn<OtherStruct>(o)     // derived type explicit
```

Restricting the allowed types for a template parameter by using a constrained variant.

```csharp
Choice: U8 or U16 or U24
tfn: <T: Choice>(p: T): U8
    ...

a = tfn(42)     // ok, U8
a = tfn("42")   // error, Str not in Choice
```

Restrict the template parameter based on metadata.

```csharp
// restricting on metadata?
// Type rules syntax? See Custom Data Types
TemplateType<T#bits=8>  // '=' conflicts with parameter default
    field: T

// in combination with parameter default
TemplateType<T#bits(8)=U8>  // not same as type rules syntax
    field: T

// special custom data type syntax?
ParamType: _
    #bits = 8
// apply rules to T and have parameter default (U8)
TemplateType<T: ParamType=U8>
    field: T
```

### Type Template Parameter Inference

Type parameters can be inferred from the context they're used in.

```csharp
templateFn: <S, T, R>(s: S, p: T): R
    ...

// try partial ? => Error
r = templateFn<U16, U8>(42, 101)    // unclear what types are specified

// parameters inferred, return type specified? Error?
r = templateFn<U16>(42, 101)

// ok, specified explicitly
r = templateFn<U8, U8, U16>(42, 101)

// ok, parameters and return type inferred
r: U16 = templateFn(42, 101)
```

### Non-Type Template Parameters

Although template parameters are usually types, it can be anything.

```C#
// non-type template params
FixedArray<T, count: U8>
    arr: Array<T>(count)
```

We also allow function pointers to be specified as template parameters.

```csharp
call<fn: Fn<(I32): I32>>(p: I32)
    return fn(p)

negate: (i: I32): I32
    ...
absolute: (i: I32): I32
    ...

a = 42
b = call<negate>(a)     // -42
c = call<absolute>(a)   // 42
```

### Code Template Parameters (inlining)

Have a code block be substituted for a template parameter.
For that we need a compile-time code reference / function pointer.

The goal is to insert code into a template that is compiled as a new whole.

```csharp
// takes a void-function with an U8 param 'as code' (#)
repeat: <fn: #Fn<Void, U8>>(c: U8)
    loop n in [0..c]
        fn(n)           // <= syntax to be determined

// use #! to not emit the fn in the binary
#! doThisFn: (p: U8)
    ...             // <= body is inserted into the template

// compiled as a new function (body)
repeat<doThisFn>(42);
// will execute doThisFn (body) 42 times (p=0-41)
```

The special syntax `#` (TBD) on the template parameter makes a distinction between passing in a function pointer (`Fn<T>`) -resulting in a function call- and copying in the code (`#Fn<T>`) which is basically inlining explicitly. Inlining allows the compiler to optimize the resulting code as a whole.

### Template Parameter Defaults

As with function parameters, template parameters can be set to a default value that can be overridden at the 'call site'.

```C#
TemplateType<T=U8>
    field1: T

// use default
t = TemplateType
    field1 = 42         // U8

// override default
t = TemplateType<Str>
    field1 = "42"       // Str
```

> Note: Inconsistency with function parameter defaults => which are not supported. Should we support both or neither?

### Variable Number of Template Parameters

> Not supported.

We really want to keep this as simple as possible.

---

## Template Specialization

When use of specific template parameter values require specific code.

```C#
typedFn: <T>(p: T)
    ...
// Identified to be a specialization by name and function type (pattern).
typedFn: <Bool>(p: Bool)
    ...

typedFn(42)         // generic typedFn<T> called
typedFn(true)       // specialization typedFn<Bool> called
```

### Partial Specialization

```csharp
templateFn: <S, T>(s: S, t: T): Str
    ...

// specialized for first parameter of Bool.
templateFn:<Bool, T>(s: Bool, t: T): Str
    ...
```

---

> TBD

Allow for multiple/nested levels of type params?

```csharp
MyType<M<T>>    // requires M to have one T
    ...         // use M and T?
```

With restrictions:

```csharp
MyType<M: Struct<T: OtherStruct>>
```

---

> TBD

For .NET interoperability we need to distinguish between .NET generics and Z# compile-time templates.

```csharp
// run time
generic<T>
    ...

// compile time
template<#T>
    ...

// combination
hybrid<#T, G>
    ...
```

---
