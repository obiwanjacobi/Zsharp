# External

The external assemblies are loaded in using the `AssemblyManager(Builder)`.
That forms a basis for resolving import (`use`) directives encountered in the code.

The import symbol will first be interpretted as a module (Type) name.
If that is not found, the import is retried as if it was a namespace.
If either method yields no results and error is generated.

## Modules

Modules are C# types - usually static classes for Maja modules.

```csharp
use namespace.module
use namespace.module.type
```

This will bring a single C# Type into scope.

## Namespaces

Namespaces contain multiple types (typically).
Also, the same namespace can occur in multiple assemblies.

```csharp
use namespace.namespace
```

This will scan all Assemblies for the namespace and bring all types contained within this namespace into scope.

## Extensions

### Operator Functions

All operators are represented by a specific function. The return type and parameter types determine when an operator-function is called in a specific situation. So the same operator can have multiple functions that implement it for different mix of types involved.
