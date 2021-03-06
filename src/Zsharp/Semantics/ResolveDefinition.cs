﻿using System;
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

            var externals = file.Symbols.FindSymbols(AstSymbolKind.Module)
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

            expression.VisitChildren(this);

            if (expression.TypeReference is null)
            {
                // comparison operators have bool result
                if ((expression.Operator & AstExpressionOperator.MaskComparison) != 0)
                {
                    var typeDef = SymbolTable!.FindDefinition<AstTypeDefinition>(
                        AstIdentifierIntrinsic.Bool.CanonicalName, AstSymbolKind.Type);
                    var typeRefType = AstTypeReferenceType.From(typeDef!);
                    SymbolTable!.Add(typeRefType);

                    expression.SetTypeReference(typeRefType!);

                    // resolve new created type
                    VisitTypeReferenceType(typeRefType);
                }
                else
                {
                    AstTypeReference? typeRef = null;
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
                    expression.SetTypeReference(typeRef!.MakeCopy());
                    Visit(expression.TypeReference!);
                }
            }
        }

        public override void VisitExpressionOperand(AstExpressionOperand operand)
        {
            operand.VisitChildren(this);

            if (operand.TypeReference is not null)
                return;

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
                AssignInferredType(operand, typeDef!);
                return;
            }

            var numeric = operand.LiteralNumeric;
            if (numeric is not null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable set.");

                var typeDef = FindTypeByBitCount(numeric.GetBitCount(), numeric.Sign);
                Ast.Guard(typeDef, "No AstTypeDefintion was found by bit count.");
                AssignInferredType(operand, typeDef!);
                return;
            }

            var litString = operand.LiteralString;
            if (litString is not null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable set.");

                var typeDef = SymbolTable!.FindDefinition<AstTypeDefinition>(
                    AstIdentifierIntrinsic.Str.CanonicalName, AstSymbolKind.Type);
                Ast.Guard(typeDef, "No AstTypeDefintion was found for String.");
                AssignInferredType(operand, typeDef!);
                return;
            }

            var var = operand.VariableReference;
            if (var is not null)
            {
                Ast.Guard(SymbolTable, "No SymbolTable was set.");
                Ast.Guard(var.Identifier, "Variable has no Identifier");

                if (var.TypeReference is null)
                {
                    var def = (IAstTypeReferenceSite?)
                        var.VariableDefinition ?? var.ParameterDefinition;

                    var typeRef = def?.TypeReference;
                    if (typeRef is not null)
                        var.SetTypeReference(typeRef.MakeCopy());
                }

                if (var.TypeReference is not null)
                    operand.SetTypeReference(var.TypeReference.MakeCopy());
            }

            var fld = operand.FieldReference;
            if (fld?.Symbol?.Definition is not null)
            {
                var enumOptDef = fld.Symbol.DefinitionAs<AstTypeDefinitionEnumOption>();
                if (enumOptDef is not null)
                {
                    var enumDef = enumOptDef.ParentAs<AstTypeDefinitionEnum>();
                    operand.SetTypeReference(AstTypeReferenceType.From(enumDef!));
                }
            }

            var fn = operand.FunctionReference;
            if (fn?.FunctionType.TypeReference is not null)
            {
                operand.SetTypeReference(fn.FunctionType.TypeReference.MakeCopy());
            }

            if (operand.TypeReference is not null)
                Visit(operand.TypeReference);
        }

        private void AssignInferredType(AstExpressionOperand operand, AstTypeDefinition typeDef)
        {
            var typeRef = AstTypeReferenceType.From(typeDef);
            typeRef.IsInferred = true;
            SymbolTable!.Add(typeRef);
            operand.SetTypeReference(typeRef);
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            Ast.Guard(assign.Variable, "AstVariable not set on assign.");
            assign.VisitChildren(this);

            if (assign.Variable is AstVariableReference varRef &&
                varRef.VariableDefinition is null)
            {
                var symbol = varRef.Symbol;

                // Variable.TypeReference is usually null.
                // It is set only when type is explicitly in source code, or has been inferred.
                var varDef = new AstVariableDefinition(varRef.TypeReference?.MakeCopy());
                varDef.SetIdentifier(varRef.Identifier!);
                varDef.SetSymbol(symbol!);
                symbol!.PromoteToDefinition(varDef, varRef);
                AstSymbolReferenceRemover.RemoveReference(varRef);
                assign.SetVariableDefinition(varDef);

                // do the children again for types
                assign.VisitChildren(this);
            }

            if (assign.Variable!.TypeReference is null)
            {
                // typeless assign of var (x = 42)
                var expr = assign.Expression;
                Ast.Guard(expr, "AstExpression not set on AstAssignement.");

                var typeRef = expr!.TypeReference;
                Ast.Guard(typeRef, "AstTypeReference not set on AstExpression.");
                Ast.Guard(typeRef!.Identifier, "AstIdentifier not set on AstTypeReference.");

                var symbol = assign.Variable.Symbol!;
                Ast.Guard(symbol, "AstSymbolEntry was not set on Variable.");

                var def = symbol.DefinitionAs<AstTypeDefinition>();
                if (def is not null)
                {
                    typeRef = AstTypeReferenceType.From(def);
                    assign.Variable.SetTypeReference(typeRef);
                }
                else
                {
                    assign.Variable.SetTypeReference(typeRef.MakeCopy());
                }

                Visit(assign.Variable.TypeReference!);
            }
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            if (!variable.HasDefinition)
            {
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
            enumOption.VisitChildren(this);

            if (enumOption.Symbol!.Definition is null &&
                !enumOption.TryResolveSymbol())
            {
                _context.UndefinedEnumeration(enumOption);
            }
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            function.VisitChildren(this);

            if (function.FunctionDefinition is null)
            {
                if (!function.TryResolveSymbol())
                {
                    if (function.IsTemplate)
                    {
                        var symbol = function.Symbol!;
                        var templateFunction = FindTemplateDefinition<AstFunctionDefinition>(function, AstSymbolKind.Function);

                        if (templateFunction is not null)
                        {
                            if (!templateFunction.IsExternal)
                            {
                                var typeDef = new AstTemplateInstanceFunction(templateFunction!);

                                typeDef.Instantiate(_context, function);
                                symbol.AddNode(typeDef);

                                Visit(typeDef);
                            }
                            else
                            {
                                symbol.AddNode(templateFunction);
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
                        function.VisitChildren(this);
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
                var typeRef = function.FunctionDefinition.FunctionType.TypeReference.MakeCopy();
                function.FunctionType.SetTypeReference(typeRef);
                // if type is intrinsic the symbol may not be set.
                if (typeRef.Symbol is null)
                    function.Symbol!.SymbolTable.Add(typeRef);
                Visit(typeRef);
            }
        }

        public override void VisitFunctionParameterReference(AstFunctionParameterReference parameter)
        {
            parameter.VisitChildren(this);

            if (parameter.TypeReference is null)
            {
                parameter.SetTypeReference(parameter.Expression!.TypeReference!.MakeCopy());
            }
        }

        public override void VisitTypeReferenceType(AstTypeReferenceType type)
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
                        var symbol = type.Symbol!;
                        var typeTemplate = FindTemplateDefinition<AstTypeDefinition>(type, AstSymbolKind.Type);

                        if (typeTemplate is AstTypeDefinitionStruct structTemplate)
                        {
                            var typeDef = new AstTemplateInstanceStruct(structTemplate);
                            typeDef.Instantiate(type);
                            symbol.AddNode(typeDef);
                            Ast.Guard(symbol.Definition, "Invalid Template Definition.");
                        }
                        else if (typeTemplate is AstTypeDefinitionIntrinsic intrinsicTemplate)
                        {
                            var typeDef = new AstTemplateInstanceType(intrinsicTemplate);
                            typeDef.Instantiate(type);
                            symbol.AddNode(typeDef);
                            Ast.Guard(symbol.Definition, "Invalid Template Definition.");
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

            type!.VisitChildren(this);
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

        private T? FindTemplateDefinition<T>(IAstIdentifierSite identifierSite, AstSymbolKind symbolKind)
            where T : class
        {
            var templateDef = SymbolTable!.FindDefinition<T>(
                identifierSite.Identifier!.SymbolName.TemplateDefinitionName, symbolKind);

            if (templateDef is null)
                templateDef = SymbolTable!.FindDefinition<T>(
                    identifierSite.Identifier!.SymbolName.GenericDefinitionName, symbolKind);

            return templateDef;
        }


        private static AstTypeDefinition? FindTypeDefinition(AstSymbolTable symbols, string typeName)
            => symbols.FindDefinition<AstTypeDefinition>(typeName, AstSymbolKind.Type);

        private bool MatchFunctionToDefinition(AstFunctionReference function)
        {
            var symbol = function.Symbol!;

            if (!symbol.HasDefinition)
                return false;

            var functionDef = symbol.FindFunctionDefinition(function);

            if (functionDef is null)
            {
                var resolvedDef = AstTypeMatcher.ResolveOverloads(function);

                if (resolvedDef is not null)
                {
                    if (SetToMatch(function, resolvedDef))
                    {
                        Visit(function.FunctionType);
                    }

                    // non-null if SetToMatch successful
                    functionDef = symbol.FindFunctionDefinition(function);
                }
            }

            return functionDef is not null;
        }

        private bool SetToMatch(AstFunctionReference function, AstFunctionDefinition functionDef)
        {
            Ast.Guard(function.FunctionType.Parameters.Count() ==
                functionDef.FunctionType.Parameters.Count(), "Number of Parameters don't match between function reference and definition");

            bool hasReplacements = false;
            var parameters = function.FunctionType.Parameters.ToList();
            var parameterDefs = functionDef.FunctionType.Parameters.ToList();
            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                if (parameter.TypeReference is null ||
                    parameter.TypeReference.IsInferred)
                {
                    var paramDef = parameterDefs[i];
                    var typeRef = paramDef.TypeReference!.MakeCopy();
                    var oldTypeRef = parameter.ReplaceTypeReference(typeRef);
                    if (oldTypeRef is not null)
                        AstSymbolReferenceRemover.RemoveReference(oldTypeRef);
                    SymbolTable!.Add(typeRef);
                    hasReplacements = true;
                }
            }

            if ((function.FunctionType.TypeReference is null ||
                    function.FunctionType.TypeReference.IsInferred) &&
                functionDef.FunctionType.TypeReference is not null)
            {
                var typeRef = functionDef.FunctionType.TypeReference!.MakeCopy();
                var oldTypeRef = function.FunctionType.ReplaceTypeReference(typeRef);
                if (oldTypeRef is not null)
                    AstSymbolReferenceRemover.RemoveReference(oldTypeRef);
                SymbolTable!.Add(typeRef);
                hasReplacements = true;
            }

            return hasReplacements;
        }
    }
}
