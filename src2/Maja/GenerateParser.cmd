java -jar antlr4/antlr-4.11.1-complete.jar -Dlanguage=CSharp -message-format antlr -o Parser -package Maja.Compiler.Parser -no-listener -visitor Maja.Compiler/MajaLexer.g4
java -jar antlr4/antlr-4.11.1-complete.jar -Dlanguage=CSharp -message-format antlr -o Parser -package Maja.Compiler.Parser -no-listener -visitor Maja.Compiler/MajaParser.g4
