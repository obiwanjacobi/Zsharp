# Map

A key-value map.

```C#
capacity = 42
m = Map<Str, U8>(capacity)

m["key1"] = 1
m["key2"] = 2
m["key3"] = 42
```

Str-to-U8 map??

```C#
map = ["key1" = 1, "key2" = 2, "key3" = 42]
```

> TBD what would be the syntax to declaratively define a map?

---

> TBD

Support mapping a map to and from a structure.

```csharp
s = Struct
    fld1 = 42
    fld2 = "42"

// untyped map to allow any value type?
m: Map <= s
m: Map<Str, Any> <= s   // or explicitly typed
// 'fld1' = 42
// 'fld2' = '42'

// map back to structure
x: Struct <= m
```

Allow maps to be passed as parameters to any function.

```csharp
fn: (p1: U8, p2: Str)
    ...

// use tuple syntax to create maps?
m = {p1 = 42, p2 = "42"}
fn(m)

fn2: (p1: U8, p2: Str, p3: Opt<U16>)
    ...

// ok, optional p3 is not specified (Nothing)
fn2(m)
```

> Keys MUST match field (struct) or parameter (function) names!

---

> For .NET interop we could use `Dictionary<TKey, TValue>` and not use a `Map` type at all.
