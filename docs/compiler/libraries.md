# Libraries

A library (meta) format for including reusable code in program.
Source code only inclusion. Everything is compiled (and optimized) at the same time.

> Name-mangling: Need to uniquely identify public (exported) names to be used by other code. Functions (parameter types, return type), Types, Template parameter and all identifying data must be incorporated into the the mangled name. It must be possible to add an overload to an existing function and have a unique identifier for it. Also name-mangling should be turned of locally for non-exported functions and types and for special (manual) cases where the name must be as-is.

## Dependencies

Need to publish required dependencies of the library itself. Libraries can depend on libraries.

## Versioning

Libraries have a specific version. Use semantic versioning (semver) and a naming mechanism to find correct files.

---

Standard Library

- Stream IO (console)
- File IO?

Build Library

Units Library
