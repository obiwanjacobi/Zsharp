# Range

A Range encapsulates a range of indices and optionally a step value.

Given an array of integers:

Array|1|2|3|4|5|6|7|8
--|--|--|--|--|--|--|--|--
Index|0|1|2|3|4|5|6|7
Reverse|-8|-7|-6|-5|-4|-3|-2|-1

## Syntax

> Note: Range's stop or end term is exclusive!

```csharp
[1..4]      // closed range (skip first)
[..4]       // from start
[4..]       // till end
[..]        // all
[0]         // first
[-1]        // last
[..-1]      // till one before end
```

```csharp
rng = Range(1, 6)     // allow??
```

```csharp
arr = (1, 2, 3, 4, 5, 6)
// index-based array composition?
x = arr[0, 3, 1, 4]
// x: Array<U8> = (1, 4, 2, 5)
// x cannot be a Slice
```

### Step

A third optional parameter for a range is the step the value takes on each iteration.

```csharp
// forward
[0..5]
[0..5: 1]   // same
[5..0: 1]   // does not iterate.

// backward
[5..0: -1]
```

If no step is specified it is always 1. This means that non-normalized ranges with start > end, will not iterate - a behavior that is most useful/common/expected I think.

### Dimensions

> TBD

Multi-dimensional ranges.

```csharp
// 2D range
[..5, 3..5]
// 3D range
[..5, 3..5, -1]

```

### Static

```C#
[1..10]
```

### Dynamic

```C#
i = 42
[0..i]
```

## Range Type

Keeps track of the indices that define a range.

```csharp
Range
    begin: U32?
    end: U32?
    step: U32 = 1
```

Ranges convert to Slices when paired with an array or list.

```csharp
a: Array<U8> = (1, 2, 3, 4, 5)
r = [0..]       // Range object
s = Slice(a, r) // Slice<T> object
i = GetIter(s)  // Iter<T> object
```

> TBD Use ranges for value range checking?

```csharp
x = 42
// 'in' is interpreted differently than in a loop!
if x in [0..100]
    // true
```

That would also mean this: `Range<T>` to allow for floats etc.
Step would only be needed in a loop/iter scenario.

## Iterators

- Array
- List
- Range
- Slice

```csharp
Iter<S, T>      // interface
    Next: (self: S): Bool _
    Current: (self: S): T _
```

```csharp
GetIter: <T>(self: Array<T>, rng: Range?): Iter<T>
GetIter: <T>(self: List<T>, rng: Range?): Iter<T>
GetIter: <T>(self: Slice<T>): Iter<T>
```

```csharp
ArrIter<T>
    arr: Ptr<Array<T>>
    i: U8

Next: <T>(self: ArrIter<T>)
    if self.arr()#Count > self.i
        i = i + 1
        return true
    return false

Current: <T>(self: ArrIter<T>)
    return self.arr()[self.i]
```

## Slices

> A pointer and a length

```csharp
Slice<T>
    ptr: Ptr<T>
    length: U32

Slice<T>
    ptr: Ptr<T>
    offset: U32
    length: U32
```

---

Examples

```csharp
arr = (1, 2, 3, 4, 5)

loop v in arr[1..-2]    // 2, 3
    ...
```
