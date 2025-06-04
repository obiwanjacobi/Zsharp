# Packages

Extra files can accompany the binary output, that provide extra meta data about the binary.

- debug symbols
- source code (zipped)
- symbol table
- modules, exports (pub) and imports (use)

## NuGet

We'll use NuGet as a package manager.

---

module locator? How are modules located? After the code is build for a module a module-info file is generated that advertises the module and allows it to be referenced during linking.

packager: bundle multiple modules and their support files into a package (probably a zip file). Packager is a function in the build library. A package has a version.

package manager? Retrieve 3rd party packages and register packages for distribution (typical).
