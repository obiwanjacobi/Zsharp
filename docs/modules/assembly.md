# Assembly

For .NET compatibility we need a way to indicate what modules will be compiled into one assembly.

The assembly file is a type of language supported project file that defines what modules to include into an assembly and what name that assembly has that is being compiled.

The `assembly` keyword works similar to how the `module` keyword works, it names the assembly. It must be the first instruction in the file - only comments or whitespace can precede it.

An `assembly` file cannot contain any runtime code other than (assembly-level) code attributes or use of compiler directives and compile time code. Everything else is a compiler error. It is possible to code the build logic in the assembly file.

> TBD: `.NET` now supports a module-initializer that runs once at assembly level.
We could introduce an `#initialize` (similar to `#execute`) to allow to specify one.

```csharp
// comments in front are allowed
# assembly Name.OfMy.Assembly
```

There can be multiple assembly files in one compilation run. This will result in generating multiple assembly files.

---

## Include

Here an example of an assembly that includes module1 and module2 that are compiled in this project (not external):

```csharp
# assembly Name.OfMy.Assembly

// included in the compiled assembly
# include
    implModule1
    implModule2
```

The `include` keyword includes the compiled source code module into the specified assembly.

> TBD: include by file names?

```csharp
# assembly Name.OfMy.Assembly

// included in the compiled assembly
# include
    file1.zs
    file2.zs
```

> TBD: Include all by default and Exclude files specifically?

> TBD: if local modules are included by file name, they could be specified using the `import` keyword and we would not need the `include` keyword. Then again, the `include` keyword could be a general way to include a file's content into another file. However that is not how it is used here. `include` in the assembly file could (be made to) mean to include a library as source code (point to -remote- root folder).

> Or perhaps introduce a `load` that distinguishes between different `include` flavors?

```csharp
# assembly Name.OfMy.Assembly

// included in the compiled assembly
# include
    file1.zs
    folder/src  // include a folder (recursive)
    'http://project.github.com/master/src'    // remote source
```

---

## Global Import

Modules globally imported in an assembly file are available throughout the project code files without explicit reference.

```csharp
# assembly Name.OfMy.Assembly

// global imports
# import
    externalModule1
    internalModuleToo

# include
    ...
```

> Also allow `export` modules to have assembly-public modules that can be used by other projects? That would mean that to determine at what accessability to generate the code, we need the full hierarchy of assembly-file and module file.

|Assembly|Module|Module Access
|--|--|--
| - | - | Private to the module.
| - | export | Internal to the assembly.
| export | - | Module (type) is assembly-public, but empty. (*)
| export | export | Public to the assembly.

*) Should probably give a warning.

> TBD: I don't like that the actual accessability is spread over two files (module and assembly)

---

## Project Attributes

Project dependencies can be listed in the assembly file as well.

- .NET platform / frameworks
- External Assembly References
- NuGet Packages
- Project References (TBD)

```csharp
// TBD: project reference?
#ref MyProject/assembly.zs

// assembly reference (pass probe paths to compiler)
#ref System.Core.dll    // or .exe

// nuget package (with version, no file extension)
#ref System.Console@>=5.0.*

// .NET framework / SDK reference
#sdk Net5.0@CoreAppSDK
```

Other project attributes may include any property and its value. Some sort of generic way to specify?

```csharp
#prop=value
```

Same with project items?

A lot of the content of a visual studio project file is related to IDE specific things. We don't need those - just the ones that relate to compilation.

---

## Assembly Attributes

Standard .NET Assembly code Attributes can be applied to the assembly.

The `Attribute` postfix in the class name can be omitted just as in C#.

```csharp
# assembly Name.OfMy.Assembly

// decorator syntax
{AssemblyFileVersion("1.0.0.0")}
{AssemblyProduct("ProductName")}

// do we have project settings available as pragmas?
{AssemblyConfiguration(#project.configuration)}
```

Strong-naming an Assembly can be done with assembly attributes.

---

## Compile-time Code

An Assembly file can also contain compile time code and other `#` directives.

```csharp
# assembly Name.OfMy.Assembly

// a compile-time function
#! calculateVersion(): Str
    ...

// calling the function to yield the version string
{AssemblyVersion(#calculateVersion()}
```

---

## Assembly Entry Point

If the assembly is an executable the entry point of the program is listed in the assembly file.

```csharp
# assembly Name.OfMy.Assembly
# execute MyEntryPointFn
```

If `#execute` is not specified the assembly is a (dynamic link) library.

The function specified must be present in one of the included modules.

The declaration of the entry point function must be one of:

```csharp
main: ()
main: (): I32
main: (args: IEnumerable<CommandLineArgument>)
main: (args: IEnumerable<CommandLineArgument>): I32
```

> Research if we can use the new System.CommandLine library that allows any number of typed parameters on the 'main' function that get automatically parsed from the command line.

---

> TBD

Clearly this is nowhere near the information for a full project management and build system. Do we try to force everything we need into an assembly file, or do we introduce another (project) file?

- a Z# version specifier with what version of the language the assembly is compatible.
- various compiler switches (compiler profile).

---

What about embedded resources? How to tell the project to embed a resource.

```csharp
# assembly Name.OfMy.Assembly

// files included in the compiled assembly as resources
# include
    image1.jpg
    assembly.dll
```

No control over naming (namespace and resource name)!
Have a compile-time function for this? `#embedFile("image1.jpg", "namespace.resourceName")` or have an `#embed` pragma?

Doing this in a module file could indicate a different scope/nesting and perhaps use the module's name as a namespace.

How about string/int resources?

```csharp
// $ auto resource string
resourceText = $"Text loaded from resource"

// performs a load resource (with local var)
print(resourceText)
```

How to reuse an existing resource then?
What is the scope of the name? Module level? no - too narrow.

> Can we built-in localization on resources?
