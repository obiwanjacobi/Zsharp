# IR Processor

The idea is to have the source code split into parts which are processed asynchronously (multi-threaded). Then to recombine those parts into a complete IrModel. Two phases may exist:

- Initial compilation that reports errors to the user
- Optimized (lowered) IrModel that is used to emit output.

The processor takes `SyntaxTree`('s) => `IrModule`s.

A `WorkItem` object carries operation details:
- Id?
- IrProcessXxxx object
    - Scope
    - Dependency List

The dependency list keeps track of all the outstanding symbols that need to be resolved for this object.

On the providing-side the `SyntaxTree` is walked and declarations and statements are queued up to be processed.

`IrProcessType` is a mutable object that keeps track of processing a Type declaration
`IrProcessFunction` is a mutable object that keeps track of processing a Function declaration
`IrProcessCodeBlock` is a mutable object that keeps track of resolving dependencies etc.

These objects all reference the relevant Syntax object.

> How to know what `IrCodeBlock` belongs to what Function (or statement within another CodeBlock)?
> Also Type and Function declarations can be nested.

Do we need an `IrProcessReference` object to link two `IrProcessXxxx` objects together in a specific way?



## Processors

On the receiving side there are registered processors that each perform a specific task. When a processor is finished it writes it's state to the `WorkItem` object.

> Is there Processor dependency or order?

- Register delcared symbols (in scope)
- Resolve dependent symbols (from scope)
    Note that this processing is depth-first in order to be able find 'more-local' symbols of the same name as 'more-global' symbols. (even though the language discourages that, it still needs to work)
- Resolve operator functions (lookup operator impls.)
- 

## Meta-Programming

The meta-functions in the program (functions that run at compile-time) are able to create new functions and types (not variables!) that need to be injected into the compilation process.
The 'language' how these additions are communicated has not been decided yet.
Probably requires some specialized Processors.

## Error Detection

During the processing warnings and errors can be collected in a `DiagnosticsList`.
Some additional error conditions need to be handled:

- Deadlock (circular references) multiple items (at least two) are waiting on eachother to finish.
- Undeclared types/functions: single items cannot be processed because the types/functions they depend on were never registered.

> How do we know when processing is finished? => When no processors have changed any of the items.

## Item Processing

- Walk the Syntax tree and create basic IrProcessXxx items for declarations and statements. Maintain references between items for dependencies (hierarchy).
- Proc: create the initial Symbol structures for Types, Functions and Expressions. These contain UnresolvedSymbol instances of unresolved references.
- Proc: Register declarations where all dependencies/unresolved Symbols are resolved.
    - What if no declarations can be resolved?
    - How do you know when it is time to resolve dependent symbols?
      - How to resolve a symbol with the same name in different scopes?
- Proc: When an item has its symbols resolved, its IrModel objects can be created and stored in the item.
- Proc: Resolve operator functions and rewrite the IrModel to reflect calling those functions.
- Proc: [other operations]
- Collect Diagnostics that each Processor might generate. (post diagnostics in a separate channel that is read by the 'client')
- Put items where all Processors are done processing the item, in the resolved-list.
- Each item that is processed is check if it has any dependencies that are currently in the resolved-list. Assume that each item only has one parent/is a dependency for one other (parent) item.
- Detect when no processor is handling an item. That item is then 'undefined'.
- When no items are in the queue or all processors are inacive (not handling items) the operation is finished.

