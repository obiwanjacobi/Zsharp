# Interfaces

Interfaces are the means to polymorphism without using objects (in an OOP sense).

---

## Function Interface

Function interfaces are a prototype for a singe function. Usually used as a callback or delegate. 

A named function type.

A function interface declares only one function and does _not_ use the `self` keyword.

```C#
FunctionInterface: (p1: U16): U8 _

callFn(ptrFn: Ptr<FunctionInterface>): U8
    return ptrFn(0x4242)

fnImpl: FunctionInterface
// may use declaration for readability
fnImpl: (p1: U16): U8
    return p1.U8([4..12])

// matches on fn signature
r = callFn(fnImpl)      // r = 0x24
```

The function interface is simply a function declaration. It ends with a `_` to indicate it is not a function definition and contains no implementation.

Function interfaces are syntactically different from Object interfaces - so an object interface with one function cannot be mixed up/interchanged with a function interface.

---

## Object Interface

Object interfaces are a template for one or more functions. Usually used as a means to polymorphism.

An object interface can declare one or more functions. It must have the `self` keyword as a first parameter.

```C#
ObjectInterface<#S>
    lowByte: (self: S, p1: U16): U8 _
    hiByte: (self: S, p1: U16): U8 _
```

The Type of `self` is set as a template parameter, for it is not known (or fixed) at this point.

Also note that there is no implementation `_`.

An interface can have more template parameters however:

```C#
TemplateInterface<#S, #T>
    lowByte: (self: S, p1: U16): T _
    hiByte: (self: S, p1: U16): T _
```

Normal template parameter restrictions can be applied:

```csharp
// recommended way to restrict self
MyStruct
    ...
RestrictedInterface<#S: MyStruct>
    lowByte: (self: S, p1: U16): U8 _
    hiByte: (self: S, p1: U16): U8 _
// The interface can only be implemented on MyStruct (or derived) types.

CompanionInterface<#S: TemplateInterface>
    fn: (self: S, p1: U8): Str _
// The interface can only be implemented on types that also implement TemplateInterface (with any T).
```

---

### Implement an interface

```C#
MyInterface<#S>
    interfunc: (self: S, p: U8) _

MyStruct
    ...
// function name must match and template parameters must satisfy restrictions (none here)
interfunc: (self: MyStruct, p: U8)
    ...

// make struct instance
s = MyStruct
    ...

// will check if all interface functions are implemented for MyStruct
// it is a 'pointer' to the interface
a: MyInterface = s

// call (both the same)
a.interfunc(42)         // because 'self'
interfunc(a, 42)
```

The interface implementation functions are matched based on the function name, template parameters, function parameter types and its return type. The `self` parameter type may be derived from other types but must match exactly.

A compile time error is generated when the compiler detects that an interface is not fully implemented for a specific type of `self`.

---

### Test for Interface Implementation

How to test dynamically (at runtime) if an object implements an interface?

```csharp
// interface definition
MyInterface<#S>
    fn1: (self: S, p: U16): U8 _

// struct definition
MyStruct
    ...

// interface implementation on MyStruct
fn1: (self: MyStruct, p: U16): U8
    ...

// MyStruct instance initialization
s = MyStruct
    ...

// non-optional interface type will Error if not implemented
i: MyInterface = s
// optional interface type will be 'Nothing' is not implemented
o: MyInterface? = s

// test before use
if o
    a = o.fn1(42)
```

The compiler has to check if the specified `self` type has implementation for all the functions of the interface.

Here are the options:

```csharp
MyInterface<#S>
    interfunc: (self: S, p: U8) _

s: Struct
    ...

// non-optional => Error if not exists
i: MyInterface = s
// optional => 'Nothing' if not exists
o: MyInterface? = s

// use builtin functions for runtime checking
b = s.Is<MyInterface<Struct>>()     // retval: Bool
i = s.As<MyInterface<Struct>>()     // cast/convert (Opt<T>)

// use intrinsic/pragma for compile time checking
b = s?#MyInterface<Struct>  // similar to check if field exists
```

---

>TBD

Allow interface definition with types?

```csharp
ObjectInterface<#S, #T>
    fld1: U8
    fld2: Str
    fn1: (self: S, p1: U16): U8 _
    fn2: (self: T, p1: U16): U8 _
```

---

>TBD

Interfaces as traits. Traits are aspects or attributes of an object.

`IHandleMessages`, `IProvideConfiguration`, `IConvertToString`, `ISerialize`...

---

Interface Jackets: a wrapper around something else that implement a certain interface. Think Extension Methods for objects/interfaces.
I think Swift uses protocol for this?

---

Static interfaces?

An interface on a type definition (not an instance).
Derived Types can override functions and call 'base' implementations.

```csharp
// as a specialized template?
TypeInterface<MyStruct>
    staticFn: (p: U8): U8
        // implementation here

TypeInterface<DerivedFromMyStruct>
    staticFn: (p: U8): U8
        // call 'base' as normal 'static' function
        return MyStruct.staticFn(p)
```
