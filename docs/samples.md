# Samples

Fibonacci

```csharp
// recursive function
fib: (f: U16): U16
    //if f = 0 or f = 1
    if f in (0, 1)
        return f
    return fib(f - 1) + fib(f - 2)

// call 42'nd fibonacci number
f = fib(42)
```

---

Bubble Sort

```csharp
// bubble sort on an array
sort: <T>(arr: Mut<Array<T>>)
    l := arr.Length
    loop i in [0..l - 1]
        loop j in [0..l - i - 1]
            if arr[j] > arr[j + 1]
                // swap operator
                arr[j] <=> arr[j + 1]

// call sort on this array
unsorted :=^ [12, 42, 101, 45, 76, 82, 37]
sort(unsorted)
```

---

Reverse String

```csharp
reverse: (input: Str): Str
    output := ""
    // minus-sign loops in reverse
    loop i in -[0..input.Length]
        output += input[i]
    return output

s := "Hello World"
r := reverse(s)
// r = "dlroW olleH"
```
