# Compilation

Once the SyntaxTree has been parsed from source code, 
it can be passed to Compilation to produce a SemanticModel.

The Compilation will analyze the syntax nodes and identify and resolve symbols
and construct a SemanticModel.

The SemanticModel can then be used to further analyze the resulting structure 
and to emit code based on the SemanticModel.