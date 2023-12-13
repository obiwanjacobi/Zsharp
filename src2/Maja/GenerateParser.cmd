java -jar antlr4/antlr-4.13.1-complete.jar -Dlanguage=CSharp -message-format antlr -o Maja.Compiler/Parser -package Maja.Compiler.Parser -no-listener -visitor Maja.Compiler/MajaLexer.g4
java -jar antlr4/antlr-4.13.1-complete.jar -Dlanguage=CSharp -message-format antlr -o Maja.Compiler/Parser -package Maja.Compiler.Parser -no-listener -visitor Maja.Compiler/MajaParser.g4
