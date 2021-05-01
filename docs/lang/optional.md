# Optional Values

There is no null or null-ptr in Z#. There is a syntax to structure the case where a value may not be available. The `?` is used on the type to indicate this.

```C#
valueMaybe: (): U8?
    return _            // to return nothing
    return 42           // to return a value

// fallback when there is no value
v = valueMaybe() ?? 42

// optional can be used in a logical expression
if v
    use(v)
else
    // no var value

// using a match expression
v = match valueMaybe()
    n: U8 => n
    _ => 0
```

The type of an optional:

```C#
valueMaybe: (): U8?
    ...

v: U8?        // initialize an optional (default = _)
v = valueMaybe()  // assign fn result
```

It is a compiler error if you use (dereference) an optional without checking. You can pass an optional value to an optional function parameter without problems.

Optional function parameters:

```C#
hasParam: (p: U8?): Bool
    return p ? true : false
    return p?       // syntax support
```

There is no implicit conversions in Z#. So p cannot be implicitly converted to a Bool value and therefor `return p` is invalid.

In this case where it needs to be converted to boolean it results in a little verbose (not so nice) code.

Optional fields in structs:

```C#
MyStruct
    f1: U8
    f2: Str?

s = MyStruct
    f1 = 42,    // not optional, must have a value
    f2 = _      // no value
```

```C#
MyStruct
    f1: MyStruct?   // data recursion only allowed when optional
    f2: U8          // actual payload

s = MyStruct
    f1 = ...

o = s.f1?.f1?.f2  // first non-value optional will stop navigation of path, result in _
```

> It is not recommended to use optional in general data structures because it does not clarify WHEN that data will or will not be available.

---

Adding or removing optional to an existing declaration is a breaking change, when at a logical level it should be considered a compatible change in most cases... How could we fix that?

---

Functional implication?

> .NET Interop: we could have Linq functions (`Select`, `SelectMany` etc) overloads for `Opt<T>` types so the can be used in function composition/flow.

```csharp
// Opt<T> promotes to Opt<R> by 'selector'
Select: <T, R>(self: Opt<T>, selector: Fn<T, R>): Opt<R>
    if self.hasItem
        return Opt<R>(selector(self.Item))
    else
        return new Opt<R>()   // empty

// handle 'nothing' case
Match: <T, R>(self: Opt<T>, none: R, some: Fn<T, R>)
    return self.hasItem ? some(self.Item) : None
```

`Opt<T>` does not expose any public properties to access the contained item `T` or an indication if the item is set. Any work is done inside the Linq `Select` or `SelectMany`...

```csharp
date: Opt<DateTime> = TryGetDate(...)
days: Opt<I32> = TryGetDays(...)

// work inside Select.
endDate: Opt<DateTime> = 
    // how to select 'days'?
    // what if days Opt is not set?
    date.Select(d => d.AddDays(days))
```
