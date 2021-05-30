# Templates

Templates are processed at compile time. A template has one or more template parameters inside `< >`.

> In light of supporting .NET generics the type names for templates are to be prefixed with a `#` on the definition to indicate compile-time processing, where as .NET generics are not prefixed - to indicate runtime processing. Note that this is not consistently applied throughout the documentation yet.

Naming Convention for template parameters:

| Name | Application | Description
|--|--|--
| R | Return Type | Return type of a function
| S | Self Type | Self function parameter type
| T | Item Type | Use when only a single template parameter.

All other template parameter names start with a capital letter and end with `T` (for types).

`transform<#InputT, #OutputT>(input: InputT, output: OutputT)`

None-Type template parameters are always processed at compile-time and do not have to be prefixed with a `#`.

## Structure Templates

```C#
MyStruct<#T>
    f: T

s = MyStruct<U8>
    f = 42
```

This will also work:

```C#
MyStruct<#T>: T
    ...

s = MyStruct<OtherStruct>
```

---

## Function Templates

```C#
typedFn: <#T>(p: T)
    ...

// type inferred
typedFn(42)
// type explicit
typedFn<U8>(42)
```

```csharp
// return values
typedRet: <#T>(): T
    ...

x = typedRet<U8>()
y: U8 = typedRet()  // type forwarding?
z = typedRet()      // Error! cannot determine type
```

---

## Template Parameters

Template parameters are applied at compile time. A parameter name (first char) _MUST_ be capitalized when it is used as a Type.

### Restricting Template Parameters

You might want to use a template with type restriction instead of a normal functions or struct with just the type, in order to keep the specific type without having to cast. For instance in case of a function return type or a structure field.

```C#
MyStruct
    ...
OtherStruct: MyStruct       // derive from MyStruct
    ...

typedFn: <#T: MyStruct>(p: T)  // accept type (derived from) MyStruct
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
tfn: <#T: Choice>(p: T): U8
    ...

a = tfn(42)     // ok, U8
a = tfn("42")   // error, Str not in Choice
```

Restrict the template parameter based on metadata (traits?).

```csharp
// restricting on metadata?
// Type rules syntax? See Custom Data Types
// Two '#' chars looks weird
TemplateType<#T#bits=8>  // '=' conflicts with parameter default
    field: T

// in combination with parameter default
TemplateType<#T#bits(8)=U8>  // not same as type rules syntax
    field: T

// special custom data type syntax?
ParamType:
    #bits = 8
// apply rules to T and have parameter default (U8)
TemplateType<#T: ParamType=U8>
    field: T
```

Require a parameterless constructor function.

```csharp
// require parameterless construction
factory<#T: ()>()
    return T()      // the type constructor function must exist

// require specific type (or derived) with parameterless construction
create<#T: MyStruct()>()
    return T()

// function U8() must exist
f = factory<U8>()       // f: U8 (0 = default)
// function MyStruct() must exist
c = create<MyStruct>()  // c: MyStruct (all fields default)
```

### Type Template Parameter Inference

Type parameters can be inferred from the context they're used in.

```csharp
templateFn: <#S, #T, #R>(s: S, p: T): R
    ...

// try partial ? => Error
r = templateFn<U16, U8>(42, 101)    // unclear what types are specified

// parameters inferred, return type specified? Error?
r = templateFn<U16>(42, 101)

// ok, specified explicitly
r = templateFn<U8, U8, U16>(42, 101)

// ok, parameters and return type inferred
r: U16 = templateFn(42, 101)

// explicitly name the template param?
r = templateFn<R=U8>(42, 101)
```

### Non-Type Template Parameters

Template parameters that are not type parameters can be specified in a similar fashion as a regular function parameter. However, the value is applied to the code (replaced if you will) at compile time. The name does not have to be capitalized.

```csharp
fn<#T, ret: T>(): T
    return ret

// Can we infer T?
n = fn<42>()        // n: U8 = 42
s = fn<"Hello">()   // s: Str = 'Hello'
```

Dimensioning data structures.

```csharp
DataStruct<count: U16>:
    Names: Str[count]   // <= TDB
```

> We don't have syntax for statically dimensioning an array (list), yet!

### Code Template Parameters (inlining)

> TBD

Have a code block be substituted for a (non-type) template parameter.
For that we need a compile-time code reference / function pointer.

The goal is to insert code into a template that is compiled as a new whole. The function will (probably) be inserted into the template instantiation as a local function.

```csharp
// takes a function template parameter 'as code'
repeat: <fn: Fn<U8>>(c: U8)
    loop n in [0..c]
        fn(n)   // need '#'?

// use #! to not emit the fn in the binary
#! doThisFn: (p: U8)
    ...             // <= body is inserted into the template

// compiled as a new function (body)
repeat<doThisFn>(42)
// will execute doThisFn (body) 42 times (p=0-41)
```

---

### Template Parameter Defaults

Template parameters can be set to a default value that can be overridden at the 'call site'.

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
typedFn: <Bool>(p: Bool)    // repeat '<Bool>'?
typedFn: (p: Bool)          // or not?
    ...

typedFn(42)         // generic typedFn<T> called
typedFn(true)       // specialization typedFn<Bool> called
```

When all template parameters are specialized a concrete type or function is created that is used as a template instantiation.

### Partial Specialization

Partial specialization means partly specifying the template parameters (not all of them). The result is another template.

Comparable to how function composition works where partially specifying function parameters yields another function.

```csharp
templateFn: <#S, #T>(s: S, t: T): Str
    ...

// specialized for first parameter of Bool.
templateFn:<Bool, #T>(s: Bool, t: T): Str
    ...
```

---

## Generics

For .NET interoperability we need to distinguish between .NET generics and Z# compile-time templates.

```csharp
// run time
generic<G>      // emits 'generic<G>'
    ...

// compile time
template<#T>    // emits 'template'
    ...

// combination
hybrid<#T, G>   // emits 'hybrid<G>'
    ...
```

The .NET `where` constraints can be specified on generic parameter as restrictions in the same way as on template parameters.

---

## Duck Typing

As an example here's a template that requires the type to have a property `Name` but it does not matter what Type it is specifically.

```csharp
// template function
GetNameFrom: <S>(self: S): Str
    return self.Name

MyStruct
    Name: Str

s = MyStruct
    Name = "Name"
n = GetNameFrom(s)  // ok, name field

// anonymous struct
a = { Name = "MyName" }
n = GetNameFrom(a)  // ok, name field

x = 42
n = GetNameFrom(x)  // Error - no Name field
```

This allows a sort of duck-typing. As long as the `self` parameter has a `Name` field the code can be compiled.

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
