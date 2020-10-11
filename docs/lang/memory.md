# Memory

The language does not know anything about memory management. It is oblivious to the existence of a heap. It does assume 'stack space' or 'global space' when allocating room for storing variables.

That means that when program data is to be stored outside the scope of a function (stack) an explicit library function must be used in order to make that happen.

## Memory Allocations

> Allocating memory is always explicit. There are NO hidden memory allocations!

When a function requires memory allocation it explicitly take in a parameter to an Allocator object.

The object stores pointers to -potential custom- functions that are used to make the actual allocations. The standard library will provide a host of convenience functions based on these core functions.

```C#
Allocator
    Alloc: Ptr<AllocMemory>
    Realloc: Ptr<ReallocMemory>
    Free: Ptr<FreeMemory>

// type bound convenience functions
Alloc(self allocObj: Allocator, size:U16): Ptr<U8>!
Realloc(self allocObj: Allocator, mem: Ptr<U8>, size: U16): Ptr<U8>!
Free(self allocObj: Allocator, mem: Ptr<U8>)
```

> Probably other core function are required - did not feel like fleshing that out at this time (something like grow/shrink?).

Typically a custom Allocator object is created through a factory function for the specific strategy. The factory function is free to return any custom structure that is derived from Allocator for its local/own bookkeeping.

```C#
alloc = getHeapAllocator()      // get allocator object

a = 42
s = a.Text(alloc)                // function that needs memory
```

### Allocation Strategies

- Heap: traditional (global) memory allocation where a reserved chunk of memory is used to satisfy memory allocation requests.
- Pool: A pool (of a fixed number) of fixed sized chunks. Usually the size of a specific type.
- Scoped: A container for allocation made within a certain scope. When going out-of-scope, memory is freed/container is destroyed.
- Object: allocated objects are on a stack. When an object is freed, all other object on top of it are freed also.
- Banks: Some sort of strategy concerning bank switching.
- Stack: implicit by using language constructs (params/local vars).
- Debug: fill memory with patterns and track calls. How to catch double-frees and use-after-free?
- OutOfMemory: simulates out-of-memory after a number of allocations or reached size.

> TBD: Is it possible to write heap management code with the current language definition? Are there features we need that are now impossible? How do we claim a block of RAM?

---

> Differentiate between code and data addresses? A pointer to a function is a code address. A pointer to an allocated array is a data address. You cannot write to code addresses. You cannot execute data addresses (possible issue when rom-images are copied to ram for fast execution in old hardware - supply transformation/casting).

---

> Construct individual structs into a pre-allocated memory space. For instance to layout an array or variable-length types.

## Volatile ??

> How to deal with memory mapped hardware (IO)? Read, Modify, Write bits.

See also [Assignment Expression](../expressions/assignment.md) (need to organize that better).

## Code Layout

Assign code to memory sections (Page0) to fit hardware targets.
Hardware description language? Linker Scripts?

## Memory Banks

Every system has its own way of extending memory. Wide range of mechanisms and different rules.
Need to provide an interface that can be implemented (inline assembly) to facility integration into the language.

### Bank Windows

A range in memory that is swapped/replaced by switching banks.

Multiple windows may exist.

Bank size == Bank Window size

### Data in Memory Banks

The active bank window is locked until operation is done with data.

Data must be self-contained.

### Code in Memory Banks

Code blocks can be switched in and out recursively if necessary.

When function call is done, previous bank needs to be restored.
