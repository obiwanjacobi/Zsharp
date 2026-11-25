# Errors

Z# knows two kinds of failure, and they are kept separate:

| Kind | Type | Marker | Nature |
| -- | -- | -- | -- |
| Error | `Err<T>`: an `Error` value or `T` | `T!` | A value returned from a function. Cheap. Used for failures the caller is expected to deal with. |
| Exception | .NET `Exception` | `T!!` | Thrown, as in .NET. Used for exceptional conditions and everything coming out of .NET code. |

Although they are different things, they are _handled_ the same way:

- **Unhandled, they propagate automatically to the caller.** There is no `try`, `catch` or propagation operator.
- **Handling is done with a `match` expression.** Errors and exceptions are matched by their type, next to the normal value.

Error types and exception types are distinct type hierarchies: an `Error` type is never an `Exception` type and vice versa. That is what keeps the two apart in a `match`.

> For expected absence of a value (not found, could not parse) use `Opt<T>` instead. See [Optional](optional.md).

---

## Returning an Error

The `!` on the return type indicates the function can return an error.

```csharp
errorFn: (): U8!
    return Error("Failed")
```

Details on [using the `Error` type can be read here](../types/error.md).

It is also possible to return an error from a function that has no return value.

```csharp
voidFn: ()!
    return Error("Failed")  // ok, exit function
```

A function without `!` cannot return an error.

```csharp
noErrorFn: ()
    return Error("Failed")  // error! no error return type
```

---

## Throwing an Exception

The `!!` on the return type documents that the function may throw an exception.

```csharp
exceptionFn: (): U8!!
    ArgumentException("Not good", "param1").throw()
```

> `.NET`: Any call into .NET code may throw, so `!!` cannot be complete. It is informational. The compiler can check it within Z# code.

`throw` and `rethrow` are functions bound to the `Exception` type that will be emitted as a C# `throw` statement.

```csharp
// use named parameters (param names of imported assemblies are available through reflection)
ArgumentException(message="Not good", paramName="param1").throw()
```

---

## Automatic Propagation

When the result of a function call is not matched, its value is used as the plain type `T`. If the call fails, the failure is passed on to the caller.

```csharp
couldWork: (): U8!
    ...

myFunc: (): Bool!
    b := couldWork()    // b: U8 (not Err<U8>)
    use(b)              // only reached when couldWork succeeded
```

### Errors need a carrier

An `Err` value can only propagate if the calling function can return it: its return type must also be `!`. Otherwise it is a compile error and the error must be handled with a `match`.

```csharp
noErrorFn: (): Bool
    b := couldWork()    // error! unhandled Err: match it or mark the function '!'
```

This keeps errors from being ignored, and the function signature shows that it can fail.

### Exceptions always propagate

Exceptions propagate as they always do in .NET, whether the calling function is marked `!!` or not.

---

## Handling with Match

A `match` on a function call handles the value, the errors and the exceptions in one place.
The value cases must be covered. Error and exception arms are optional: anything not matched propagates (see above).

```csharp
v := match readConfig(path)
    cfg: Config -> cfg                          // the value
    e: ConfigError -> Config.Default            // an Err value
    ex: FileNotFoundException -> Config.Empty   // an exception
    // anything else propagates to the caller
```

Match any error or any exception by their base types:

```csharp
v := match couldWork()
    n: U8 -> n
    err: Error -> 0                 // any error
    ex: Exception -> 0              // any exception
```

An arm can exit the function or pass on a different failure:

```csharp
a := match couldWork()
    n: U8 -> n
    custom: MyError -> return custom.fld        // return exits the function
    err: Error -> return Error("Failed", err)   // wrap and return a new error
```

To rethrow an exception explicitly:

```csharp
v := match exceptionFn()
    n: U8 -> n
    ex: IOException ->
        log(ex)
        ex.rethrow()
```

Handling failures of multiple statements together (the former `try` block). That requires `match` to accept a code block, which works with local functions or if code blocks are expressions (see [Functions](functions.md)).

```csharp
// using local functions as scope
steps: (): U8!
    a := step1()
    b := step2(a)
    return step3(b)

v := match steps()
    n: U8 -> n
    err: Error -> 0

// using capture and code-block as expression (new syntax)
v := match
    |a|                 // capture block as the subject
        b := step2(a)
        step3(b)
    n: U8 -> n
    err: Error -> 0
```

A shorthand for "on any error use this value". The twin of `??` for optionals. This does not cover exceptions.

```csharp
parse: (s: Str): U8!
    ...

// only for Err<T> errors, not exceptions.
x := parse(s) !! 0
```

> Using this syntax means ignoring any errors. Not to be used lightly.

---

## Cleanup

Cleanup is done with `defer` (always) and `errdefer` (only when leaving with a failure). See [Defer](defer.md).

```csharp
fn: ()!
    f := open(path)
    defer f.Close()         // always
    errdefer log("failed")  // only when an error or exception leaves this function
    ...
```

> `errdefer` runs for both a propagating `Err` and a propagating exception.

---

## Fatal Errors

What some call 'panic', also known as exit().

For some types of errors you want to crash and have diagnostics.

```csharp
// exits program
FatalError("panic!")
FatalError("panic!", err)
```

A `FatalError` can be triggered anywhere, also in functions that do not return errors.

> What errors are fatal? DivideByZero, StackOverflow, OutOfMemory, ... ??

---

## Error Trace

When an error is returned from a function and it naturally bubbles up the call stack, a trace can be made of all the code sites it visits.

This diagnostic information can be useful for tracking down problems.

---

## Interop

> `.NET`: A .NET assembly will see the `Err<T>` type as a `OneOf<T>` of the Z# Error object and T.

> What if multiple transitions between errors and exceptions take place? How do you preserve the original error/exception type? Have an `Exception` field in `Error` and store error instances in context data (under a fixed key) in an exception.

> How to access standard errors (make a list of standard errors?)?
