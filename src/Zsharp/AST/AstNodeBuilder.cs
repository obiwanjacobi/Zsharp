using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using Zsharp.Parser;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstNodeBuilder : ZsharpBaseVisitor<object?>
    {
        private readonly AstBuilderContext _builderContext;
        private readonly string _namespace;

        public AstNodeBuilder(AstBuilderContext context, string ns)
        {
            _builderContext = context;
            _namespace = ns;
        }

        protected AstBuilderContext BuilderContext => _builderContext;

        public IEnumerable<AstMessage> Errors => _builderContext.Errors;

        public bool HasErrors => _builderContext.HasErrors;

        public object? VisitChildrenExcept(ParserRuleContext node, params ParserRuleContext?[] except)
        {
            var result = DefaultResult;
            for (int i = 0; i < node.children.Count; i++)
            {
                var child = node.children[i];
                if (except.Contains(child))
                    continue;

                if (!ShouldVisitNextChild(node, result))
                {
                    break;
                }

                var childResult = child.Accept(this);
                result = AggregateResult(result, childResult);
            }

            return result;
        }

        protected override object? AggregateResult(object? aggregate, object? nextResult)
        {
            if (nextResult is null)
            {
                return aggregate;
            }
            return nextResult;
        }

        // use as generic error handler
        protected override bool ShouldVisitNextChild(IRuleNode node, object? currentResult)
        {
            if (node is ParserRuleContext context &&
                context.exception is not null)
            {
                _builderContext.CompilerContext.SyntaxError(context);
                // usually pointless to continue
                return false;
            }
            return true;
        }

        public override object? VisitFile(FileContext context)
        {
            var file = New.AstFile(_namespace, _builderContext.CompilerContext.IntrinsicSymbols, context);

            _builderContext.SetCurrent(file);
            _builderContext.SetCurrent(file.CodeBlock!);
            _ = base.VisitChildren(context);
            _builderContext.RevertCurrent();
            _builderContext.RevertCurrent();

            return file;
        }

        public override object? VisitStatement_module(Statement_moduleContext context)
        {
            var module = _builderContext.CompilerContext.Modules.AddModule(context);

            var symbolTable = _builderContext.GetCurrent<IAstSymbolTableSite>();
            symbolTable.Symbols.Add(module);

            return base.VisitChildren(context);
        }

        public override object? VisitStatement_import(Statement_importContext context)
        {
            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();

            var importNamespace = context.module_namespace()?.GetText();
            if (!String.IsNullOrEmpty(importNamespace))
            {
                Ast.Guard(importNamespace.EndsWith(".*"), "import module namespace does not end in '.*'");
                var moduleNamespace = AstSymbolName.ToCanonical(importNamespace[..^2]);
                var modules = _builderContext.CompilerContext.Modules.ImportNamespace(moduleNamespace);
                if (!modules.Any())
                {
                    _builderContext.CompilerContext.AddError(context,
                        $"Module namespace '{importNamespace}' was not found in any external Assembly.");
                    return null;
                }

                foreach (var mod in modules)
                {
                    var modSymbol = symbols.Symbols.Add(mod);
                    modSymbol.SymbolLocality = AstSymbolLocality.Imported;
                }
                return null;
            }

            var symbolName = AstSymbolName.Parse(context.module_name().GetText());
            var alias = context.alias_module()?.GetText();
            var hasAlias = !String.IsNullOrEmpty(alias);

            // if alias then last part of dot name is symbol.
            var moduleName = hasAlias ? symbolName.CanonicalName.Namespace : symbolName.CanonicalName.FullName;
            var module = _builderContext.CompilerContext.Modules.Import(moduleName);
            if (module is null)
            {
                _builderContext.CompilerContext.AddError(context,
                        $"Module '{moduleName}' was not found in an external Assembly.");
                return null;
            }

            if (hasAlias)
            {
                module.AddAlias(symbolName.CanonicalName, alias!);
            }

            var symbol = symbols.Symbols.Add(module);
            symbol.SymbolLocality = AstSymbolLocality.Imported;
            return null;
        }

        public override object? VisitStatement_export(Statement_exportContext context)
        {
            var module = _builderContext.GetCurrent<AstModuleImpl>();
            module.AddExport(context);

            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();
            var canonicalName = AstSymbolName.ToCanonical(context.identifier_func().GetText());
            var symbol = symbols.Symbols.AddSymbol(canonicalName, AstSymbolKind.NotSet);
            symbol.SymbolLocality = AstSymbolLocality.Exported;

            return null;
        }

        public override object? VisitCodeblock(CodeblockContext context)
        {
            var stSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            var symbols = stSite.Symbols;
            string scopeName = symbols.Namespace;

            var cbSite = _builderContext.GetCurrent<IAstCodeBlockSite>();
            var parent = cbSite as AstFunctionDefinition;
            if (parent?.HasIdentifier ?? false)
            {
                scopeName = parent.Identifier.SymbolName.CanonicalName.FullName;
            }

            var codeBlock = new AstCodeBlock(scopeName, symbols, context);
            cbSite.SetCodeBlock(codeBlock);

            _builderContext.SetCurrent(codeBlock);
            _ = base.VisitChildren(context);
            _builderContext.RevertCurrent();
            return codeBlock;
        }

        //
        // Function
        //

        public override object? VisitFunction_def(Function_defContext context)
        {
            var codeBlock = _builderContext.GetCodeBlock(context);
            var function = New.AstFunctionDefinitionImpl(context);

            codeBlock.AddLine(function);
            _builderContext.SetCurrent(function.FunctionType);
            _builderContext.SetCurrent(function);

            // process identifier first (needed for symbol)
            var identifier = context.identifier_func();
            VisitIdentifier_func(identifier);
            Ast.Guard(function.Identifier, "Function Identifier is not set.");

            // template params also determines identifier
            var templateParams = context.template_param_list();
            if (templateParams is not null)
            {
                VisitTemplate_param_list(templateParams);
            }

            _ = VisitChildrenExcept(context, identifier, templateParams);

            var functionTable = _builderContext.GetCurrent<IAstSymbolTableSite>();
            function.CreateSymbols(functionTable.Symbols, codeBlock.Symbols);

            if (context.Parent is Statement_export_inlineContext)
                function.Symbol!.SymbolLocality = AstSymbolLocality.Exported;

            if (!function.FunctionType.HasTypeReference)
            {
                var typeRef = new AstTypeReferenceType(AstIdentifierIntrinsic.Void);
                function.FunctionType.SetTypeReference(typeRef);
                codeBlock.Symbols.Add(typeRef);
            }

            _builderContext.RevertCurrent();
            _builderContext.RevertCurrent();
            return function;
        }

        public override object? VisitFunction_parameter(Function_parameterContext context)
        {
            var parameter = New.AstFunctionParameterDefinition(context);

            VisitFunctionParameter(parameter);

            return parameter;
        }

        public override object? VisitFunction_parameter_self(Function_parameter_selfContext context)
        {
            var parameter = New.AstFunctionParameterDefinition(context);
            parameter.SetIdentifier(AstIdentifierIntrinsic.Self);

            VisitFunctionParameter(parameter);

            return parameter;
        }

        private void VisitFunctionParameter(AstFunctionParameterDefinition parameter)
        {
            var function = _builderContext.GetCurrent<AstFunctionDefinitionImpl>();
            function.FunctionType.AddParameter(parameter);

            _builderContext.SetCurrent(parameter);
            _ = VisitChildren(parameter.Context);
            _builderContext.RevertCurrent();
        }

        public override object? VisitFunction_call(Function_callContext context)
        {
            var function = CreateFunctionReference(context);
            var codeBlock = _builderContext.GetCodeBlock(context);
            codeBlock!.AddLine(function);
            return function;
        }

        public override object? VisitFunction_call_self(Function_call_selfContext context)
        {
            var fnRef = (AstFunctionReference)VisitFunction_call(context.function_call())!;
            var varRef = (AstVariableReference)VisitVariable_ref(context.variable_ref())!;

            // make variable into a parameter ref
            var expression = new AstExpression(new AstExpressionOperand(varRef));
            var param = new AstFunctionParameterReference(expression);
            param.SetIdentifier(AstIdentifierIntrinsic.Self);
            fnRef.FunctionType.AddParameter(param);

            return fnRef;
        }

        protected AstFunctionReference CreateFunctionReference(Function_callContext context)
        {
            var function = New.AstFunctionReference(context);
            _builderContext.SetCurrent(function.FunctionType);
            _builderContext.SetCurrent(function);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();
            _builderContext.RevertCurrent();

            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();
            function.CreateSymbols(symbols.Symbols);

            return function;
        }

        public override object? VisitFunction_param_use(Function_param_useContext context)
        {
            var parameter = New.AstFunctionParameterReference(context);
            var function = _builderContext.GetCurrent<AstFunctionReference>();
            function.FunctionType.AddParameter(parameter);

            _builderContext.SetCurrent(parameter);
            var node = (AstNode?)VisitChildren(context);
            _builderContext.RevertCurrent();

            if (!parameter.HasExpression)
            {
                parameter.SetExpression(node switch
                {
                    AstExpressionOperand op => new AstExpression(op),
                    AstExpression expression => expression,
                    _ => throw new InternalErrorException("Parameter reference not an Expression or Operand.")
                });
            }

            return parameter;
        }

        //
        // Variable
        //

        public override object? VisitVariable_def_typed(Variable_def_typedContext context)
        {
            var variable = New.AstVariableDefinition(context);
            var codeBlock = _builderContext.GetCodeBlock(context);

            codeBlock!.AddLine(variable);

            _builderContext.SetCurrent(variable);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(variable);

            return variable;
        }

        public override object? VisitVariable_def(Variable_defContext context)
        {
            var codeBlock = _builderContext.GetCodeBlock(context);

            _builderContext.SetCurrent(codeBlock);
            var results = VisitChildren(context);
            _builderContext.RevertCurrent();
            return results;
        }

        public override object? VisitVariable_ref(Variable_refContext context)
        {
            var varRef = New.AstVariableReference(context);

            BuilderContext.SetCurrent(varRef);
            VisitChildren(context);
            BuilderContext.RevertCurrent();

            if (context.SELF() is not null)
                varRef.SetIdentifier(AstIdentifierIntrinsic.Self);

            var symbols = BuilderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(varRef);

            return varRef;
        }

        public override object? VisitVariable_assign_value(Variable_assign_valueContext context)
        {
            var assign = New.AstAssignment(context);
            AstVariable variable = context.type_ref_use() is null
                ? New.AstVariableReference(context)
                : New.AstVariableDefinition(context);

            VisitVariableAssign(context, assign, variable);

            return assign;
        }

        public override object? VisitVariable_assign_struct(Variable_assign_structContext context)
        {
            var assign = New.AstAssignment(context);
            AstVariable variable = New.AstVariableReference(context);

            VisitVariableAssign(context, assign, variable);

            return assign;
        }

        private void VisitVariableAssign(ParserRuleContext context, AstAssignment assign, AstVariable variable)
        {
            var codeBlock = _builderContext.GetCodeBlock(context);

            codeBlock.AddLine(assign);
            _builderContext.SetCurrent(assign);

            assign.SetVariable(variable);

            _builderContext.SetCurrent(variable);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();
            _builderContext.RevertCurrent();

            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(variable);
        }

        //
        // Flow
        //

        public override object? VisitStatement_loop_infinite(Statement_loop_infiniteContext context)
        {
            var branch = New.AstBranchExpression(context);
            BuildBranch(branch, context);
            return branch;
        }

        public override object? VisitStatement_loop_iteration(Statement_loop_iterationContext context)
        {
            var branch = New.AstBranchExpression(context);
            BuildBranch(branch, context);
            return branch;
        }

        public override object? VisitStatement_loop_while(Statement_loop_whileContext context)
        {
            var branch = New.AstBranchExpression(context);
            BuildBranch(branch, context);
            return branch;
        }

        public override object? VisitStatement_if(Statement_ifContext context)
        {
            var branch = New.AstBranchConditional(context);
            BuildBranch(branch, context);
            return branch;
        }

        public override object? VisitStatement_else(Statement_elseContext context)
        {
            var branch = New.AstBranchConditional(context);
            var indent = _builderContext.CheckIndent(context, context.indent());
            BuildSubBranch(branch, context, indent);
            return branch;
        }

        public override object? VisitStatement_elseif(Statement_elseifContext context)
        {
            var indent = _builderContext.CheckIndent(context, context.indent());
            var branch = New.AstBranchConditional(context);
            BuildSubBranch(branch, context, indent);
            return branch;
        }

        public override object? VisitStatement_return(Statement_returnContext context)
        {
            var branch = New.AstBranchExpression(context);
            BuildBranch(branch, context);
            return branch;
        }

        public override object? VisitStatement_break(Statement_breakContext context)
        {
            var codeBlock = _builderContext.GetCodeBlock(context);

            var branch = New.AstBranch(context);
            codeBlock.AddLine(branch);
            return branch;
        }

        public override object? VisitStatement_continue(Statement_continueContext context)
        {
            var codeBlock = _builderContext.GetCodeBlock(context);

            var branch = New.AstBranch(context);
            codeBlock.AddLine(branch);
            return branch;
        }

        private void BuildBranch(AstBranch branch, ParserRuleContext context)
        {
            var codeBlock = _builderContext.GetCodeBlock(context);
            codeBlock.AddLine(branch);

            _builderContext.SetCurrent(branch);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();
        }

        private void BuildSubBranch(AstBranchConditional subBranch, ParserRuleContext context, uint indent)
        {
            var branch = _builderContext.GetCurrent<AstBranchConditional>();
            Ast.Guard(indent == branch.Indent, "Indentation mismatch CodeBlock <=> Branch");

            branch.AddSubBranch(subBranch);

            _builderContext.SetCurrent(subBranch);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();
        }

        //
        // Identifier
        //

        public override object? VisitIdentifier_type(Identifier_typeContext context)
        {
            _builderContext.AddIdentifier(New.AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_var(Identifier_varContext context)
        {
            _builderContext.AddIdentifier(New.AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_param(Identifier_paramContext context)
        {
            _builderContext.AddIdentifier(New.AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_func(Identifier_funcContext context)
        {
            _builderContext.AddIdentifier(New.AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_field(Identifier_fieldContext context)
        {
            _builderContext.AddIdentifier(New.AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_enumoption(Identifier_enumoptionContext context)
        {
            _builderContext.AddIdentifier(New.AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_template_param(Identifier_template_paramContext context)
        {
            _builderContext.AddIdentifier(New.AstIdentifier(context));
            return null;
        }

        //
        // Expression
        //

        public override object? VisitExpression_value(Expression_valueContext context)
            => CreateExpression(context);

        public override object? VisitExpression_logic(Expression_logicContext context)
            => CreateExpression(context);

        public override object? VisitComptime_expression_value(Comptime_expression_valueContext context)
            => CreateExpression(context);

        public override object? VisitExpression_iteration(Expression_iterationContext context)
            => CreateExpression(context);

        private AstExpression? CreateExpression(ParserRuleContext context)
        {
            var expr = new AstExpressionBuilder(_builderContext, _namespace)
                .Build(context);

            if (expr is not null)
            {
                var site = _builderContext.GetCurrent<IAstExpressionSite>();
                site.SetExpression(expr);
                return expr;
            }

            return null;
        }

        //
        // Type
        //

        public override object? VisitEnum_def(Enum_defContext context)
        {
            var symbolsSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            var typeDef = New.AstTypeDefinitionEnum(context, symbolsSite.Symbols);

            var codeBlock = _builderContext.GetCodeBlock(context);
            codeBlock.AddLine(typeDef);

            _builderContext.SetCurrent(typeDef);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            if (!typeDef.HasBaseType)
            {
                var typeRef = new AstTypeReferenceType(AstIdentifierIntrinsic.I32);
                symbolsSite.Symbols.Add(typeRef);

                typeDef.SetBaseType(typeRef);
            }

            symbolsSite.Symbols.Add(typeDef);

            if (context.Parent is Statement_export_inlineContext)
            {
                typeDef.Symbol!.SymbolLocality = AstSymbolLocality.Exported;
            }

            int value = 0;
            foreach (var field in typeDef.Fields)
            {
                if (!field.HasExpression)
                {
                    field.SetExpression(AstExpressionBuilder.CreateLiteral(value));
                }
                else
                {
                    value = (int)field.Expression!.RHS!.LiteralNumeric!.Value;
                }
                value++;
            }

            return typeDef;
        }

        public override object? VisitEnum_option_def(Enum_option_defContext context)
        {
            var fieldDef = New.AstTypeDefinitionEnumOption(context);

            _builderContext.SetCurrent(fieldDef);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var typeDef = _builderContext.GetCurrent<AstTypeDefinitionEnum>();
            typeDef.AddField(fieldDef);

            var symbolsSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            symbolsSite.Symbols.Add(fieldDef);

            return fieldDef;
        }

        public override object? VisitEnum_option_def_listline(Enum_option_def_listlineContext context)
        {
            throw new NotImplementedException();
        }

        public override object? VisitStruct_def(Struct_defContext context)
        {
            var symbolsSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            var typeDef = New.AstTypeDefinitionStruct(context, symbolsSite.Symbols);

            var codeBlock = _builderContext.GetCodeBlock(context);
            codeBlock.AddLine(typeDef);

            _builderContext.SetCurrent(typeDef);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            symbolsSite.Symbols.Add(typeDef);

            if (context.Parent is Statement_export_inlineContext)
            {
                typeDef.Symbol!.SymbolLocality = AstSymbolLocality.Exported;
            }

            return typeDef;
        }

        public override object? VisitStruct_field_def(Struct_field_defContext context)
        {
            var fieldDef = New.AstTypeDefinitionStructField(context);

            _builderContext.SetCurrent(fieldDef);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var typeDef = _builderContext.GetCurrent<AstTypeDefinitionStruct>();
            typeDef.AddField(fieldDef);

            var symbolsSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            symbolsSite.Symbols.Add(fieldDef);

            return fieldDef;
        }

        public override object? VisitStruct_field_init(Struct_field_initContext context)
        {
            var field = New.AstTypeFieldInitialization(context);

            _builderContext.SetCurrent(field);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(field);

            var typeInit = _builderContext.GetCurrent<IAstTypeInitializeSite>();
            typeInit.AddFieldInit(field);

            return field;
        }

        public override object? VisitTemplate_param_any(Template_param_anyContext context)
        {
            if (context.COMPTIME() is not null)
            {
                var templateParam = New.AstTemplateParameterDefinition(context);

                _builderContext.SetCurrent(templateParam);
                _ = VisitChildren(context);
                _builderContext.RevertCurrent();

                var template = _builderContext.GetCurrent<IAstTemplateSite<AstTemplateParameterDefinition>>();
                template.AddTemplateParameter(templateParam);

                return templateParam;
            }

            var genericParam = New.AstGenericParameterDefinition(context);

            _builderContext.SetCurrent(genericParam);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var genericSite = _builderContext.GetCurrent<IAstGenericSite<AstGenericParameterDefinition>>();
            genericSite.AddGenericParameter(genericParam);

            return genericParam;
        }

        public override object? VisitTemplate_param_use(Template_param_useContext context)
        {
            var templateParam = New.AstTemplateParameterReference(context);

            _builderContext.SetCurrent(templateParam);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var template = _builderContext.GetCurrent<IAstTemplateSite<AstTemplateParameterReference>>();
            template.AddTemplateParameter(templateParam);

            return templateParam;
        }

        public override object? VisitType_ref(Type_refContext context)
        {
            var typeRef = New.AstTypeReferenceType(context);

            _builderContext.SetCurrent(typeRef);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var template = _builderContext.TryGetCurrent<IAstTemplateSite<AstTemplateParameterDefinition>>();
            if (template is not null)
                typeRef.IsTemplateParameter = template.TemplateParameters
                    .OfType<AstTemplateParameterDefinition>()
                    .Any(p => p.Identifier.SymbolName.CanonicalName.FullName == typeRef.Identifier.SymbolName.CanonicalName.FullName);

            var generic = _builderContext.TryGetCurrent<IAstGenericSite<AstGenericParameterDefinition>>();
            if (generic is not null)
                typeRef.IsGenericParameter = generic.GenericParameters
                    .OfType<AstGenericParameterDefinition>()
                    .Any(p => p.Identifier.SymbolName.CanonicalName.FullName == typeRef.Identifier.SymbolName.CanonicalName.FullName);

            var trSite = _builderContext.GetCurrent<IAstTypeReferenceSite>();
            trSite.SetTypeReference(typeRef);

            var symbolsSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            symbolsSite.Symbols.Add(typeRef);

            return typeRef;
        }
    }
}