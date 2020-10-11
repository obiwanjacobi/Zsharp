using System;
using Zlang.NET.AST;

namespace Zlang.NET.Semantics
{
    public class ResolveTypes : AstVisitor
    {
        private AstSymbolTable _globalSymbols;
        private AstSymbolTable _symbolTable;

        public void Apply(AstModule module)
        {
            VisitModule(module);
        }

        public void Apply(AstFile file)
        {
            VisitFile(file);
        }

        public override void VisitTypeReference(AstTypeReference type)
        {
            if (type.TypeDefinition != null)
                return;

            Ast.Guard(_symbolTable, "ResolveTypes has no SymbolTable.");
            Ast.Guard(type?.Identifier, "AstTypeReference or AstIdentifier is null.");

            var entry = _symbolTable.GetEntry(type!.Identifier!.Name, AstSymbolKind.Type);
            if (entry != null)
            {
                var def = entry.GetDefinition<AstTypeDefinition>();
                Ast.Guard(def, "Type Symbol Entry has no AstTypeDefinition.");
                type.SetTypeDefinition(def!);
            }

            VisitChildren(type);
        }

        public override void VisitCodeBlock(AstCodeBlock codeBlock)
        {
            var symbols = SetSymbolTable(codeBlock.Symbols);
            VisitChildren(codeBlock);
            SetSymbolTable(symbols);
        }

        public override void VisitFile(AstFile file)
        {
            var symbols = SetSymbolTable(file.Symbols);
            VisitChildren(file);
            SetSymbolTable(symbols);
        }

        public override void VisitExpression(AstExpression expression)
        {
            VisitChildren(expression);

            if (expression.TypeReference == null)
            {
                AstTypeReference? leftTypeRef = expression.LHS?.TypeReference;
                AstTypeReference? rightTypeRef = expression.RHS?.TypeReference;

                if (leftTypeRef != null && rightTypeRef != null)
                {
                    Ast.Guard(leftTypeRef.IsEqual(rightTypeRef), "AstExpression has non-equal Types References.");
                }

                AstTypeReference? typeRef = null;
                if (leftTypeRef != null)
                {
                    typeRef = AstTypeReference.Create(leftTypeRef);
                }
                else if (rightTypeRef != null)
                {
                    typeRef = AstTypeReference.Create(rightTypeRef);
                }
                Ast.Guard(typeRef, "Expression yielded no Type.");
                expression.SetTypeReference(typeRef!);
            }

        }

        public override void VisitExpressionOperand(AstExpressionOperand operand)
        {
            VisitChildren(operand);

            if (operand.TypeReference != null)
                return;

            var expr = operand.Expression;
            if (expr != null)
            {
                Ast.Guard(expr.TypeReference, "AstExpression.TypeReference is null.");
                var typeRef = AstTypeReference.Create(expr.TypeReference!);
                operand.SetTypeReference(typeRef);
                return;
            }

            var numeric = operand.Numeric;
            if (numeric != null)
            {
                var typeDef = FindTypeByBitCount(numeric.GetBitCount(), numeric.Sign);
                Ast.Guard(typeDef, "No AstTypeDefintion was found by bit count.");
                var typeRef = AstTypeReference.Create(numeric, typeDef!);
                operand.SetTypeReference(typeRef);
                return;
            }

            var var = operand.VariableReference;
            if (var != null)
            {
                // TODO
            }
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            Ast.Guard(_symbolTable, "ResolveTypes has no SymbolTable.");
            VisitChildren(assign);

            if (assign.Variable is AstVariableDefinition varDef &&
                varDef.TypeReference == null)
            {
                // typeless assign of var (x = 42)
                var expr = assign.Expression;
                Ast.Guard(expr, "AstExpression not set on AstAssignement.");
                var typeRef = expr!.TypeReference;
                Ast.Guard(typeRef, "AstTypeReference not set on AstExpression.");
                Ast.Guard(typeRef!.Identifier, "AstIdentifier not set on AstTypeReference.");
                var entry = _symbolTable.GetEntry(typeRef!.Identifier!.Name, AstSymbolKind.Type);
                if (entry != null)
                {
                    var def = entry.GetDefinition<AstTypeDefinition>();
                    Ast.Guard(def, "AstTypeDefinition not set on AstSymbolEntry.");
                    typeRef = AstTypeReference.Create(expr, def!);
                    varDef.SetTypeReference(typeRef);
                }
            }
        }

        private AstTypeDefinition? FindTypeByBitCount(UInt32 bitCount, AstNumericSign sign)
        {
            Ast.Guard(_globalSymbols, "ResolveTypes has no Global SymbolTable.");

            var index = bitCount / 8;
            if (bitCount % 8 > 0)
                index++;

            Ast.Guard(index <= 32, "Numeric Type too large.");

            AstTypeDefinition? typeDef = null;
            string typeName = sign == AstNumericSign.Signed ? "I" : "U";

            switch (index)
            {
                case 2:
                    typeDef = FindType(_globalSymbols, typeName + "16");
                    break;
                case 3:
                    typeDef = FindType(_globalSymbols, typeName + "24");
                    break;
                case 4:
                    typeDef = FindType(_globalSymbols, typeName + "32");
                    break;
                default:
                    typeDef = FindType(_globalSymbols, typeName + "8");
                    break;
            }
            return typeDef;
        }

        private AstTypeDefinition? FindType(AstSymbolTable symbols, string typeName)
        {
            var entry = symbols.GetEntry(typeName, AstSymbolKind.Type);
            if (entry != null)
            {
                return entry.GetDefinition<AstTypeDefinition>();
            }
            return null;
        }

        private AstSymbolTable SetSymbolTable(AstSymbolTable symbolTable)
        {
            if (_globalSymbols == null)
            {
                _globalSymbols = symbolTable;
            }

            var symbols = _symbolTable;
            _symbolTable = symbolTable;
            return symbols;
        }
    }
}
