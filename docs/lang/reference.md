# Reference

> References will take the place of pointers. When finished, the pointers documentation will be removed.

A reference is a .NET garbage collected pointer to an object instance.

In .NET the the type determines the kind of reference. Regular classes are reference-types and structs are value-types.

|Type | Description
|--|--
| Reference | The object instance is passed by reference.
| Value | The object instance is passed by value (copied).

A reference type can never be passed by value (without explicitly copying).
A value type can be passed by reference (ref keyword in C#).

---

| Z# | C# |Description
|--|--|--
| struct | record | Reference Type with value semantics
| custom data type | struct | A value object

```csharp
ref         # keyword
Ref<T>      # Wrapper Type
.Ref()      # Conversion Function
```

Use `Ref<T>` type to indicate the type is a reference type or the function parameter is by-ref or the variable is a reference (explicit).

```csharp
// reference type
MyRefType: Ref              // no <T>
MyRefType: Reference        // alternate
    ...
MyDerivedRefType: Ref<U8>   // make an explicit ref-type based on a value-type

fn: (p: Ref<U8>)
    p = 42          // changes value on call site


a = 42
r: Ref<U8> = a.Ref()    // convert 'a' to a Ref (explicit var)
r = a.Ref()             // convert 'a' to a Ref (implicit var)
r: Ref<U8> = 42         // Error! cannot take ref from a literal
```
