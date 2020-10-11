# Zlang Packages

Final output is a binary file that could be programmed in to an EPROM. There is no executable file format.

Extra files can accompany the binary output, that provide extra meta data about the binary.

- debug symbols
- (LLVM) byte code (for linker optimization or interpretation)
- source code (zipped)
- symbol table
- modules, exports and imports

## Packaging Features

targeting memory banks

auto packing code into fixed size banks

bank switching thunks

binary (byte code) format for late linking with optional optimization.

---

module locator? How are modules located? After the code is build for a module a module-info file is generated that advertises the module and allows it to be referenced during linking.

packager: bundle multiple modules and their support files into a package (probably a zip file). Packager is a function in the build library. A package has a version.

package manager? Retrieve 3rd party packages and register packages for distribution (typical).
