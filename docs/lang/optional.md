# Optional Values

There is no null or null-ptr in Z#. There is a syntax to structure the case where a value may not be available. The `?` is used on the type to indicate this.

```C#
valueMaybe: (): U8?
    return _            // to return nothing
    return 42           // to return a value

// fallback when there is no value
v := valueMaybe() ?? 42

// cascade ?? operators
x := v ?? valueMaybe() ?? 101
x := v? or valueMaybe()? or 101

// optional can be used in a logical expression: '?'
if v?
    use(v)
else
    // no var value

// using a match expression
v := match valueMaybe()
    n: U8 => n
    _ => 0
```

The type of an optional:

```C#
valueMaybe: (): U8?
    ...

v: Mut<U8>?        // initialize an optional (default = _)
v = valueMaybe()  // assign fn result
```

It is a compiler error if you use (dereference) an optional without checking. You can pass an optional value to an optional function parameter without problems.

Optional function parameters:

```C#
hasParam: (p: U8?): Bool
    return p ? true : false     // bah!
    return p?       // syntax support
```

There is no implicit conversions in Z#. So p cannot be implicitly converted to a Bool value and therefor `return p` is invalid.

---

Syntax to test if an optional value is set.

```csharp
boundFn: (self: MyStruct): Opt<MyStruct>
    ...

s := MyStruct
    ...

// syntax TBD
_ = s?.boundFn()?.boundFn()
_ = s&.boundFn()&.boundFn()
```

---

## Optional fields in Structs

```C#
MyStruct
    f1: U8
    f2: Str?

s := MyStruct
    f1 = 42,    // not optional, must have a value
    f2 = _      // no value
```

```C#
MyStruct
    f1: MyStruct?   // data recursion only allowed when optional
    f2: U8          // actual payload

s := MyStruct
    f1 = ...

o := s.f1?.f1?.f2  // first non-value optional will stop navigation of path, result in _
```

> It is not recommended to use optional in general data structures because it does not clarify WHEN that data will or will not be available.

---

## Optional Inference

> TBD: changes if an error is reported.

```csharp
errIfNot42: (p: U8): Bool!
    return p = 42 ? true : Error('Not 42')

b := errIfNot42(42)      // true
b := errIfNot42(101)     // Error!

b: Opt<Bool> = errIfNot42(42)   // true
b: Opt<Bool> = errIfNot42(101)  // Nothing
```

This will wrap the call in a try-catch (and look for a logger in the context?) and return Nothing on an Error.

---

## Option Matching

Have a helper for matching optional values.

> `map<R>(self: Opt<T>, fn: Fn<(v: T):R>): Opt<R>`

```csharp
o: Opt<U8>

// maps Nothing => Nothing
// calls lambda with value, return result
x := o.map(v -> v + v)
// x: Opt<U16>
```

The map function is the same as the .NET Select method.

Do we also need a `Bind` (SelectMany) function?

> `bind<R>(self: Opt<T>, fn: Fn<(v: T):Opt<R>>): Opt<R>`

---

> TBD: Adding or removing optional to an existing declaration is a breaking change, when at a logical level it should be considered a compatible change in most cases... How could we fix that?

---

Functional implication?

> .NET Interop: we could have Linq functions (`Select`, `SelectMany` etc) overloads for `Opt<T>` types so the optional value can be used in function composition/flow.

```csharp
// Opt<T> promotes to Opt<R> by 'selector'
Select: <T, R>(self: Opt<T>, selector: Fn<T, R>): Opt<R>
    if self.hasItem
        return Opt<R>(selector(self.Item))
    else
        return Opt<R>()   // empty

// handle 'nothing' case
Match: <T, R>(self: Opt<T>, none: R, some: Fn<T, R>): R
    return self.hasItem ? some(self.Item) : none
```

`Opt<T>` does not expose any public properties to access the contained item `T` or an indication if the item is set - except for the boolean `?` convertor syntax. Any work is done inside the Linq `Select` or `SelectMany`...

```csharp
date: Opt<DateTime> = TryGetDate(...)
days: Opt<I32> = TryGetDays(...)

// work inside Select.
endDate: Opt<DateTime> = 
    // how to select 'days'?
    // what if days Opt is not set?
    date.Select(d => d.AddDays(days))
```

> TBD: We may add the `Nullable<T>` members for compatibility.

---

> TBD

Boolean operators on optional values?

---

All .NET Try-methods are to be wrapped in an extension method that returns an `Opt<T>`.
Perhaps also make duplicates for XxxxOrDefault() as XxxxxOrNothing() returning an `Opt<T>`...?
