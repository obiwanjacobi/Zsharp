using System;
using System.Collections.Generic;
using System.Linq;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstNodeCloner : AstVisitor
    {
        private readonly AstBuilderContext _context;
        private readonly Dictionary<string, string> _typeMap
            = new Dictionary<string, string>();

        public AstNodeCloner(CompilerContext context, uint indent = 0)
        {
            _context = new AstBuilderContext(context, indent);
        }

        public void Clone(AstFunctionReference functionRef, AstFunctionDefinition functionDef, AstTemplateInstanceFunction templateFunction)
        {
            if (functionDef is AstFunctionDefinitionImpl functionDefImpl)
            {
                CreateTypeMap(functionRef, functionDefImpl);

                _context.SetCurrent(templateFunction);
                VisitChildren(functionDefImpl);
                _context.RevertCurrent();

                templateFunction.SetIdentifier(functionRef.Identifier);

                // use template def itself to access symbol-tree
                var symbols = (IAstSymbolTableSite)functionDef;
                templateFunction.CreateSymbols(symbols.Symbols);
            }
            else if (functionDef.IsIntrinsic)
            {
                throw new NotImplementedException(
                    "Intrinsic Function not implemented yet.");
            }
            else
            {
                throw new NotImplementedException(
                    "[Interop] .NET Generics not implemented yet.");
            }
        }

        private void CreateTypeMap(AstFunctionReference functionRef, AstFunctionDefinitionImpl functionDef)
        {
            var defTemplateParams = functionDef.TemplateParameters.ToArray();
            var refTemplateParams = functionRef.TemplateParameters.ToArray();

            Ast.Guard(refTemplateParams.Length == defTemplateParams.Length,
                "Inconsistent number of template parameters between definition and instantiation.");

            for (int i = 0; i < functionDef.TemplateParameters.Count(); i++)
            {
                var defParam = defTemplateParams[i];
                var refParam = refTemplateParams[i];

                _typeMap.Add(
                    defParam.Identifier.CanonicalName,
                    refParam.TypeReference.Identifier.CanonicalName
                );
            }
        }

        public T? Clone<T>(T node) where T : AstNode, IAstCodeBlockItem
        {
            var cb = new AstCodeBlock("Test", _context.CompilerContext.IntrinsicSymbols);
            _context.SetCurrent(cb);
            Visit(node);
            _context.RevertCurrent();

            var result = cb.ItemAt<T>(0);
            result?.Orphan();

            return result;
        }

        //---------------------------------------------------------------------

        public override void VisitModulePublic(AstModulePublic module)
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
                codeBlock.Symbols.Name, codeBlock.Symbols.ParentTable, codeBlock.Context);

            var site = _context.GetCurrent<IAstCodeBlockSite>();
            site.SetCodeBlock(cb);

            _context.SetCurrent(cb);
            VisitChildren(codeBlock);
            _context.RevertCurrent();
        }

        public override void VisitFunctionDefinition(AstFunctionDefinition function)
        {
            if (function is AstFunctionDefinitionImpl functionDef)
            {
                var fnDef = new AstFunctionDefinitionImpl((Function_defContext)functionDef.Context!);
                fnDef.SetIdentifier(functionDef.Identifier);

                var codeBlock = _context.GetCurrent<AstCodeBlock>();
                codeBlock.AddItem(fnDef);

                _context.SetCurrent(fnDef);
                VisitChildren(functionDef);
                _context.RevertCurrent();

                var symbols = _context.GetCurrent<IAstSymbolTableSite>();
                fnDef.CreateSymbols(symbols.Symbols);
            }
        }

        public override void VisitFunctionParameterDefinition(AstFunctionParameterDefinition parameter)
        {
            var paramDef = parameter.Context switch
            {
                Function_parameterContext ctx => new AstFunctionParameterDefinition(ctx),
                Function_parameter_selfContext ctx => new AstFunctionParameterDefinition(ctx),
                _ => new AstFunctionParameterDefinition(parameter.Identifier)
            };

            paramDef.SetIdentifier(parameter.Identifier);


            var fnDef = _context.GetCurrent<AstFunctionDefinition>();
            fnDef.AddParameter(paramDef);

            _context.SetCurrent(paramDef);
            VisitChildren(parameter);
            _context.RevertCurrent();

            // parameter symbols are registered with FunctionDefinition.CreateSymbols()
        }

        public override void VisitFunctionParameterReference(AstFunctionParameterReference parameter)
        {
            var paramRef = new AstFunctionParameterReference((Function_param_useContext)parameter.Context);
            // param ref usually has no Identifier
            paramRef.TrySetIdentifier(parameter.Identifier);

            var fnRef = _context.GetCurrent<AstFunctionReference>();
            fnRef.AddParameter(paramRef);

            _context.SetCurrent(paramRef);
            VisitChildren(parameter);
            _context.RevertCurrent();
        }

        public override void VisitFunctionReference(AstFunctionReference function)
            => CloneFunctionReference(function);

        private AstFunctionReference CloneFunctionReference(AstFunctionReference function)
        {
            var fnRef = new AstFunctionReference(function.Context);
            fnRef.SetIdentifier(function.Identifier);

            _context.SetCurrent(fnRef);
            VisitChildren(function);
            _context.RevertCurrent();

            return fnRef;
        }

        public override void VisitExpression(AstExpression expression)
            => CloneExpression(expression);

        private AstExpression CloneExpression(AstExpression expression)
        {
            var expr = new AstExpression(expression.Context)
            {
                Operator = expression.Operator
            };

            var site = _context.GetCurrent<IAstExpressionSite>();
            site.SetExpression(expr);

            _context.SetCurrent(expr);
            VisitChildren(expression);
            _context.RevertCurrent();

            return expr;
        }

        public override void VisitExpressionOperand(AstExpressionOperand operand)
        {
            AstNode child = null;

            if (operand.Expression != null)
                child = CloneExpression(operand.Expression);

            if (operand.FieldReference != null)
                child = CloneFieldReference(operand.FieldReference);

            if (operand.FunctionReference != null)
                child = CloneFunctionReference(operand.FunctionReference);

            if (operand.LiteralBoolean != null)
                child = CloneLiteralBoolean(operand.LiteralBoolean);

            if (operand.LiteralNumeric != null)
                child = CloneLiteralNumeric(operand.LiteralNumeric);

            if (operand.LiteralString != null)
                child = CloneLiteralString(operand.LiteralString);

            if (operand.VariableReference != null)
                child = CloneVariableReference(operand.VariableReference);

            var op = new AstExpressionOperand(child);
            op.SetTypeReference(CloneTypeReference(operand.TypeReference));

            var exp = _context.GetCurrent<AstExpression>();
            exp.Add(op);
        }

        private AstLiteralBoolean CloneLiteralBoolean(AstLiteralBoolean literal)
            => new AstLiteralBoolean((Literal_boolContext)literal.Context);

        private AstLiteralNumeric CloneLiteralNumeric(AstLiteralNumeric literal)
        {
            if (literal.Context == null)
                return new AstLiteralNumeric(literal.Value);

            return AstLiteralNumeric.Create((NumberContext)literal.Context);
        }

        private AstLiteralString CloneLiteralString(AstLiteralString literal)
            => new AstLiteralString((StringContext)literal.Context);

        public override void VisitAssignment(AstAssignment assign)
        {
            var a = new AstAssignment(assign.Context)
            {
                Indent = assign.Indent
            };

            var codeBlock = _context.GetCurrent<AstCodeBlock>();
            codeBlock.AddItem(a);

            _context.SetCurrent(a);

            var v = CloneVariable(assign.Variable);
            a.SetVariable(v);

            VisitExpression(assign.Expression);
            _context.RevertCurrent();
        }

        private AstVariable CloneVariable(AstVariable variable)
        {
            return variable switch
            {
                AstVariableDefinition varDef => CloneVariableDefinition(varDef),
                AstVariableReference varRef => CloneVariableReference(varRef),
                _ => throw new InvalidOperationException("AstVariable sub type not implemented.")
            };
        }

        public override void VisitVariableDefinition(AstVariableDefinition variable)
        {
            var varDef = CloneVariableDefinition(variable);

            var codeBlock = _context.GetCurrent<AstCodeBlock>();
            codeBlock.AddItem(varDef);
        }

        private AstVariableDefinition CloneVariableDefinition(AstVariableDefinition variable)
        {
            var varDef = new AstVariableDefinition(variable.Context)
            {
                Indent = variable.Indent
            };
            varDef.SetIdentifier(variable.Identifier);

            _context.SetCurrent(varDef);
            VisitChildren(variable);
            _context.RevertCurrent();

            var symbols = _context.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(varDef);

            return varDef;
        }

        public override void VisitVariableReference(AstVariableReference variable)
            => CloneVariableReference(variable);

        private AstVariableReference CloneVariableReference(AstVariableReference variable)
        {
            var varRef = new AstVariableReference(variable.Context);
            varRef.SetIdentifier(variable.Identifier);

            _context.SetCurrent(varRef);
            VisitChildren(variable);
            _context.RevertCurrent();

            var symbols = _context.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(varRef);

            return varRef;
        }

        public override void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
        {
            VisitChildren(enumType);
            throw new NotImplementedException();
        }

        public override void VisitTypeDefinitionEnumOption(AstTypeDefinitionEnumOption enumOption)
        {
            VisitChildren(enumOption);
            throw new NotImplementedException();
        }

        public override void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
        {
            VisitChildren(structType);
            throw new NotImplementedException();
        }

        public override void VisitTypeDefinitionStructField(AstTypeDefinitionStructField structField)
        {
            VisitChildren(structField);
            throw new NotImplementedException();
        }

        public override void VisitTypeFieldDefinition(AstTypeFieldDefinition field)
        {
            VisitChildren(field);
            throw new NotImplementedException();
        }

        public override void VisitTypeFieldInitialization(AstTypeFieldInitialization field)
        {
            VisitChildren(field);
            throw new NotImplementedException();
        }

        public override void VisitTypeFieldReferenceEnumOption(AstTypeFieldReferenceEnumOption enumOption)
        {
            var fldRef = new AstTypeFieldReferenceEnumOption((Enum_option_useContext)enumOption.Context);

            _context.SetCurrent(fldRef);
            VisitChildren(enumOption);
            _context.RevertCurrent();
        }

        public override void VisitTypeFieldReferenceStructField(AstTypeFieldReferenceStructField structField)
        {
            var fldRef = new AstTypeFieldReferenceStructField((Variable_field_refContext)structField.Context);

            _context.SetCurrent(fldRef);
            VisitChildren(structField);
            _context.RevertCurrent();
        }

        private AstTypeFieldReference CloneFieldReference(AstTypeFieldReference fieldReference)
        {
            return fieldReference switch
            {
                AstTypeFieldReferenceStructField => new AstTypeFieldReferenceStructField((Variable_field_refContext)fieldReference.Context),
                AstTypeFieldReferenceEnumOption => new AstTypeFieldReferenceEnumOption((Enum_option_useContext)fieldReference.Context),
                _ => throw new InvalidOperationException(
                    $"AstNodeCloner does not implement this FieldReference Type: {fieldReference.GetType().Name}")
            };
        }

        public override void VisitTypeReference(AstTypeReference type)
            => CloneTypeReference(type);

        private AstTypeReference CloneTypeReference(AstTypeReference type)
        {
            AstTypeReference typeRef;

            if (_typeMap.TryGetValue(type.Identifier.CanonicalName, out string newTypeName))
            {
                var symbols = _context.GetCurrent<IAstSymbolTableSite>();
                var newSymbol = symbols.Symbols.FindEntry(newTypeName, AstSymbolKind.Type);

                typeRef = AstTypeReference.From(newSymbol.DefinitionAs<AstTypeDefinition>());
            }
            else
                typeRef = type.MakeProxy();

            var site = _context.GetCurrent<IAstTypeReferenceSite>();
            site.SetTypeReference(typeRef);

            return typeRef;
        }

        public override void VisitBranch(AstBranch branch)
        {
            AstBranch br;

            if (branch.BranchType == AstBranchType.ExitIteration)
                br = new AstBranch((Statement_continueContext)branch.Context);
            else if (branch.BranchType == AstBranchType.ExitLoop)
                br = new AstBranch((Statement_breakContext)branch.Context);
            else
                throw new InvalidOperationException("Unknown Branch Type.");

            var cb = _context.GetCurrent<AstCodeBlock>();
            cb.AddItem(br);

            _context.SetCurrent(br);
            VisitChildren(branch);
            _context.RevertCurrent();
        }

        public override void VisitBranchConditional(AstBranchConditional branch)
        {
            var br = new AstBranchConditional(branch.Context);

            var cb = _context.GetCurrent<AstCodeBlock>();
            cb.AddItem(br);

            _context.SetCurrent(br);
            VisitChildren(branch);
            _context.RevertCurrent();
        }

        public override void VisitBranchExpression(AstBranchExpression branch)
        {
            var br = new AstBranchExpression((Statement_returnContext)branch.Context);

            var cb = _context.GetCurrent<AstCodeBlock>();
            cb.AddItem(br);

            _context.SetCurrent(br);
            VisitChildren(branch);
            _context.RevertCurrent();
        }
    }
}
