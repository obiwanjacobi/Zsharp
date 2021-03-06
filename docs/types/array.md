# Array

The array is a continuous sequence of elements. The structure of these elements are expressed in types. An array is always one-dimensional. It it possible to make arrays of arrays, thus simulating multi-dimensional arrays.

An array has a fixed size, determined at compile time that cannot be changed: the array cannot be made larger or smaller. All its data is stored on the stack (or in the global segment).

An immutable array is initialized once and its contents cannot be changed after that:

```C#
// immutable
arr = [1, 2, 3, 4, 5]#imm               // 5 x U8
arr: Imm<Array<U16>> = [1, 2, 3, 4, 5]  // 5 x U16

arr[0] = 42                     // error!
arr: Array<U16> = [1, 2, 3, 4, 5]#imm  // error! arr is not Imm<T>
```

A mutable array has its size specified up front, but the contents of its elements can be changed dynamically in code.

```C#
// mutable
arr = [1, 2, 3, 4, 5]               // 5 x U8
arr: Array<U16> = [1, 2, 3, 4, 5]   // 5 x U16
// using a creator function
arr = Array<U8>([1, 2, 3, 4])   // 4 x U8
arr = Array<U8>(10)             // 10 x U8 (all 0)

arr[0] = 42                     // first element now has value 42
```

> What is the syntax to specify a fixed array in a structure?

```csharp
MyStruct:
    arr: Array<U8>(10)
    arr: U8[10]
    arr: U8(10)
```

There are two ways to select array elements:

- By Index
- By Range

Using an index (zero-based, positive integer) pinpoints a single element. The type of this operation is the element type of the array. It can be used to read or write a single value (T).

Multiple array elements are selected when using [Ranges](range.md). These too can be used to read a value from the array as well as write a value to to the array - if it is mutable.

The resulting type of a Range operation is a `Slice`. A slice is a view into the original array bounded by the range that created it. It is continuous and sequential, just like an array. Think of it as a subset.

A slice does not take up storage (other than a pointer and a length) and therefor is not a real concrete type. It is a virtual and dynamic type that is implemented by the compiler. A slice takes on the type of the array it originated from.

Here's an example writing values to a mutable array:

```C#
arr = Array<U8>(10)         // 10 x U8
arr[3] = 42                 // set 4th element
arr[0..3] = 42              // slice: 1st, 2nd and 3rd
arr[..] = 0                 // slice: all set to zero
```

Reading values from an array is similar:

```C#
arr = [1, 2, 3, 4, 5]       // 5 x U8
x = arr[3]                  // x = 4
l = arr[-1]                 // l = 5
s = arr[0..3]               // s = [1, 2, 3]
arr2 = arr[..]              // arr2 = all elements (slice, not a copy!)
```

> Do we repeat (or truncate) range assignments of unequal length? `arr1[0..4] = arr2[0..2]`

---

> Array8, Array16 could be used to indicate the number of bits used for the max count.

```csharp
Array<T> : T[]     // variable length at compile time
```

---

## Array Pointers

To avoid copying over the complete array for a function call, it has to be passed by reference (pointer).

What will the syntax be when accessing (dereferencing) an array pointer?

```csharp
arrayFn: (Ptr<Array<U8>> arr)
    first = arr()[0]

arr = [12, 23, 34, 45, 56, 67]
arrayFn(arr.Ptr())
```

---

> TBD

Difference between `readonly` and `immutable`:

- ReadOnly: An Immutable object that does not have any way to change it.
- Immutable: A fixed/frozen object that performs changes by creating new representations, leaving the original immutable object intact.

```csharp
arr = [1, 2, 3, 4, 5]   // readonly
arr[0] = 42             // error: array is readonly

arr: Imm<Array<U8>> = [1, 2, 3, 4, 5]   // immutable
arr2 = arr.SetAt(i, 42) // returns a new array with changed value at index 'i'
```

---

> TBD

Should creating data use the same operator/syntax as indexing?
Introduce a syntax difference between indexing/ranges `[]` and making data `()`?

You could argue that indexing is a function that looks up the value at a specific position.

Would `[]` be an operator - with a backing function? Would it be overloadable. Would it apply to other Types?

What would the syntax look like if there were no special operators to work with an array?

```csharp
arr = { 1, 2, 3 }   // using object syntax is conflicting
arr = ( 1, 2, 3 )   // list construction syntax?

i = 1           // index
x = arr.At(i)   // lookup value (U8)
p = arr.PtrTo(i)  // lookup pointer (Ptr<U8>)
s = arr.PtrTo(i, 2) // sub-array (Slice<U8, 2>)

arr: Array<U8> = ( 1, 2, 3 )     // mutable
arr.At(i) = 42  // At() used as getter and setter?

// or separate?
x = arr.GetAt(i)
arr.SetAt(i, 42)
```

> What type would the `At()` function return in order to read and write from it? (A reference as a transparent pointer?)

Are Slices the single way to return references into an array (or list)? Overhead for single values, meant for sub-ranges...

What operators does the Slice have overloaded?

---

> TBD

Multi Dimensional Arrays

Tensor => arbitrary number of dimensions...

```csharp
// 2x3
arr2D = (1, 2, 3), (4, 5, 6)
// 2x 3x3
arr3D = ((1, 2, 3), (4, 5, 6), (7, 8, 9)),
        ((11, 12, 13), (14, 15, 16), (17, 18, 19))
// 2x 2x 2x2
arr4D = (((1, 2), (3, 4)), ((5, 6), (7, 8))),
        (((11, 12), (13, 14)), ((15, 16), (17, 18)))
```

```csharp
arr2D = Array<U8>(4, 3)
arr2D.At(0, 0) = 42
arr2D.At(3, 2) = 42     // last position

arr3D = Array<U8>(4, 3, 2)

```

---

> TBD

Reuse F# libraries to implement immutable array/list/dictionary sharing algortithms?
