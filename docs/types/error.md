# Error

The `Error` type is a virtual struct that is managed by the compiler. Conceptually the Error type can be thought of as follows:

```C#
Error
    message: Str        // the error message text
    nested: Error?      // an optional nested error
```

However these fields are not accessible directly, but have accessor functions:

```C#
errorFn() catch(err)
    txt = err.message()             // access error message text
    nestedErr = err.nested()        // nested/inner error object
```

Its use is identified by the `!` symbol.

```C#
couldErr: (p: U8): U8!
    ...
```

The `couldErr` function returns a `U8` but could also return an `Error`. In this sense the `Error` type looks to be transparent, but in fact it is implemented as an union between `Error` and the specified type.

The Error type has a creator function that takes a string of text describing the error.

```C#
Error: (msg: Str): Error
Error: <T: Error>(msg: Str, err: T): T
```

So to return an `Error` from a function:

```C#
couldErr: (p: U8): U8!
    return Error("Failed for value {p}.");
```

As the example shows, a formatted string can be used to reveal extra information about the context where the error occurred in.

See [Errors](../lang/errors.md) for more information on how to handle errors in code and the keywords that can be used.

---

The `Error` type can be used to derive from to create custom errors with extra data fields.

```C#
MyError: Error          // derive from Error
    f1: U8
```

To use this custom Error instead of the default one:

```C#
couldErr: (p: U8): U8!
    err = MyError                   // just like any struct
        err.f1 = p
    return Error("Failed.", err)    // overrides the Error type

v = couldErr() catch(err)
    match err                       // use pattern matching
        myErr: MyError => myErr.f1  // access custom field
```

Wrap this construction code into a function for ease of use:

```C#
MyError: (msg: Str, p: U8): MyError
    err = MyError
        err.f1 = p
    return Error(msg, err);

couldErr(p: U8): U8!
    return MyError("Failed.", p)    // custom fn
```

Using custom Error types, it is possible to make hierarchies and test on the actual Error type in the handler code - using [pattern matching](../lang/match.md).

## Nesting Errors

```C#
errorFn: (): U8!
    return Error("Failed")

couldWork(): U8!
v = errorFn() catch(err)
    return Error("New Error", err)
```

## Standard Errors

```C#
Error.OptionalEmpty
Error.InvalidParam
Error.Failed
Error.NotSupported
Error.NotImplemented
Error.NotExpected

errFun: (): Bool!
    return Error(Error.Failed)
```
