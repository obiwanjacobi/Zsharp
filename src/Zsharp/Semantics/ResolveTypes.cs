﻿using System;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class ResolveTypes : AstVisitorWithSymbols
    {
        public void Apply(AstModule module) => VisitModule(module);

        public void Apply(AstFile file) => VisitFile(file);

        public override void VisitTypeReference(AstTypeReference type)
        {
            if (type.TypeDefinition != null)
                return;

            Ast.Guard(SymbolTable, "ResolveTypes has no SymbolTable.");
            Ast.Guard(type?.Identifier, "AstTypeReference or AstIdentifier is null.");

            var entry = SymbolTable!.Find(type!);
            // will not be found if external
            if (entry != null)
            {
                var def = entry.DefinitionAs<AstTypeDefinition>();
                Ast.Guard(def, "Type Symbol Entry has no AstTypeDefinition.");
                type!.SetTypeDefinition(def!);
            }

            VisitChildren(type!);
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
                    // TODO: implicit conversion compiler error
                    Ast.Guard(leftTypeRef.IsEqual(rightTypeRef), "AstExpression has non-equal Types References.");
                }

                AstTypeReference? typeRef = null;
                if (leftTypeRef != null)
                {
                    typeRef = leftTypeRef;
                }
                else if (rightTypeRef != null)
                {
                    typeRef = rightTypeRef;
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
                Ast.Guard(expr.TypeReference, "AstExpression.TypeReference not set.");
                operand.SetTypeReference(expr.TypeReference!);
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
                Ast.Guard(SymbolTable, "No SymbolTable was set.");
                Ast.Guard(var.Identifier, "Variable has no Identifier");

                if (var.TypeReference == null)
                {
                    var def = (IAstTypeReferenceSite?)var.VariableDefinition
                        ?? (IAstTypeReferenceSite?)var.ParameterDefinition;

                    Ast.Guard(def, "No VariableDefinition or ParameterDefintion was set.");

                    var typeRef = def?.TypeReference
                        ?? FindTypeReference(var);

                    if (typeRef != null)
                    {
                        var.SetTypeReference(new AstTypeReference(typeRef));
                    }
                }

                operand.SetTypeReference(var.TypeReference!);
            }
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            Ast.Guard(assign.Variable, "AstVariable not set on assign.");
            VisitChildren(assign);

            if (assign.Variable!.TypeReference == null)
            {
                // typeless assign of var (x = 42)
                var expr = assign.Expression;
                Ast.Guard(expr, "AstExpression not set on AstAssignement.");

                var typeRef = expr!.TypeReference;
                Ast.Guard(typeRef, "AstTypeReference not set on AstExpression.");
                Ast.Guard(typeRef!.Identifier, "AstIdentifier not set on AstTypeReference.");

                var entry = assign.Variable.Symbol;
                Ast.Guard(entry, "AstSymbolEntry was not set on Variable.");

                var def = entry!.DefinitionAs<AstTypeDefinition>();
                if (def != null)
                {
                    typeRef = AstTypeReference.Create(expr, def);
                    assign.Variable.SetTypeReference(typeRef);
                }
                else
                {
                    assign.Variable.SetTypeReference(new AstTypeReference(typeRef));
                }
            }
        }

        private AstTypeReference? FindTypeReference(AstNode? node)
        {
            if (node == null)
                return null;

            if (node is AstTypeReference typeRefNode)
                return typeRefNode;

            AstTypeReference? typeRef = null;

            // down into expression
            if (node is AstExpression expression)
            {
                typeRef = expression.TypeReference
                    ?? expression.RHS?.TypeReference
                    ?? expression.LHS?.TypeReference
                    ?? FindTypeReference(expression.RHS?.Expression)
                    ?? FindTypeReference(expression.LHS?.Expression)
                    ;
            }
            else if (node is IAstTypeReferenceSite typeRefSite)
            {
                typeRef = typeRefSite.TypeReference;
            }

            // up to parent
            var parent = node.Parent;

            if (parent != null && typeRef == null)
            {
                typeRef = FindTypeReference(parent);
            }

            return typeRef;
        }

        private AstTypeDefinition? FindTypeByBitCount(UInt32 bitCount, AstNumericSign sign)
        {
            Ast.Guard(GlobalSymbols, "ResolveTypes has no Global SymbolTable.");

            var index = bitCount / 8;
            if (bitCount % 8 > 0)
                index++;

            Ast.Guard(index <= 32, "Numeric Type too large.");

            AstTypeDefinition? typeDef = null;
            string typeName = sign == AstNumericSign.Signed ? "I" : "U";

            switch (index)
            {
                case 0:
                case 1:
                    typeDef = FindTypeDefinition(GlobalSymbols!, typeName + "8");
                    break;
                case 2:
                    typeDef = FindTypeDefinition(GlobalSymbols!, typeName + "16");
                    break;
                case 3:
                case 4:
                    typeDef = FindTypeDefinition(GlobalSymbols!, typeName + "32");
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                    typeDef = FindTypeDefinition(GlobalSymbols!, typeName + "64");
                    break;
                default:
                    throw new NotSupportedException(
                        $"The '{typeName}{index}' Type is not supported.");
            }

            return typeDef;
        }

        private static AstTypeDefinition? FindTypeDefinition(AstSymbolTable symbols, string typeName)
            => symbols.FindDefinition<AstTypeDefinition>(typeName, AstSymbolKind.Type);
    }
}
