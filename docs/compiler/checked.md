# Checked Functions

Use functions that do extra checking like bounds and overflow checking using a compiler option. This make the code safer but also bigger and slower. Typically on during development and off for release.

User should also be able to write a checked and unchecked version of a function.

## Code Contracts

Provide a way to be explicit about the expectations of the code (functions).

> Detailed diagnostics information is available when a check fails.

### At Compile Time

The compiler (or standard library) provides an `#assert` (not sure about the name yet) function that checks the provided condition and generates an error (a warning too?) when it evaluates to false. Additional text can be specified to explain the problem.

This can be used as a basis to allow compile time checking of expectations of any code. Helper functions could be created with more appropriate names like, expect or guard etc.

### At Run Time

This requires the code that does the validation to be present in the output binary. The `assert` function can be used to signal the runtime problem. At runtime `assert` failures result in a `FatalError`.

Again helper functions can be made to make more appropriate names and -more importantly- have a way to eliminate the code when at compile time the necessary flags are not found.

## Operator Overloads

Operator overloads are checked by a built-in test framework to ensure they follow the laws for that specific operator. This ensures that an (overloaded) operator always behaves in the same predictable manner.

> TDB: determine laws for each operator.

- Identity law (operator should not change identity of operands)
- Associative law (precedence)
- Commutative law
- Distributive law

## Compile Time Conditions

A fairly fine grained control over what functions are checked and what function are not check may be desireable. This should be specified on the compiler command line as well as in code.

For some code it may be necessary to always check boundaries for example... (cannot imagine this happening)

### Removing Code

At some point the compiler will encounter a function call where it's function is not flagged to be compiled. In that case, the function call will be removed from the code as well as all code that now becomes invalid.

In the next example `conditionalFn` has been removed and all dependent code will therefor also be removed.

```C#
t := conditionalFn(true, "example")
if t = 42
    code_that_uses_t
other_code              // only this code will remain
```

Specifying the check flags in code:

```C#
arr := [1, 2, 3, 4, 5]

#enable(Checks.Bounds)
    arr[42]         // will always be checked
```

Check Flags:

- Bounds
- Overflow
- Underflow
- Divide by Zero    (always FatalError but now extra diags)

> TBD: Coupling a flag to a function?

```C#
[[Conditional(Checks.Bounds)]]
checkFn: <T>(arr: Array<T>, index: U8)
    ...
```

---

> A generic mechanism to pre (post?) check function calls at compile time? (at runtime too?)

```csharp
MyFunction: (p: U8): Bool
    ...

#check MyFunction: CheckMyFunction

// is passed the arguments of MyFunctions, can raise an Error if validation fails. May be included conditionally (Debug build).
#! CheckMyFunction: (p:U8): Error?
    ...
```

---

> TBD

A way to suppress compiler warnings.
