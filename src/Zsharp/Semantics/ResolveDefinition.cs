using System;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class ResolveDefinition : AstVisitorWithSymbols
    {
        private readonly CompilerContext _context;

        public ResolveDefinition(CompilerContext context)
        {
            _context = context;
        }

        public override void VisitExpression(AstExpression expression)
        {
            if (expression.TypeReference != null)
                return;

            VisitChildren(expression);

            if (expression.TypeReference == null)
            {
                AstTypeReference? typeRef = null;

                // comparison operators have bool result
                if ((expression.Operator & AstExpressionOperator.MaskComparison) != 0)
                {
                    var typeDef = SymbolTable!.FindDefinition<AstTypeDefinition>(
                        AstIdentifierIntrinsic.Bool.CanonicalName, AstSymbolKind.Type);
                    typeRef = AstTypeReference.From(typeDef!);
                    SymbolTable!.Add(typeRef);

                    expression.SetTypeReference(typeRef!);

                    // resolve new created type
                    VisitTypeReference(typeRef);
                }
                else
                {
                    AstTypeReference? leftTypeRef = expression.LHS?.TypeReference;
                    AstTypeReference? rightTypeRef = expression.RHS?.TypeReference;

                    if (leftTypeRef != null)
                    {
                        typeRef = leftTypeRef;
                    }
                    else if (rightTypeRef != null)
                    {
                        typeRef = rightTypeRef;
                    }

                    Ast.Guard(typeRef, "Expression yielded no Type.");
                    // TODO: depending on the operator the type may need to be enlarged.
                    expression.SetTypeReference(typeRef!);
                }
            }
        }

        public override void VisitExpressionOperand(AstExpressionOperand operand)
        {
            if (operand.TypeReference != null)
                return;

            VisitChildren(operand);

            var expr = operand.Expression;
            if (expr != null)
            {
                Ast.Guard(expr.TypeReference, "AstExpression.TypeReference not set.");
                operand.SetTypeReference(expr.TypeReference!);
                return;
            }

            var litBool = operand.LiteralBoolean;
            if (litBool != null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable set.");

                var typeDef = SymbolTable!.FindDefinition<AstTypeDefinition>(
                    AstIdentifierIntrinsic.Bool.CanonicalName, AstSymbolKind.Type);
                Ast.Guard(typeDef, "No AstTypeDefintion was found for Boolean.");
                AssignType(operand, typeDef!);
                return;
            }

            var numeric = operand.LiteralNumeric;
            if (numeric != null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable set.");

                var typeDef = FindTypeByBitCount(numeric.GetBitCount(), numeric.Sign);
                Ast.Guard(typeDef, "No AstTypeDefintion was found by bit count.");
                AssignType(operand, typeDef!);
                return;
            }

            var litString = operand.LiteralString;
            if (litString != null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable set.");

                var typeDef = SymbolTable!.FindDefinition<AstTypeDefinition>(
                    AstIdentifierIntrinsic.Str.CanonicalName, AstSymbolKind.Type);
                Ast.Guard(typeDef, "No AstTypeDefintion was found for String.");
                AssignType(operand, typeDef!);
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

                    var typeRef = def?.TypeReference ?? FindTypeReference(var);
                    if (typeRef != null)
                    {
                        var.SetTypeReference(typeRef.MakeProxy());
                    }
                }

                if (var.TypeReference != null)
                {
                    operand.SetTypeReference(var.TypeReference);
                }
            }

            var fld = operand.FieldReference;
            if (fld?.Symbol?.Definition != null)
            {
                var enumOptDef = fld.Symbol.DefinitionAs<AstTypeDefinitionEnumOption>();
                if (enumOptDef != null)
                {
                    var enumDef = enumOptDef.ParentAs<AstTypeDefinitionEnum>();
                    operand.SetTypeReference(AstTypeReference.From(enumDef!));
                }
            }

            var fn = operand.FunctionReference;
            if (fn?.TypeReference != null)
            {
                operand.SetTypeReference(fn.TypeReference.MakeProxy());
            }
        }

        private void AssignType(AstExpressionOperand operand, AstTypeDefinition typeDef)
        {
            var typeRef = AstTypeReference.From(typeDef);
            var entry = typeRef.Symbol ?? SymbolTable!.Add(typeRef);
            if (entry.Definition == null)
                entry.AddNode(typeDef);

            operand.SetTypeReference(typeRef);
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            Ast.Guard(assign.Variable, "AstVariable not set on assign.");
            VisitChildren(assign);

            if (assign.Variable is AstVariableReference varRef &&
                varRef.VariableDefinition == null)
            {
                var entry = varRef.Symbol;

                // variable.TypeReference can be null
                var varDef = new AstVariableDefinition(varRef.TypeReference?.MakeProxy());
                varDef.SetIdentifier(varRef.Identifier!);
                varDef.SetSymbol(entry!);
                entry!.PromoteToDefinition(varDef, varRef);

                assign.SetVariableDefinition(varDef);

                // do the children again for types
                VisitChildren(assign);
            }

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
                    typeRef = AstTypeReference.From(def);
                    assign.Variable.SetTypeReference(typeRef);
                }
                else
                {
                    assign.Variable.SetTypeReference(typeRef.MakeProxy());
                }
            }
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            if (!variable.HasDefinition)
            {
                var entry = variable.Symbol;
                Ast.Guard(entry, "Variable has no Symbol.");

                var success = variable.TryResolve();

                if (!success &&
                    variable.ParentAs<AstAssignment>() == null)
                {
                    _context.UndefinedVariable(variable);
                }
            }
        }

        public override void VisitTypeFieldReferenceEnumOption(AstTypeFieldReferenceEnumOption enumOption)
        {
            VisitChildren(enumOption);

            if (enumOption.Symbol!.Definition == null &&
                !enumOption.TryResolve())
            {
                _context.UndefinedEnumeration(enumOption);
            }
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            VisitChildren(function);

            if (function.FunctionDefinition == null)
            {
                if (!function.TryResolve())
                {
                    if (function.IsTemplate)
                    {
                        var entry = function.Symbol!;
                        var templateFunction = entry.SymbolTable.FindDefinition<AstFunctionDefinition>(
                            function.Identifier!.TemplateDefinitionName, AstSymbolKind.Function);

                        var typeDef = new AstTemplateInstanceFunction(templateFunction!);

                        typeDef.Instantiate(_context, function);
                        entry.AddNode(typeDef);

                        Visit(typeDef);
                    }
                }

                // in case of overloads, TryResolve may succeed (finding the correct SymbolEntry)
                // but FunctionDefinition may still be null (FunctionReference.OverloadKey does not match functionDef)
                if (function.FunctionDefinition == null)
                {
                    var overloadDef = ResolveOverload(function);
                    if (overloadDef == null)
                    {
                        _context.UndefinedFunction(function);
                    }
                    else
                    {
                        function.Symbol!.SetOverload(function, overloadDef);
                        Visit(overloadDef);
                    }
                }
            }

            if (function.TypeReference == null &&
                function.FunctionDefinition?.TypeReference != null)
            {
                var typeRef = function.FunctionDefinition.TypeReference.MakeProxy();
                function.SetTypeReference(typeRef);
                Visit(typeRef);
            }
        }

        public override void VisitFunctionParameterReference(AstFunctionParameterReference parameter)
        {
            VisitChildren(parameter);

            // TODO: take parameter type from definition?
            if (parameter.TypeReference == null)
            {
                parameter.SetTypeReference(parameter.Expression!.TypeReference!.MakeProxy());
            }
        }

        public override void VisitTypeReference(AstTypeReference type)
        {
            if (type.TypeDefinition != null)
                return;

            Ast.Guard(SymbolTable, "ResolveTypes has no SymbolTable.");
            Ast.Guard(type?.Identifier, "AstTypeReference or AstIdentifier is null.");

            if (!type!.IsTemplateParameter)
            {
                var success = type!.TryResolve();
                if (!success)
                {
                    if (type.IsTemplate)
                    {
                        var entry = type.Symbol;
                        var templateType = entry!.SymbolTable.FindDefinition<AstTypeDefinitionStruct>(
                            type.Identifier!.TemplateDefinitionName, AstSymbolKind.Type);

                        var typeDef = new AstTemplateInstanceStruct(templateType!);
                        typeDef.Instantiate(type);
                        entry.AddNode(typeDef);
                        Ast.Guard(entry.Definition, "Invalid Template Definition.");
                    }
                    else
                        _context.UndefinedType(type);
                }
            }
            VisitChildren(type!);
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

            Ast.Guard(index <= 64, "Numeric Type too large.");
            string typeName = sign == AstNumericSign.Signed ? "I" : "U";

            typeName = index switch
            {
                < 2 => typeName + "8",
                < 3 => typeName + "16",
                < 5 => typeName + "32",
                < 9 => typeName + "64",
                _ => throw new NotSupportedException(
            $"The '{typeName}{index}' Type is not supported."),
            };

            var typeDef = FindTypeDefinition(GlobalSymbols!, typeName);
            return typeDef;
        }

        private static AstTypeDefinition? FindTypeDefinition(AstSymbolTable symbols, string typeName)
            => symbols.FindDefinition<AstTypeDefinition>(typeName, AstSymbolKind.Type);

        private AstFunctionDefinition? ResolveOverload(AstFunctionReference function)
        {
            // TODO: more elaborate overload resolution here...
            return function.Symbol!.Overloads.SingleOrDefault(def => def.OverloadKey == function.OverloadKey);
        }
    }
}
