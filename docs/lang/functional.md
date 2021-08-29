# Functional

Z# also contains a couple of functional programming features that are described here.

A pure function is a function that returns the exact same result given the same inputs without any side effects.

A pure function -without side-effects- can be recognized by the lack of (mutable?) captures and the presence of immutable (only in-) parameters. It also has to have a return value.

> This is not always true (a database read function may return different results for the same inputs - even if we explicitly capture the non-mutable database connection) so we may need to introduce special syntax to indicate pure functional functions...

```txt
Pure functions can only call other pure functions. 
Impure functions (with side-effects) can also call pure functions.
Pure functions cannot call impure functions, ever.
```

The question is can the compiler detect that.

```csharp
// has imm param but potentially writes to globalVar
sideEffect: [globalVar.Ptr()](p: Ptr<Imm<U8>>)
// takes two params and produces result - no side effects
pureFn: (x: U8, y: U8): U16

// seems ok because capture is immutable
// but what if it is a database connection?
// then the function can still return different results for the same args
pureFnMaybe: [globalVar](x: U8, y: U8): U16

// special syntax to promise purity?
// on the return type? (haskal uses IO for impure)
pureFn: [globalVar](x: U8, y: U8): Pure<U16>
// with a 'pure' keyword?
pure pureFn: [globalVar](x: U8, y: U8): U16
// with a 'fun' keyword?
fun pureFn: [globalVar](x: U8, y: U8): U16
```

## Higher Order Function

A higher order function is a function that takes or returns another function (or both).

```csharp
// a function that returns a function
pureFn: (arr: Arr<U8>): Fn<(U8): U8>
    ...
// call pureFn and call the function it returns
v = pureFn([1,2])(42)
```

> TBD: Syntax for function partial application, composition and currying?

> function composition, partial application and parameter currying should be done inline and at compile time ideally. This would generate more code, but perform better at runtime - as opposed to linking existing functions together. Not sure how to handle external functions that are already compiled.

See also [Piping Operator](#Piping-Operator).

> Composition and value piping are different concepts. Function Composition is building functions from other functions at compile time. Value piping chains (result) values between multiple function calls (result of one function is parameter for next function).

---

## Partial Application

The Expression body syntax is also used for partial application. In this case the function body of the composite function (alias) is not compiled into code but used as a composition template.

```csharp
fn: (p: U8, s: Str): Bool
    ...

// expression body => composition
fnPartial (p: Str) = fn(42, p)

// calls fn(42, "42")
b = fnPartial("42")
```

---

Due to our choices in syntax most of these functional principles have to be spelled out. We could make an exception for not having to specify the `()` [when there is only one (or no - for poor man's property syntax) parameter?].

```csharp
add: (p1: U8, p2: U8): U16
    return p1 + p2
// partial application by hand
add42 (p: U8): U16 = add(42, p)
```

By returning a local function it becomes anonymous. Any references to external variables / parameters need to be captured.

```csharp
fn: (p1: U8, p2: Str): Fn<(U8): U8>
    internalFn: [p2](p1: U8): U8
        return p2.U8() + p1
    return internalFn
```

---

How about (inline) partial application?

Partial application results in an anonymous function that may be compiled to an function impl.

```csharp
fn: (p: U8, s: Str)
    ...

// what syntax to indicate partial application
// instead of trying to call a potential overload?
partialFn = fn(42,)
partialFn("42") // calls fn(42, "42")

// also good
partialFn = fn(,"42")
partialFn(42) // calls fn(42, "42")

// by name? (still need syntax to differentiate overloads)
partialFn = fn(s="42")
partialFn("42") // calls fn(42, "42")
```

> Can you take a reference/pointer to a composite function? If you do, you create an actual function stub with the composition compiled.

> `.NET`: Functions with captures (lambdas) and partial application will probably result in function classes that have members for the captured data and/or the applied (partial) function parameters.

> TODO: look into monads. I don't understand them yet.

---

## Piping Operator

To make nested function calls more readable. More 'functional'.
Would make line-breaks in long statements (chains) a lot more readable?

```csharp
a = fn1(fn2(fn3(42)))
b = fn3(42) |> fn2() |> fn1()
// functional syntax
b = 42 |> fn3 |> fn2 |> fn1
```

Subsequent function calls (after `|>`) will have their 1st param missing. That looks a bit strange (but no different than bound functions?). `()` can be omitted when a function has zero or one parameter?

```csharp
[0..5] |> fn()  // passed in array
```

Could also have a 'backward' piping operator? `<|` going the other way...

```csharp
fn1() <| fn2() <| fn3() <| 42
```

---

> TBD

- Map, Apply, Bind ?? What are the names to use here? https://fsharpforfunandprofit.com/series/map-and-bind-and-apply-oh-my/
