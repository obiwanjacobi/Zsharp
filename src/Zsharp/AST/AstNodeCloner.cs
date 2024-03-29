﻿using System;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    /// <summary>
    /// Clones templated code into an instantiation.
    /// </summary>
    public class AstNodeCloner : AstVisitor
    {
        private readonly AstBuilderContext? _context;
        private readonly AstCurrentContext _current;
        private AstTemplateArgumentMap? _argumentMap;

        public AstNodeCloner()
        {
            _current = new AstCurrentContext();
        }

        public AstNodeCloner(CompilerContext context, uint indent = 0)
        {
            _current = _context = new AstBuilderContext(context, indent);
        }

        public void Clone(AstFunctionReference functionRef,
            AstFunctionDefinition templateFunctionDef, AstTemplateInstanceFunction instanceFunction,
            AstTemplateArgumentMap argumentMap)
        {
            if (templateFunctionDef is AstFunctionDefinition functionDef)
            {
                _argumentMap = argumentMap;
                _current.SetCurrent(instanceFunction.FunctionType);
                _current.SetCurrent(instanceFunction);
                functionDef.VisitChildren(this);
                _current.RevertCurrent();
                _current.RevertCurrent();

                instanceFunction.SetIdentifier(functionRef.Identifier);

                AstSymbolTable symbols;
                if (templateFunctionDef is IAstSymbolTableSite symbolSite)
                {
                    // registered in impl function defs symbol table
                    symbols = symbolSite.SymbolTable;
                }
                else
                {
                    // intrinsics are registered in root symbol table
                    symbols = functionRef.Symbol.SymbolTable.GetRootTable();
                }

                instanceFunction.CreateSymbols(symbols);
            }
            else
            {
                throw new NotImplementedException(
                    "[Interop] .NET Generics not implemented yet.");
            }
        }

        public T? Clone<T>(T node) where T : AstNode, IAstCodeBlockLine
        {
            Ast.Guard(_context is not null, "Must construct with CompilerContext.");
            AstCodeBlock cb = new AstCodeBlock("Clone", _context!.CompilerContext.IntrinsicSymbols);

            _current.SetCurrent(cb);
            Visit(node);
            _current.RevertCurrent();

            var result = cb.LineAt<T>(0);
            result?.Orphan();

            return result;
        }

        //---------------------------------------------------------------------

        public override void VisitModulePublic(AstModuleImpl module)
        {
            throw new InvalidOperationException("AstNodeCloner cannot clone a Module.");
        }

        public override void VisitFile(AstFile file)
        {
            throw new InvalidOperationException("AstNodeCloner cannot clone a File.");
        }

        public override void VisitCodeBlock(AstCodeBlock codeBlock)
        {
            var cb = new AstCodeBlock(
                codeBlock.SymbolTable.Name, codeBlock.SymbolTable!.ParentTable!, codeBlock.Context);

            var site = _current.GetCurrent<IAstCodeBlockSite>();
            site.SetCodeBlock(cb);

            _current.SetCurrent(cb);
            codeBlock.VisitChildren(this);
            _current.RevertCurrent();
        }

        public override void VisitFunctionDefinition(AstFunctionDefinition function)
        {
            if (function is AstFunctionDefinitionImpl functionDef)
            {
                var fnDef = new AstFunctionDefinitionImpl((Function_defContext)functionDef.Context!);
                fnDef.SetIdentifier(functionDef.Identifier);

                var codeBlock = _current.GetCurrent<AstCodeBlock>();
                codeBlock.AddLine(fnDef);

                _current.SetCurrent(fnDef.FunctionType);
                _current.SetCurrent(fnDef);
                functionDef.VisitChildren(this);
                _current.RevertCurrent();
                _current.RevertCurrent();

                var symbols = _current.GetCurrent<IAstSymbolTableSite>();
                fnDef.CreateSymbols(symbols.SymbolTable);
            }
        }

        public override void VisitFunctionParameterDefinition(AstFunctionParameterDefinition parameter)
        {
            var paramDef = parameter.Context switch
            {
                Function_parameterContext ctx => new AstFunctionParameterDefinition(ctx, parameter.IsSelf),
                Function_parameter_selfContext ctx => new AstFunctionParameterDefinition(ctx, parameter.IsSelf),
                _ => new AstFunctionParameterDefinition(parameter.Identifier)
            };

            paramDef.TrySetIdentifier(parameter.Identifier);

            var fnDef = _current.GetCurrent<AstFunctionDefinition>();
            fnDef.FunctionType.AddParameter(paramDef);

            _current.SetCurrent(paramDef);
            parameter.VisitChildren(this);
            _current.RevertCurrent();

            // parameter symbols are registered with FunctionDefinition.CreateSymbols()
        }

        public override void VisitFunctionParameterArgument(AstFunctionParameterArgument argument)
        {
            var arg = new AstFunctionParameterArgument(argument.Context!);
            // param ref usually has no Identifier
            arg.TrySetIdentifier(argument.Identifier);

            var fnRef = _current.GetCurrent<AstFunctionReference>();
            fnRef.FunctionType.AddArgument(arg);

            _current.SetCurrent(arg);
            argument.VisitChildren(this);
            _current.RevertCurrent();
        }

        public override void VisitFunctionReference(AstFunctionReference function)
            => CloneFunctionReference(function);

        private AstFunctionReference CloneFunctionReference(AstFunctionReference function)
        {
            Ast.Guard(function.Context, "No FunctionReference Context set.");
            var fnRef = new AstFunctionReference(function.Context!, function.EnforceReturnValueUse);

            var typeArg = _argumentMap!.LookupArgument(function.Identifier);
            // a conversion function may be templated by a type (T)
            if (typeArg is null || !typeArg.HasTypeReference)
                fnRef.SetIdentifier(function.Identifier);
            else
                fnRef.SetIdentifier(typeArg.TypeReference.Identifier);

            _current.SetCurrent(fnRef.FunctionType);
            _current.SetCurrent(fnRef);
            function.VisitChildren(this);
            _current.RevertCurrent();
            _current.RevertCurrent();

            return fnRef;
        }

        public override void VisitExpression(AstExpression expression)
            => CloneExpression(expression);

        private AstExpression CloneExpression(AstExpression expression)
        {
            var expr = new AstExpression(expression.Context!)
            {
                Operator = expression.Operator
            };

            var site = _current.GetCurrent<IAstExpressionSite>();
            site.SetExpression(expr);

            _current.SetCurrent(expr);
            expression.VisitChildren(this);
            _current.RevertCurrent();

            return expr;
        }

        public override void VisitExpressionOperand(AstExpressionOperand operand)
        {
            AstNode? child = null;

            if (operand.HasExpression)
                child = CloneExpression(operand.Expression);

            if (operand.FieldReference is not null)
                child = CloneFieldReference(operand.FieldReference);

            if (operand.FunctionReference is not null)
                child = CloneFunctionReference(operand.FunctionReference);

            if (operand.LiteralBoolean is not null)
                child = CloneLiteralBoolean(operand.LiteralBoolean);

            if (operand.LiteralNumeric is not null)
                child = CloneLiteralNumeric(operand.LiteralNumeric);

            if (operand.LiteralString is not null)
                child = CloneLiteralString(operand.LiteralString);

            if (operand.VariableReference is not null)
                child = CloneVariableReference(operand.VariableReference);

            Ast.Guard(child, "Expression Operand yielded no AstNode.");
            var op = new AstExpressionOperand(child!);

            _current.SetCurrent(op);
            CloneTypeReference(operand.TypeReference);
            _current.RevertCurrent();

            var exp = _current.GetCurrent<AstExpression>();
            exp.Add(op);
        }

        private static AstLiteralBoolean CloneLiteralBoolean(AstLiteralBoolean literal)
            => new(literal.Context!, literal.Value);

        private static AstLiteralNumeric CloneLiteralNumeric(AstLiteralNumeric literal)
        {
            if (literal.Context is null)
                return new AstLiteralNumeric(literal.Value);

            return AstLiteralNumeric.Create((NumberContext)literal.Context);
        }

        private static AstLiteralString CloneLiteralString(AstLiteralString literal)
            => new((StringContext)literal.Context);

        public override void VisitAssignment(AstAssignment assign)
        {
            var a = new AstAssignment(assign.Context)
            {
                Indent = assign.Indent
            };

            var codeBlock = _current.GetCurrent<AstCodeBlock>();
            codeBlock.AddLine(a);

            _current.SetCurrent(a);

            var v = CloneVariable(assign.Variable!);
            a.SetVariable(v);

            VisitExpression(assign.Expression);
            _current.RevertCurrent();
        }

        private AstVariable CloneVariable(AstVariable variable)
        {
            return variable switch
            {
                AstVariableDefinition varDef => CloneVariableDefinition(varDef),
                AstVariableReference varRef => CloneVariableReference(varRef),
                _ => throw new InternalErrorException("AstVariable sub type not implemented.")
            };
        }

        public override void VisitVariableDefinition(AstVariableDefinition variable)
        {
            var varDef = CloneVariableDefinition(variable);

            var codeBlock = _current.GetCurrent<AstCodeBlock>();
            codeBlock.AddLine(varDef);
        }

        private AstVariableDefinition CloneVariableDefinition(AstVariableDefinition variable)
        {
            var varDef = new AstVariableDefinition(variable.Context!)
            {
                Indent = variable.Indent
            };
            varDef.SetIdentifier(variable.Identifier);

            _current.SetCurrent(varDef);
            variable.VisitChildren(this);
            _current.RevertCurrent();

            var symbols = _current.GetCurrent<IAstSymbolTableSite>();
            symbols.SymbolTable.Add(varDef);

            return varDef;
        }

        public override void VisitVariableReference(AstVariableReference variable)
            => CloneVariableReference(variable);

        private AstVariableReference CloneVariableReference(AstVariableReference variable)
        {
            var varRef = new AstVariableReference(variable.Context!);
            varRef.SetIdentifier(variable.Identifier);

            _current.SetCurrent(varRef);
            variable.VisitChildren(this);
            _current.RevertCurrent();

            var symbols = _current.GetCurrent<IAstSymbolTableSite>();
            symbols.SymbolTable.Add(varRef);

            return varRef;
        }

        public override void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
        {
            enumType.VisitChildren(this);
            throw new NotImplementedException();
        }

        public override void VisitTypeDefinitionEnumOption(AstTypeDefinitionEnumOption enumOption)
        {
            enumOption.VisitChildren(this);
            throw new NotImplementedException();
        }

        public override void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
        {
            structType.VisitChildren(this);
            throw new NotImplementedException();
        }

        public override void VisitTypeDefinitionStructField(AstTypeDefinitionStructField structField)
        {
            structField.VisitChildren(this);
            throw new NotImplementedException();
        }

        public override void VisitTypeFieldDefinition(AstTypeFieldDefinition field)
        {
            field.VisitChildren(this);
            throw new NotImplementedException();
        }

        public override void VisitTypeFieldInitialization(AstTypeFieldInitialization field)
        {
            field.VisitChildren(this);
            throw new NotImplementedException();
        }

        public override void VisitTypeFieldReferenceEnumOption(AstTypeFieldReferenceEnumOption enumOption)
        {
            var fldRef = new AstTypeFieldReferenceEnumOption(enumOption.Context!);

            _current.SetCurrent(fldRef);
            enumOption.VisitChildren(this);
            _current.RevertCurrent();
        }

        public override void VisitTypeFieldReferenceStructField(AstTypeFieldReferenceStructField structField)
        {
            var fldRef = new AstTypeFieldReferenceStructField(structField.Context!);

            _current.SetCurrent(fldRef);
            structField.VisitChildren(this);
            _current.RevertCurrent();
        }

        private static AstTypeFieldReference CloneFieldReference(AstTypeFieldReference fieldReference)
        {
            return fieldReference switch
            {
                AstTypeFieldReferenceStructField => new AstTypeFieldReferenceStructField(fieldReference.Context!),
                AstTypeFieldReferenceEnumOption => new AstTypeFieldReferenceEnumOption(fieldReference.Context!),
                _ => throw new InternalErrorException(
                    $"AstNodeCloner does not implement this FieldReference Type: {fieldReference.GetType().Name}")
            };
        }

        public override void VisitTypeReferenceType(AstTypeReferenceType type)
            => CloneTypeReference(type);

        private AstTypeReference CloneTypeReference(AstTypeReference type)
        {
            AstTypeReference typeRef;

            var templateArgument = _argumentMap?.LookupArgument(type.Identifier);
            if (templateArgument is not null)
            {
                typeRef = templateArgument.TypeReference.MakeCopy();
            }
            else if (type.TypeDefinition is IAstTemplateSite<AstTemplateParameterDefinition> templateDef &&
                templateDef.IsTemplate)
            {
                var typeRefType = AstTypeReferenceType.From(type.TypeDefinition);

                foreach (AstTemplateParameterDefinition templParamDef in templateDef.TemplateParameters)
                {
                    templateArgument = _argumentMap?.LookupArgument(templParamDef.Identifier);
                    if (templateArgument is not null)
                    {
                        var templArg = new AstTemplateParameterArgument(templateArgument.TypeReference.MakeCopy());
                        typeRefType.AddTemplateArgument(templArg);
                    }
                    else
                        throw new InternalErrorException(
                            $"Template Parameter '{templParamDef.Identifier.NativeFullName}' could not be resolved.");
                }

                typeRef = typeRefType;
            }
            else
                typeRef = type.MakeCopy();

            var site = _current.GetCurrent<IAstTypeReferenceSite>();
            site.SetTypeReference(typeRef);

            return typeRef;
        }

        public override void VisitBranch(AstBranch branch)
        {
            if (branch.BranchKind != AstBranchKind.ExitIteration &&
                branch.BranchKind != AstBranchKind.ExitLoop)
                throw new InternalErrorException("Unknown Branch Type.");

            var br = new AstBranch(branch.Context!, branch.BranchKind);
            var cb = _current.GetCurrent<AstCodeBlock>();
            cb.AddLine(br);

            _current.SetCurrent(br);
            branch.VisitChildren(this);
            _current.RevertCurrent();
        }

        public override void VisitBranchConditional(AstBranchConditional branch)
        {
            var br = new AstBranchConditional(branch.Context!);

            var cb = _current.GetCurrent<AstCodeBlock>();
            cb.AddLine(br);

            _current.SetCurrent(br);
            branch.VisitChildren(this);
            _current.RevertCurrent();
        }

        public override void VisitBranchExpression(AstBranchExpression branch)
        {
            var br = new AstBranchExpression(branch.Context!, branch.BranchKind);

            var cb = _current.GetCurrent<AstCodeBlock>();
            cb.AddLine(br);

            _current.SetCurrent(br);
            branch.VisitChildren(this);
            _current.RevertCurrent();
        }
    }
}
