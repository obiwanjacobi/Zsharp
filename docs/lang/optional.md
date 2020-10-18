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
