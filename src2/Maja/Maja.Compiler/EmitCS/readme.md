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

## TBD

The `CodedBuilder` currently emits C# code to a writer as well as build an object model representing namespace, classes, methods properties, fields etc.

The new idea is to use the C# LambdaExpression object model to represent all code to be generated.
When the complete model is built it can be serialized into C# code (text) or used by the compiler for compile-time code execution.

The problem comes when calling other methods/functions that are also to be compiled.
The Expression model uses `System.Reflection.MethodInfo` that describes the method to be called, 
but the method may not have been compiled yet and can therefor not be created using the reflection API.
We may have to create our own version of MethodInfo (its abstact so we can derive our own class) in order to describe methods that are not yet compiled.
This also goes for other reflection types (`System.Reflection.*`) that would need to be implemented, which add up to a lot of work.
