# Defer

The `defer` keyword can be used in scenarios where execution of a code needs to happen at a later moment.

This pattern is useful for instance when cleaning up resources.

```csharp
useResourceFn: (): Bool!
    // allocate a resource
    r = try AcquireResource(42)
    // schedule closing the resource
    defer r.Close()

    // code here uses 'r'

    // <= end of scope executes defer statement
    return true
```

A defer statement is evaluated at runtime. This means runtime conditions can decide when or what to defer.

```csharp
useResourceFn: (): Bool
    // allocate a resource
    a = AcquireResource(42)
    r = match a
        Error => return false // exit function
        // schedule closing the resource
        _ => defer r.Close()

    // code here uses 'r'

    // <= end of scope executes defer statement
    return true
```

Multiple defer statements can be executed, which are stored on a stack. When the -usually file- scope ends, the deferred entries on the stack are popped off and run - resulting in a reversed order.

---

## Error Conditions

In more complex scenarios it is sometimes necessary to only cleanup if an error condition occurs. For this the `errdefer` keyword can be used.

```csharp
createResourceFn: (): Resource!
    // allocate a resource
    a = try AcquireResource(42)
    errdefer a.Close()

    // we only want to return a if b succeeds
    a.b = try AcquireResource(42)

    // <= if Err then executes errdefer statement
    return a
```

---

> TBD

Can we use `defer` for other deferred execution?
Linq does deferred execution and it is common in functional programming to defer actual execution of (composed) functions until a value is requested (a function that returns an 'infinite' set of values, but only providing one value at a time).

---

Can we tag functions that require `defer` to cleanup resource acquired by another (tagged) function?

```csharp
// how??
Open: (identity: Str): Resource
    ...
Close: (resource: Resource)
    ...

// mark as a defer candidate
Close: (resource: Resource) defer

d: Dispose<Resource> = Open("...")
defer d.Dispose()       // implicit
// calls Dispose(self: Resource) on exit
```
