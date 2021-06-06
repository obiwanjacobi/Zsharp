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

        public override void VisitFile(AstFile file)
        {
            var symbolTable = SetSymbolTable(_context.Modules.SymbolTable);

            var externals = file.Symbols.FindEntries(AstSymbolKind.Module)
                .Select(s => s.DefinitionAs<AstModuleExternal>())
                .Where(m => m is not null);

            foreach (var mod in externals)
            {
                VisitModuleExternal(mod!);
            }

            SetSymbolTable(symbolTable);

            base.VisitFile(file);
        }

        public override void VisitExpression(AstExpression expression)
        {
            if (expression.TypeReference is not null)
                return;

            VisitChildren(expression);

            if (expression.TypeReference is null)
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

                    if (leftTypeRef is not null)
                    {
                        typeRef = leftTypeRef;
                    }
                    else if (rightTypeRef is not null)
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
            if (operand.TypeReference is not null)
                return;

            VisitChildren(operand);

            var expr = operand.Expression;
            if (expr is not null)
            {
                Ast.Guard(expr.TypeReference, "AstExpression.TypeReference not set.");
                operand.SetTypeReference(expr.TypeReference!);
                return;
            }

            var litBool = operand.LiteralBoolean;
            if (litBool is not null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable set.");

                var typeDef = SymbolTable!.FindDefinition<AstTypeDefinition>(
                    AstIdentifierIntrinsic.Bool.CanonicalName, AstSymbolKind.Type);
                Ast.Guard(typeDef, "No AstTypeDefintion was found for Boolean.");
                AssignType(operand, typeDef!);
                return;
            }

            var numeric = operand.LiteralNumeric;
            if (numeric is not null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable set.");

                var typeDef = FindTypeByBitCount(numeric.GetBitCount(), numeric.Sign);
                Ast.Guard(typeDef, "No AstTypeDefintion was found by bit count.");
                AssignType(operand, typeDef!);
                return;
            }

            var litString = operand.LiteralString;
            if (litString is not null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable set.");

                var typeDef = SymbolTable!.FindDefinition<AstTypeDefinition>(
                    AstIdentifierIntrinsic.Str.CanonicalName, AstSymbolKind.Type);
                Ast.Guard(typeDef, "No AstTypeDefintion was found for String.");
                AssignType(operand, typeDef!);
                return;
            }

            var var = operand.VariableReference;
            if (var is not null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable was set.");
                Ast.Guard(var.Identifier, "Variable has no Identifier");

                if (var.TypeReference is null)
                {
                    var def = (IAstTypeReferenceSite?)var.VariableDefinition
                        ?? (IAstTypeReferenceSite?)var.ParameterDefinition;

                    var typeRef = def?.TypeReference ?? FindTypeReference(var);
                    if (typeRef is not null)
                    {
                        var.SetTypeReference(typeRef.MakeProxy());
                    }
                }

                if (var.TypeReference is not null)
                {
                    operand.SetTypeReference(var.TypeReference);
                }
            }

            var fld = operand.FieldReference;
            if (fld?.Symbol?.Definition is not null)
            {
                var enumOptDef = fld.Symbol.DefinitionAs<AstTypeDefinitionEnumOption>();
                if (enumOptDef is not null)
                {
                    var enumDef = enumOptDef.ParentAs<AstTypeDefinitionEnum>();
                    operand.SetTypeReference(AstTypeReference.From(enumDef!));
                }
            }

            var fn = operand.FunctionReference;
            if (fn?.FunctionType.TypeReference is not null)
            {
                operand.SetTypeReference(fn.FunctionType.TypeReference.MakeProxy());
            }
        }

        private void AssignType(AstExpressionOperand operand, AstTypeDefinition typeDef)
        {
            var typeRef = AstTypeReference.From(typeDef);
            var entry = typeRef.Symbol ?? SymbolTable!.Add(typeRef);
            if (entry.Definition is null)
                entry.AddNode(typeDef);

            operand.SetTypeReference(typeRef);
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            Ast.Guard(assign.Variable, "AstVariable not set on assign.");
            VisitChildren(assign);

            if (assign.Variable is AstVariableReference varRef &&
                varRef.VariableDefinition is null)
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

            if (assign.Variable!.TypeReference is null)
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
                if (def is not null)
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

                var success = variable.TryResolveSymbol();

                if (!success &&
                    variable.ParentAs<AstAssignment>() is null)
                {
                    _context.UndefinedVariable(variable);
                }
            }
        }

        public override void VisitTypeFieldReferenceEnumOption(AstTypeFieldReferenceEnumOption enumOption)
        {
            VisitChildren(enumOption);

            if (enumOption.Symbol!.Definition is null &&
                !enumOption.TryResolveSymbol())
            {
                _context.UndefinedEnumeration(enumOption);
            }
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            VisitChildren(function);

            if (function.FunctionDefinition is null)
            {
                if (!function.TryResolveSymbol())
                {
                    if (function.FunctionType.IsTemplate)
                    {
                        var entry = function.Symbol!;
                        var templateFunction = entry.SymbolTable.FindDefinition<AstFunctionDefinition>(
                            function.Identifier!.SymbolName.TemplateDefinitionName, AstSymbolKind.Function);

                        if (templateFunction is not null)
                        {
                            if (!templateFunction.IsExternal)
                            {
                                var typeDef = new AstTemplateInstanceFunction(templateFunction!);

                                typeDef.Instantiate(_context, function);
                                entry.AddNode(typeDef);

                                Visit(typeDef);
                            }
                            else
                            {
                                entry.AddNode(templateFunction);
                            }
                        }
                    }
                }

                // in case of overloads, TryResolve may succeed (finding the correct SymbolEntry)
                // but FunctionDefinition may still be null (FunctionReference.OverloadKey does not match functionDef)
                if (function.FunctionDefinition is null)
                {
                    if (MatchFunctionToDefinition(function))
                    {
                        // make sure all new types are resolved.
                        VisitChildren(function);
                    }
                    else
                    {
                        _context.UndefinedFunction(function);
                    }
                }
            }

            if (function.FunctionType.TypeReference is null &&
                function.FunctionDefinition?.FunctionType.TypeReference is not null)
            {
                var typeRef = function.FunctionDefinition.FunctionType.TypeReference.MakeProxy();
                function.FunctionType.SetTypeReference(typeRef);
                // if type is intrinsic the symbol may not be set.
                if (typeRef.Symbol is null)
                    function.Symbol!.SymbolTable.Add(typeRef);
                Visit(typeRef);
            }
        }

        public override void VisitFunctionParameterReference(AstFunctionParameterReference parameter)
        {
            VisitChildren(parameter);

            // TODO: take parameter type from definition?
            if (parameter.TypeReference is null)
            {
                parameter.SetTypeReference(parameter.Expression!.TypeReference!.MakeProxy());
            }
        }

        public override void VisitTypeReference(AstTypeReference type)
        {
            if (type.TypeDefinition is not null)
                return;

            Ast.Guard(SymbolTable, "ResolveTypes has no SymbolTable.");
            Ast.Guard(type?.Identifier, "AstTypeReference or AstIdentifier is null.");

            if (!type!.IsTemplateParameter)
            {
                var success = type!.TryResolveSymbol();
                if (!success)
                {
                    if (type.IsTemplate)
                    {
                        var entry = type.Symbol;
                        var typeTemplate = entry!.SymbolTable.FindDefinition<AstTypeDefinition>(
                            type.Identifier!.SymbolName.TemplateDefinitionName, AstSymbolKind.Type);

                        if (typeTemplate is AstTypeDefinitionStruct structTemplate)
                        {
                            var typeDef = new AstTemplateInstanceStruct(structTemplate);
                            typeDef.Instantiate(type);
                            entry.AddNode(typeDef);
                            Ast.Guard(entry.Definition, "Invalid Template Definition.");
                        }
                        else if (typeTemplate is AstTypeDefinitionIntrinsic intrinsicTemplate)
                        {
                            var typeDef = new AstTemplateInstanceType(intrinsicTemplate);
                            typeDef.Instantiate(type);
                            entry.AddNode(typeDef);
                            Ast.Guard(entry.Definition, "Invalid Template Definition.");
                        }
                        else
                            _context.UndefinedType(type);
                    }
                    else if (!type.IsExternal)
                    {
                        // TODO: for now unresolved external references are ignored.
                        _context.UndefinedType(type);
                    }
                }
            }

            VisitChildren(type!);
        }

        private AstTypeReference? FindTypeReference(AstNode? node)
        {
            if (node is null)
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

            if (parent is not null && typeRef is null)
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

        private bool MatchFunctionToDefinition(AstFunctionReference function)
        {
            var entry = function.Symbol!;

            if (!entry.HasDefinition)
                return false;

            //if (!entry.HasOverloads)
            //{
            //    SetToMatch(function, entry.DefinitionAs<AstFunctionDefinition>()!);
            //    return true;
            //}

            // TODO: Find closest match of overloads...
            return entry.FindFunctionDefinition(function) is not null;
        }

        // TODO: this needs more refinement than blindly overwriting types.
        // - Only overwrite if no type was found
        // - only overwrite if the current and new type are compatible / implicit convertable
        private void SetToMatch(AstFunctionReference functionRef, AstFunctionDefinition functionDef)
        {
            var oldTypeRefs =
                functionRef.FunctionType.Parameters.Select(p => p.TypeReference!).ToList();

            if (functionRef.FunctionType.TypeReference is not null)
                oldTypeRefs.Add(functionRef.FunctionType.TypeReference!);

            // remove the type-references from the symbol table that are about to be overwritten
            functionRef.Symbol!.SymbolTable.RemoveReferences(oldTypeRefs);

            functionRef.OverrideTypes(
                functionDef.FunctionType.TypeReference,
                functionDef.FunctionType.Parameters.Select(p => p.TypeReference!));

            // TODO: ParameterRef.Expression can have different TypeReference than the parameter.
        }
    }
}
