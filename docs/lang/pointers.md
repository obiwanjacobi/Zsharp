# Pointers

## Pointer Types

The template type `Ptr<T>` is used to represent a pointer.

> The number of bits used for a pointer into memory depends on the memory model: 16, 20, 24 ...?
When bank switching and extended memory is worked out this number is configured for the compiler to use.

```C#
ptr: Ptr<U8>        // pointer to an U8
```

Create a pointer:

```C#
v = 42;
ptr = v.Ptr()       // explicit call to make ptr
```

Dereferencing a pointer is done by using de function `()`'s.

```C#
ptr: Ptr<U8>
v = ptr()           // v: U8
```

Assigning a new value to the pointed-to-storage:

```C#
changeByRef: (Ptr<U8> ptr)
    ptr() = 42          // write new value

    v = ptr()           // read into a local copy
    v = 42              // does NOT change ptr value!
```

### Optional

```C#
ptr: Ptr<U8>?       // an optional pointer to U8
ptr: Ptr<U8?>       // pointer to an optional U8
```

Pointer variables need to be initialized when declared or they must be made optional.

```csharp
p: Ptr<U8>      // error! must have value or be optional
p: Ptr<U8>?     // ok, no value - so optional

a = 42;
p: Ptr<U8> = a.Ptr()    // ok, ptr has value
```

> How does writing to an optional ptr work?

```csharp
a: U8?                      // now _
p: Ptr<U8?> = a.Ptr()
p() = 42
```

### Pointer to Pointer

Should be no different than creating that initial pointer.

```C#
pp: Ptr<Ptr<U8>>        // ptr to ptr to U8
opp: Ptr<Ptr<U8>>?      // optional ptr to a ptr to U8
pop: Ptr<Ptr<U8>?>      // ptr to optional ptr to U8

p: Ptr<U8>              // Ptr<U8>
pp = p.Ptr()            // Ptr<Ptr<u8>>
```

> Only two levels allowed? => yes

### Pointer to Arrays

Works the same as any other ptr.

```csharp
arr = [1, 2, 3, 4]
p = arr.Ptr()       // Ptr<Array<U8>>
```

The pointer _into_ an Array is not expressed with the `Ptr<T>` type. Instead the `Slice<T>` type is used for this.

> TODO: `Slice<T>`

### Pointer to Immutable

```C#
// literals are immutable by default
x: Imm<U8> = 42
p = x.Ptr()     // Ptr<Imm<U8>>
p() = 101       // error! immutable
```

### Pointer Arithmetic

Typically a `Slice<T>` should be used to index into a pointer. See Pointer to Arrays.

For typed pointers the pointer value can only be manipulated with a value from its type. This is to accommodate variable length elements in an 'array'.

```csharp
Struct
    length: U8
p: Ptr<Struct>
p = p + p.length    // ok, used struct field as value
p = p + Struct#size // ok
p = p + 42          // error, can't add random value to ptr.
```

For untyped pointers (opaque type references) does not have this restriction:

```csharp
p: Ptr                  // untyped
p = p + 42              // ok
p = p + Struct#size     // ok
```

Untyped Ptr's cannot be cast to a type / struct after they have been moved from their original value.

> So what use is there?

---

> TBD: Syntax for pointing to members of structures?

Needs an offset (compile time) from a runtime Ptr.

```C#
MyStruct
    field1: U8
    field2: U16

s = MyStruct
    ...
p = s.Ptr()
pFld2 = p#offset(MyStruct.field2)
```

> What about pointing to bit-field members?

---

### Casting

Type compatibility.

> Do we allow type conversion? Deref a `Ptr<U8>` into an `U16`?

```C#
MyStruct : OtherStruct
    ...

ptr = Ptr<MyStruct>
cast: OtherStruct = ptr     // ok, cast to derived type
p = cast                    // ok, is original type
```

```C#
MyStruct : OtherStruct
    ...
MyStruct2: OtherStruct
    ...
ptr = Ptr<MyStruct>
cast = ptr.value<OtherStruct>()
p2 = cast.value<MyStruct2>()        // error, is not original type
```

If the original type is lost or cannot be determined at compile time, casting up the inheritance hierarchy will always fail.

## Pointer to a Function

A pointer to a function can be taken by assigning the function name to a variable or parameter.

> This deviates from normal pointer behavior. To be more consistent perhaps a different syntax is needed: `myFunction.Ptr()` would be more in line with how other pointers work.

The type of a function pointer is the Function Type wrapped in a `Ptr<T>`.

Here is an example of how to construct a pointer to a function.

```C#
MyFunction: (magic: U8) _   // function interface
myFnImpl: MyFunction        // implementation
    ...

p = myFnImpl            // p: Ptr<MyFunction>
p = myFnImpl.Ptr()      // explicit
takePtr(p, 42)          // call function with ptr to function

takePtr: (ptr: Ptr<MyFunction>, p: U8)
   ptr(p)            // call the MyFunction through ptr
                     // passing in its 'magic' param
```

To take a pointer from a function, it must specify its (function) Type up front. This function type definition contains the signature of the number of parameters and their types as well as the return type - if any.

When the code has a pointer to a function, it can be called by specifying the `()` straight after it. Any parameters the function that is pointed to requires, must be passed in at that time. The return value -if any- will be available when the function returns.

A function without implementation is called a function declaration or [Function Interface](interfaces.md).

### Function Pointers in Structures

Use function pointers in a structure fields as a way to simulate an (OO) object (by hand).

Normal (data) fields can still be added - but are publicly accessible.

```csharp
// function interfaces
OpenFn: (path: Str): Ptr _
CloseFn: (file: Ptr) _

// structure with function pointers
File
    open: Ptr<OpenFn>
    close: Ptr<CloseFn>

// function interface implementations
MyOpen: OpenFn
    ...
MyClose: MyClose
    ...

// init the struct (instance)
f: File
    open = MyOpen
    close = MyClose

// call functions through pointers
p = f.open("path/to/file.txt")
f.close(p)
```

Could be used to make virtual functions (by hand) and implement polymorphism at the object level.

## Untyped Pointer

Opaque Type Reference.

A way to export a handle to an instance of a private type.

Purpose is to not expose internal structures that are allocated on behalf of clients/callers.

Use a type-less `Ptr`.

```csharp
export outFn: (p: U8): Ptr
    s: MyStruct
        ...

    return s.Ptr()  // cast/convert

export inFn: (p: Ptr): U8
    s: MyStruct = p()
    x: OtherStruct = p()  // error! not the correct type
    ...

// usage
import outFn    // pseudo
import inFn     // pseudo

o = outFn(42)   // o: Ptr
o.x             // error! Ptr does not allow member access

a = inFn(o)     // Ptr as parameter
```

It is also possible to make a 'typed' typeless pointer:

```csharp
Handle: Ptr _
createFn: (p: U8): Handle
    ...
openFn: (h: Handle): Stream
    ...
```

`Handle` is still a typeless pointer but made specific to a set of functions. This way you can direct the user of your API to not use any typeless pointer, but the specific one you designed. It makes you API clearer.

### Static Ptr Helper

```C#
a = 42
ptr = Ptr.to(a)

ptr = Ptr.to(42)    // ptr to literal is immutable
```

> How would a function know its a literal value?

---

> TBD

Reference Counted Pointer

Weak Pointer

Garbage Collected Pointer