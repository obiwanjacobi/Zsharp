# Range

A Range encapsulates a range of indices and optionally a step value or a sequence of numbers.

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

// TBD
[3..=9]      // inclusive end?
```

> Note negative values in ranges used for indexing arrays and lists, mean 'count backward from the end of the array'. Negative numbers in range expressions to generate number sequences mean literal negative numbers.

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

> TBD

A range results in a (virtual) list with numbers. So shouldn't the syntax reflex a list?

```csharp
rng = (0..10)       // range from 0 to 9 -incl.
stp = (0..10: 2)    // 0, 2, 4, 6, 8
stp = (0..10+ 2)    // alternate syntax? (bc ':' is for type)
```

> TBD: use list syntax `()` when creating range objects. Use index syntax `[]` when extracting from Array or List objects.

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

> `.NET`: how does C# behave concerning non-normalized ranges and iteration? => Throws an `ArgumentOutOfRangeException` on `Range` ctor.

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

> Use ranges for value range checking (range-condition).

```csharp
x = 42
// 'in' is interpreted differently than in a loop!
if x in [0..100]
    // true
```

That would also mean this: `Range<T>` to allow for floats etc.
Step would only be optional for integers but mandatory for floating point numbers.

```csharp
flt = [0.0..1.0: 0.2]  // 0.0, 0.2, 0.4, 0.6, 0.8
```

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
    ptr: Ptr<T>     // ptr to first element
    length: U32

Slice<T>
    ptr: Ptr<T>     // pointer to object
    offset: U32     // offset to first element
    length: U32
```

---

Examples

```csharp
arr = (1, 2, 3, 4, 5)

loop v in arr[1..-2]    // 2, 3
    ...
```

---

> TBD

Add a negative sign to do a reverse range?

```csharp
loop n in [8..0]    // reverse range
loop n in -[0..8]   // neg operator on fwd range
    for_n_7_6_5_4_3_2_1_0
```

---

> TBD

Have syntax for standard 'sub-array' behavior of `.NET` `Range` usage?

I think .NET copies over the array elements when indexing with a range.

```csharp
arr = (0, 1, 2, 3, 4, 5, 6, 7, 8)
sub = arr.[0..5]        // dot operator
sub = arr.copy([0..5])  // explicit function
```

---

> TBD

Use a specific range syntax to indicate if the indices are inclusive or exclusive.

```csharp
incl = [1..6]       // 1 and 6 are both part of the range
excl = (1..6)       // 1 and 6 are not part of the range
half = [1..6)       // 1 is incl, 6 is excl
other = (1..6]      // 1 is excl, 6 is incl
```

https://www.cuemath.com/algebra/interval-notation
