# Capture

Captures are read-only snapshots (copies) or references to contextual state -like local variables- that accompany a child context.

```csharp
// basic capture syntax
x := 42

|x|
    ...
```

---

## Function Capture

Function can use captures to be able to reference global variables in their body. All dependencies of a function are explicitly declared either as normal parameters (implicit or context parameters) or captures.

> TBD: have the function capture syntax look exactly the same as the block-capture. `fn: (p: U8): U8 = |c| ...`

```csharp
x = 42
fn: |x|(p: U8): Bool
    return p = x        // true if p = 42
```

Lambda's can use captures to be able to reference data in their vicinity to use during execution of the lambda.

```csharp
fn: (p: U8, predicate: Fn<(U8): Bool>)
    if predicate(p)
        ...

x = 42
// lambda captures x to compare with
fn(42, |x|(p) -> p = x)

// how would the alternate syntax work with lambdas?
fn(42, (p) -> |x| p = x)
```

> TBD: Can capture of global vars occure anywhere in th function body?

```csharp
x := 42     //global
fn: (): U8
    some_code_here
    |x|     // capture global
        work_with_x_here
    
    x_is_out_of_scope_here
```

---

## Capture Context

An additional syntax is considered for capturing dependencies of any code block. This may be a valuable feature when refactoring code.

```csharp
v := 42
// following code is dependent on v
|v|
    // use v here
```

Captures also may be used as a synchronization mechanism for shared data. At the start of a capture a copy is made of the data and the code (function) works with that copy. The actual value may be changed (by another thread) in the meantime.

In case of a mutable capture, it's value is written back to the original storage when the block of code is completed.

> Should mutable captures be renamed/aliased? `[x = y.Ptr()]`   -- old: don't use Ptr()

That would also suggest that capture blocks themselves could be multi-threading / execute separately from other parts of the function if the dependencies would allow it. Not sure if this 'feature' would be desirable for it would make reasoning about the code harder.

> How do we allow to opt-in for all these different capture behaviors?

> We do need a mechanism to handle conflicts when writing back captured data? Or is this managed by using the correct Data type wrapper (`Atom<T>`)?

> Add syntax/semantics for auto-disposing captured vars?

> Do captures also wait for nested async calls (join) until exiting their scope? - Yes.

Read-only capture are never a problem, the problem exists when using pointer-captures that are written to sometime during the execution of the function/lambda or code block.

We might not always want to make a hidden state object to store their 'captures'. New in dotnet is `static` lambdas...

Function-captures may want to use different mechanism to synchronize access to shared/global state. We could use data type wrappers like `Atom<T>` to indicate this strategy. By default a global variable state is read at first access by the function and compared with the actual value before a new value is written at the end of the function (optimistic locking).

Lambda-captures may outlive the function they're declared in. Not sure how pointer-captures would work in that scenario?

Block-captures work as functions would.

```csharp
x := 42
fn: |x|(p: U8): Bool
    return x = p

// no capture has to be specified in call
sameAsX = fn(42)   // true
```

---

> How does capture work inside loops?

```csharp
#use
    Fn
    Print

// Fn => (): Void
l = List<Fn>(10)
loop c in [0..10]
    l.Add(|c|() -> Print(c))
for fn in l
    fn();   // what does it print?
```

`|c|` is capturing by value, so it should print 0-9.

---

Capture state is not 'committed' when the capture scope is exited in an error condition (exception).

```csharp
x := 42
try |x|
    x = 101
    Error("Oh no!")

// x = 42
```

---

## Capture Aliases

Like all Aliases, using the assignment operator will rename the capture for (inside) the function/capture scope.

```csharp
x := 42
fn: |y=x|(p: U8): Bool
    return y = p    // true if p = 42
```

---

> TBD: How to 'add scopes' without indenting too much. Capture, error handling (with and use) etc. also require scopes.

```csharp
a := 42
use, with, |a|  // commas?
    ...
use; with; |a|  // line breaks?
    ...
```

---

> TBD: Use captures as a dependency list to run a function when one of the captured values changes?

```csharp
x :=^ 42        // mutable
fn: |x|(p: U8)
    ...

// some sort of registration?
// what will be the parameter values when the function re-triggers?
Reactive.OnCaptureChange(fn, p = 42)    // lifts the captures as dependencies
// what happens to any return value from fn?
// returned from Reactive.OnCaptureChange as an (async) enumerator?

x = 101     // trigger fn
x = 2112    // trigger fn

// how is the variable x hooked in order to produce a trigger?
```

---

> TBD: debug-only captures?

> TBD: have a capture syntax that forwards the dependency towards any subsequent calls inside the capture block. This would help in identifying pure functions?
