# Range

> Note: Range's stop or end term is exclusive!

```C#
[1..4]      // closed range
[..4]       // from start
[4..]       // till end
[..]        // all
[0]         // first
[-1]        // last
```

```C#
rng = Rng(1, 6)     // ??
```

## Step

A third optional parameter for a range is the step the value takes on each iteration.

```csharp
// forward
[0..5]
[0..5, 1]   // same
[5..0, 1]   // does not iterate.

// backward
[5..0, -1]
```

If no step is specified it is always 1. This means that ranges with start > end, will not iterate - a behavior that is most useful/common/expected I think.

## Static

```C#
[1..10]
```

## Dynamic

```C#
i = 42
[0..i]
```

Iterators

- Array
- List
- Range
- Slice

Usually not a type you would create directly.

```csharp
Iter<T>
    Next: (self): Bool _
    Current: (self): T _
```

Usually not function you would call directly.

```csharp
GetIter: <T>(self: Array<T>): Iter<T>
GetIter: <T>(self: List<T>): Iter<T>
GetIter: <T>(self: Range<T>): Iter<T>
GetIter: <T>(self: Slice<T>): Iter<T>
```

```csharp
ArrIter<T>
    arr: Array<T>
    i: U8

Next: <T>(self: ArrIter<T>)
    if self.arr#Count > self.i
        i = i + 1
        return true
    return false

Current: <T>(self: ArrIter<T>)
    return self.arr[self.i]
```

## Slices

> A pointer and a length
