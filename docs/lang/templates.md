# Templates and Generics

Templates are processed at compile time. A template has one or more template parameters inside `< >`.

In light of supporting .NET generics the type names for templates are to be prefixed with a `#` on the definition to indicate compile-time processing, where as .NET generics are not prefixed - to indicate runtime processing.

Naming Convention for template and generic type parameters:

| Name | Application | Description
|--|--|--
| R | Return Type | Return type of a function
| S | Self Type | Self function parameter type
| T | Item Type | Use when only a single template parameter.

All other template and generic parameter names start with a capital letter and end with `T`.

Example:
`transform<#InputT, #OutputT>(input: InputT, output: OutputT)`

---

## Structure Templates

```csharp
MyStruct<#T>
    f: T

s : MyStruct<U8> =
    f = 42
```

This will also work:

```csharp
MyStruct<#T>: T
    ...

s : MyStruct<OtherStruct>
```

As well as

```csharp
CRTP<T>
    Super: T

Usage : CRTP<Usage>
    //Super: Usage
```

CRTP: https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern

---

## Function Templates

```csharp
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

x := typedRet<U8>()
y: U8 = typedRet()  // type forwarding?
z := typedRet()      // Error! cannot determine type
```

---

## Template Parameters

Template parameters are applied at compile time. A parameter name (first char) _MUST_ be capitalized when it is used as a Type.

In most cases Template Type parameters can be inferred from it's usage. Explicitly specifying them is only needed when inference is ambiguous.
`_` can be used in places where the Template parameter needs to be inferred when in combination with other type parameters that are explicitly stated.
`fn<_, Str>(42, "42")`
All type parameters can be omitted when they can be inferred from usage.

---

### Restricting Template Parameters

You might want to use a template with type restriction instead of a normal functions or struct with just the type, in order to keep the specific type without having to cast. For instance in case of a function return type or a structure field.

```csharp
MyStruct
    ...
OtherStruct: MyStruct       // derive from MyStruct
    ...

typedFn: <#T: MyStruct>(p: T)  // accept type (derived from) MyStruct
    ...

o = OtherStruct             // instantiate struct
    ...

typedFn(o)                  // type inferred and checked
typedFn<MyStruct>(o)        // base type explicit
typedFn<OtherStruct>(o)     // derived type explicit
```

Restricting the allowed types for a template parameter by using a constrained variant.

```csharp
Choice: U8 or U16 or U32
tfn: <#T: Choice>(p: T): U8
    ...

a := tfn(42)     // ok, U8
a := tfn("42")   // error, Str not in Choice
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
f := factory<U8>()       // f: U8 (0 = default)
// function MyStruct() must exist
c := create<MyStruct>()  // c: MyStruct (all fields default)
```

---

### Type Template Parameter Inference

Type parameters can be inferred from the context they're used in.

```csharp
templateFn: <#S, #T, #R>(s: S, p: T): R
    ...

// try partial ? => Error
r := templateFn<U16, U8>(42, 101)    // unclear what types are specified

// Error, parameters inferred, but what type is specified?
r := templateFn<U16>(42, 101)

// ok, specified explicitly
r := templateFn<U8, U8, U16>(42, 101)

// ok, parameters and return type inferred
r: U16 = templateFn(42, 101)

// ok, explicitly name the template param
r := templateFn<R=U8>(42, 101)
```

A (self) bound function can use type inference to discover the template/generic parameters used in its `self` parameter.

```csharp
MyType<G, #T>
    GenFld: G
    TmplFld: T

// type inference fixes the issue
boundFn: <G, #T>(self: MyType<G, #T>)
    ...
```

Can we get this to work also?

```csharp
transform: <T: Transform<S, D>, S, D>(self: T, source: S): D

// from the specified 'T' the 'S' and 'D' types are inferred.

t: Transform<Struct1, Struct2> = ...
s1 := ...
s2 := transform(t, s1)
```

---

### Non-Type Template Parameters

Template parameters that are not type parameters can be specified in a similar fashion as a regular function parameter. However, the value is applied to the code (replaced if you will) at compile time. The name does not have to be capitalized. The compile time nature of the non-type template parameter is indicated by using a `#` before its name.

```csharp
// use '#' before name to indicate a compile-time template param
repeatX: (#count: U16)
    loop count      // don't use '#' in code
        ...
```

```csharp
// not using '#' before name makes it a normal parameter
repeatX: (count: U16)     // present at runtime
    ...
```

```csharp
// Error: cannot use the same name for template and function parameters
repeatX: (#count: U16, count: U16)
    ...
```

Example

```csharp
fn<#T>(#ret: T): T
    return ret

// Can we infer T?
n := fn<42>()        // n: U8 = 42
s := fn<"Hello">()   // s: Str = 'Hello'
```

Dimensioning data structures. Structure parameters.

```csharp
DataStruct(#count: U16):
    Names: Str[count]           // <= TDB
    Names: Array<Str>(count)    // or this?

// both type template and compile-time parameter
DataStruct<#T>(#init: T):
    fld1: T = init

s := DataStruct<U8>(42)

// can structures also have normal parameters?
// interpret this as a primary constructor?
// No: we have syntax for constructing a struct
DataStruct(count: U16):
    Names: Str[count]           // <= TDB
    Names: Array<Str>(count)    // or this?

//!!!! ambiguous syntax
// function calls look the same as type instantiation
s := MyStructure<U8>(42)    // type init
f := MyFunction<U8>(42)     // function invocation

// fix by changing the type syntax
s : MyStructure<U8>(42) = { ... }   // type init
f := MyStructure(42)                // constructor/convertor function
```

> We don't have syntax for statically dimensioning an array (list), yet!

Special cases?

In both cases the `T` can be inferred from usage.

```csharp
fn: <#T>(ptr: Ptr<T>)
    ...

fn: <#T>(#ptr: Ptr<T>)
    ...
```

---

### Code Template Parameters (inlining)

> TBD

Have a code block be substituted for a (non-type) template parameter.
For that we need a compile-time code reference / function pointer.

The goal is to insert code into a template that is compiled as a new whole. The function will (probably) be inserted into the template instantiation as a local function.

```csharp
// takes a (non-type) function template parameter 'as code'
repeat: (c: U8, #fn: Fn<U8>)
    loop n in [0..c]
        fn(n)   // need '#'? => no

// optionally use #! to not emit the fn in the binary
#! doThisFn: (p: U8)
    ...             // <= body is inserted into the template

// compiled as a new function (body)
repeat(42, doThisFn)
// will execute doThisFn (body) 42 times (p=0-41)
```

---

### Template Parameter Defaults

Template parameters can be set to a default value that can be overridden at the 'call site'.

```csharp
TemplateType<#T=U8>
    field1: T

// use default
t := TemplateType
    field1 = 42         // U8

// override default
t := TemplateType<Str>
    field1 = "42"       // Str
```

---

### Variable Number of Template Parameters

> Not supported.

We really want to keep this as simple as possible.

`.NET` does not support a variable number of generic type parameters.

---

## Template Alias

A new name for an existing template resolved at compile time. The alias name will not appear in the binary.

```csharp
// template type alias
AliasTemplate<#T1, #T2> = SomeType<T1, OtherType<T1, T2>>
// template function alias with partial application
templateFn<#T> = fnTempl<T, U8>
```

Aliases can be exported from a module to be reused within the assembly.

---

## Template Specialization

When use of specific template parameter values require specific code.

```csharp
typedFn: <#T>(p: T)
    ...
// Identified to be a specialization by name and function type (pattern).
typedFn: <Bool>(p: Bool)    // repeat '<Bool>'? (yes)
typedFn: (p: Bool)          // or infer from usage? (no, this is a non-template overload)
    ...

typedFn(42)         // default typedFn<T> called
typedFn(true)       // specialization typedFn<Bool> called
```

When all template parameters are specialized a concrete type or function is created that is used as a template instantiation.

---

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

For .NET interoperability we need to distinguish between .NET generics and Z# compile-time template parameters.

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

Depending on our choice of how to represent Z# structs in .NET - which will probably be `record`s, a `class` constraint will be added by default. If the compiler can determine it is safe for value types as well (for example in Z# structs), it can be omitted (only when no explicit type restriction was specified).

---

## Duck Typing

As an example here's a template that requires the type to have a property `Name` but it does not matter what Type it is specifically.
Dependencies in templates are not formalized but are deferred until actual compilation.

```csharp
// template function
GetNameFrom: <#S>(self: S): Str
    return self.Name

MyStruct
    Name: Str

s := MyStruct
    Name = "Name"
n := GetNameFrom(s)  // ok, name field

// anonymous struct
a := { Name = "MyName" }
n := GetNameFrom(a)  // ok, name field

x := 42
n := GetNameFrom(x)  // Error - no Name field
```

This allows a sort of duck-typing. As long as the `self` parameter has a `Name` field the code can be compiled.

---

> TBD

Allow for multiple/nested levels of type params?

```csharp
MyType<#M<#T>>    // requires M to have one T
    ...         // use M and T?
```

With restrictions:

```csharp
MyType<#M: Struct<#T: OtherStruct>>
```

---

> TBD

Usage of the same template parameter at different places.

```csharp
// usage
MyType<OtherType<SameType1, SameType2>, SameType1, SameType2> myType

// type template alias
MyOtherType<T1, T2> = MyType<OtherType<T1, T2>, T1, T2>
// usage of alias
MyOtherType<SameType1, SameType2> myOtherType

// use template param as default for other template param
ReuseType<T1, T2=T1>
    ...
```

---

> TBD

- Generics: Auto extract a non-generic base representation that contains all non-generic code of a templated generic definition. This extraction considers all types and their (self) bound functions without any generic parameters. The result would be a C# base class with all the non-generic members and derived from that the specific generic class with all the generic members. Perhaps have a code decorator to opt-in into this 'feature'.

---

The source code of any templated functions and types that are public (exported) are stored as a resources in the resulting assembly. This way the source code can be reused by the Z# compiler when an external module tries to use the templated function or type.

Instead of the actual source code, we could also store the serialized AST for performance - but that would make it compiler version dependent.

Perhaps also generate a class with loader methods for each symbol name that can have custom code attributes for the Z# compiler to discover.

---

Have syntax for instantiating templates separate from using the template. After which the new concrete function can be passed around.

```csharp
fnT: <#T>(p: T): T
    ...

// some sort of compiler directive
fn := #fnT<I32>

// as I32
v := fn(42)
s := fn("42")   // error, type needs to be I32
```
