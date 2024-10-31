# Package

A file describing the NuGet package to be published.

A package can contain multiple assemblies.
The file can contain multiple package definitions.

> TBD: reuse the dotnet packaging format and tools (`.nuspec`).

- Package attributes
  - id, title, description, icon
  - authors, owners, copyright
  - project url, repo url, license (accept)
  - tags
- Package version
  - readme
  - release notes
- Dependencies (per target framework)
- Files (per target folder)
