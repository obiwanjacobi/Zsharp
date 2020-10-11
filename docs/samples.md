# Samples

Fibonacci

```csharp
// recursive function
fib: (f: U16): U16
    if f = 0 or f = 1
        return f
    return fib(f - 1) + fib(f - 2)

// call 42'nd fibonacci number
fib(42)
```

---

Bubble Sort

```csharp
// bubble sort on pointer to array
sort: <T>(arr: Ptr<Array<T>>)
    // Array length is known at compile-time
    l = arr()#length
    loop i in [0..l - 1]
        loop j in [0..l - i - 1]
            if arr()[j] > arr()[j + 1]
                // swap using deconstruct / tuple
                (arr()[j + 1], arr()[j]) =
                        { arr()[j], arr()[j + 1] }

// call sort on this array
unsorted = [12, 42, 101, 45, 76, 82, 37]
// pass in a pointer to avoid copying
sort(unsorted.Ptr())
```

---
