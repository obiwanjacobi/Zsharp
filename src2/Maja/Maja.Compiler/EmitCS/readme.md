# Emit CSharp

The emit stage results in C# code.

## From Intermediate Representation To Emit C#

After the initial Ir tree has been generated, 
some constructs are lowered to represent a logical C# structure.

There may be Ir structures that need to be linked to the Maja runtime library.

| Ir Source | Ir Target for C# | 
|--|--|
| ?? | Nothing yet...

After the Ir model has been lowered it is passed to the `CodeBuilder` that will walk the Ir tree and generate code for each node.
During this walk a C# code representation is maintained in a stack.

This code representation consists mainly of C# structure components like namespace, class, method, field, method, property etc.
As the Ir tree is walked these C# structure elements are pushed and popped of a stack. 

The C# structure objects are only there for reference and to support decisions further down that may need information on what the namespace name is, what type of class is being generated or the access modifier of the module class etc.

| Ir Source | C# code | 
|--|--|
| Constructor function | object T constructor
| Conversion function | cast
|  |

Each `IrModule` results in one C# file that is saved to disk and added to a `CSharpProject`. When the emit stage is done (there should not be any errors at this point) the C# project is built and the resulting assembly becomes available.

