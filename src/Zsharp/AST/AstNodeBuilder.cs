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

        public object? VisitChildrenExcept(ParserRuleContext node, ParserRuleContext except)
        {
            var result = DefaultResult;
            for (int i = 0; i < node.children.Count; i++)
            {
                var child = node.children[i];
                if (Object.ReferenceEquals(child, except))
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
            if (nextResult == null)
            {
                return aggregate;
            }
            return nextResult;
        }

        // use as generic error handler
        protected override bool ShouldVisitNextChild(IRuleNode node, object? currentResult)
        {
            if (node is ParserRuleContext context &&
                context.exception != null)
            {
                _builderContext.CompilerContext.SyntaxError(context);
                // usually pointless to continue
                return false;
            }
            return true;
        }

        public override object? VisitFile(FileContext context)
        {
            var file = new AstFile(_namespace, _builderContext.CompilerContext.IntrinsicSymbols, context);

            _builderContext.SetCurrent(file);
            _builderContext.SetCurrent(file.CodeBlock);
            _ = base.VisitChildren(context);
            _builderContext.RevertCurrent();
            _builderContext.RevertCurrent();

            return file;
        }

        public override object? VisitStatement_module(Statement_moduleContext context)
        {
            _builderContext.CompilerContext.Modules.AddModule(context);
            return base.VisitChildren(context);
        }

        public override object? VisitStatement_import(Statement_importContext context)
        {
            var dotName = AstDotName.FromText(context.module_name().GetText());
            var alias = context.alias_module()?.GetText();

            // if alias then last part of dot name is symbol.
            var moduleName = String.IsNullOrEmpty(alias) ? dotName.ToString() : dotName.ModuleName;
            var module = _builderContext.CompilerContext.Modules.Import(moduleName);
            if (module == null)
            {
                _builderContext.CompilerContext.AddError(context,
                        $"Module '{moduleName}' was not found in an external Assembly.");
                return null;
            }

            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();
            if (!String.IsNullOrEmpty(alias))
            {
                // lookup the symbol in the external module
                var entry = module.Symbols.FindEntry(dotName.Symbol, AstSymbolKind.Unknown);
                if (entry == null)
                {
                    _builderContext.CompilerContext.AddError(module, context,
                        $"Symbol '{dotName.Symbol}' was not found in Module '{module.Name}'.");
                    return null;
                }

                if (entry.HasOverloads)
                    entry = symbols.Symbols.AddSymbol(dotName.Symbol, entry.SymbolKind, entry.Overloads.ToArray());
                else
                    entry = symbols.Symbols.AddSymbol(dotName.Symbol, entry.SymbolKind, entry.Definition);

                entry.SymbolLocality = AstSymbolLocality.Imported;
                entry.AddAlias(alias);
            }
            else
            {
                var entry = symbols.AddSymbol(module.Name, AstSymbolKind.Module, module);
                entry.SymbolLocality = AstSymbolLocality.Imported;
            }
            return null;
        }

        public override object? VisitStatement_export(Statement_exportContext context)
        {
            var module = _builderContext.GetCurrent<AstModulePublic>();
            module.AddExport(context);

            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbols.Symbols.AddSymbol(context.identifier_func().GetText(), AstSymbolKind.NotSet, null);
            entry.SymbolLocality = AstSymbolLocality.Exported;

            return null;
        }

        public override object? VisitCodeblock(CodeblockContext context)
        {
            var stSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            var symbols = stSite.Symbols;
            string scopeName = symbols.Namespace;

            var cbSite = _builderContext.GetCurrent<IAstCodeBlockSite>();
            var parent = cbSite as AstFunctionDefinition;
            if (parent?.Identifier != null)
            {
                scopeName = parent.Identifier.CanonicalName;
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
            var file = _builderContext.GetCurrent<AstFile>();
            var function = new AstFunctionDefinitionImpl(context);

            file.AddFunction(function);
            _builderContext.SetCurrent(function);

            // process identifier first (needed for symbol)
            var identifier = context.identifier_func();
            VisitIdentifier_func(identifier);
            Ast.Guard(function.Identifier, "Function Identifier is not set.");

            var symbolTable = _builderContext.GetCurrent<IAstSymbolTableSite>();
            function.CreateSymbols(symbolTable.Symbols);

            if (context.Parent is Statement_export_inlineContext)
            {
                function.Symbol!.SymbolLocality = AstSymbolLocality.Exported;
            }

            _ = VisitChildrenExcept(context, identifier);
            _builderContext.RevertCurrent();
            return function;
        }

        public override object? VisitFunction_parameter(Function_parameterContext context)
        {
            var parameter = new AstFunctionParameterDefinition(context);
            var function = _builderContext.GetCurrent<AstFunctionDefinitionImpl>();
            function.TryAddParameter(parameter);

            _builderContext.SetCurrent(parameter);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            return parameter;
        }

        public override object? VisitFunction_parameter_self(Function_parameter_selfContext context)
        {
            var function = _builderContext.GetCurrent<AstFunctionDefinitionImpl>();
            var parameter = new AstFunctionParameterDefinition(context);
            parameter.SetIdentifier(AstIdentifierIntrinsic.Self);
            function.TryAddParameter(parameter);

            _builderContext.SetCurrent(parameter);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();
            return parameter;
        }

        public override object? VisitFunction_call(Function_callContext context)
        {
            _builderContext.CheckIndent(context, context.indent());

            var function = CreateFunctionReference(context);
            var codeBlock = _builderContext.GetCodeBlock();
            codeBlock!.AddItem(function);
            return function;
        }

        protected AstFunctionReference CreateFunctionReference(Function_callContext context)
        {
            var function = new AstFunctionReference(context);
            _builderContext.SetCurrent(function);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();
            function.CreateSymbols(symbols.Symbols);
            return function;
        }

        public override object? VisitFunction_param_use(Function_param_useContext context)
        {
            var parameter = new AstFunctionParameterReference(context);
            var function = _builderContext.GetCurrent<AstFunctionReference>();
            function.TryAddParameter(parameter);

            _builderContext.SetCurrent(parameter);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            return parameter;
        }

        //
        // Variable
        //

        public override object? VisitVariable_def_typed(Variable_def_typedContext context)
        {
            var variable = new AstVariableDefinition(context);
            var codeBlock = _builderContext.GetCodeBlock();

            codeBlock!.AddItem(variable);

            _builderContext.SetCurrent(variable);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var symbols = _builderContext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(variable);

            return variable;
        }

        public override object? VisitVariable_def(Variable_defContext context)
        {
            var indent = _builderContext.CheckIndent(context, context.indent());
            var codeBlock = _builderContext.GetCodeBlock(indent);

            _builderContext.SetCurrent(codeBlock);
            var results = VisitChildren(context);
            _builderContext.RevertCurrent();
            return results;
        }

        public override object? VisitVariable_assign_value(Variable_assign_valueContext context)
        {
            var assign = new AstAssignment(context);
            AstVariable variable = context.type_ref_use() == null
                ? new AstVariableReference(context)
                : new AstVariableDefinition(context);

            VisitVariableAssign(context, assign, variable);

            return assign;
        }

        public override object? VisitVariable_assign_struct(Variable_assign_structContext context)
        {
            var assign = new AstAssignment(context);
            AstVariable variable = new AstVariableReference(context);

            VisitVariableAssign(context, assign, variable);

            return assign;
        }

        private void VisitVariableAssign(ParserRuleContext context, AstAssignment assign, AstVariable variable)
        {
            var codeBlock = _builderContext.GetCodeBlock();

            codeBlock.AddItem(assign);
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

        public override object? VisitStatement_if(Statement_ifContext context)
        {
            var indent = _builderContext.CheckIndent(context, context.indent());
            var codeBlock = _builderContext.GetCodeBlock(indent);

            var branch = new AstBranchConditional(context);
            codeBlock.AddItem(branch);

            _builderContext.SetCurrent(branch);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();
            return branch;
        }

        public override object? VisitStatement_else(Statement_elseContext context)
        {
            var indent = _builderContext.CheckIndent(context, context.indent());
            var branch = _builderContext.GetCurrent<AstBranchConditional>();
            Ast.Guard(indent == branch.Indent, "Indentation mismatch CodeBlock <=> Branch");

            var subBr = new AstBranchConditional(context);
            branch.AddSubBranch(subBr);

            _builderContext.SetCurrent(subBr);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();
            return subBr;
        }

        public override object? VisitStatement_elseif(Statement_elseifContext context)
        {
            var indent = _builderContext.CheckIndent(context, context.indent());
            var branch = _builderContext.GetCurrent<AstBranchConditional>();
            Ast.Guard(indent == branch.Indent, "Indentation mismatch CodeBlock <=> Branch");

            var subBr = new AstBranchConditional(context);
            branch.AddSubBranch(subBr);

            _builderContext.SetCurrent(subBr);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();
            return subBr;
        }

        public override object? VisitStatement_return(Statement_returnContext context)
        {
            var indent = _builderContext.CheckIndent(context, context.indent());
            var codeBlock = _builderContext.GetCodeBlock(indent);

            var branch = new AstBranchExpression(context);
            codeBlock.AddItem(branch);

            _builderContext.SetCurrent(branch);
            _ = base.VisitChildren(context);
            _builderContext.RevertCurrent();
            return branch;
        }

        public override object? VisitStatement_break(Statement_breakContext context)
        {
            var indent = _builderContext.CheckIndent(context, context.indent());
            var codeBlock = _builderContext.GetCodeBlock(indent);

            var branch = new AstBranch(context);
            codeBlock.AddItem(branch);
            return branch;
        }

        public override object? VisitStatement_continue(Statement_continueContext context)
        {
            var indent = _builderContext.CheckIndent(context, context.indent());
            var codeBlock = _builderContext.GetCodeBlock(indent);

            var branch = new AstBranch(context);
            codeBlock.AddItem(branch);
            return branch;
        }

        //
        // Identifier
        //

        public override object? VisitIdentifier_type(Identifier_typeContext context)
        {
            _builderContext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_var(Identifier_varContext context)
        {
            _builderContext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_param(Identifier_paramContext context)
        {
            _builderContext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_func(Identifier_funcContext context)
        {
            _builderContext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_field(Identifier_fieldContext context)
        {
            _builderContext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_enumoption(Identifier_enumoptionContext context)
        {
            _builderContext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_template_param(Identifier_template_paramContext context)
        {
            _builderContext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        //
        // Expression
        //

        public override object? VisitExpression_value(Expression_valueContext context)
        {
            return CreateExpression(builder => builder.Build(context));
        }

        public override object? VisitExpression_logic(Expression_logicContext context)
        {
            return CreateExpression(builder => builder.Build(context));
        }

        public override object? VisitComptime_expression_value(Comptime_expression_valueContext context)
        {
            return CreateExpression(builder => builder.Build(context));
        }

        private AstExpression? CreateExpression(Func<AstExpressionBuilder, AstExpression?> buildFn)
        {
            var builder = new AstExpressionBuilder(_builderContext, _namespace);
            var expr = buildFn(builder);
            if (expr != null)
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
            var typeDef = new AstTypeDefinitionEnum(context, symbolsSite.Symbols);

            var codeBlock = _builderContext.GetCodeBlock();
            codeBlock.AddItem(typeDef);

            _builderContext.SetCurrent(typeDef);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            if (typeDef.BaseType == null)
            {
                var typeRef = AstTypeReference.Create(AstTypeDefinitionIntrinsic.I32);
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
                if (field.Expression == null)
                {
                    field.SetExpression(AstExpressionBuilder.CreateLiteral(value));
                }
                else
                {
                    value = (int)field.Expression.RHS.LiteralNumeric.Value;
                }
                value++;
            }

            return typeDef;
        }

        public override object? VisitEnum_option_def(Enum_option_defContext context)
        {
            var fieldDef = new AstTypeDefinitionEnumOption(context);

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

        public override object? VisitEnum_base_type(Enum_base_typeContext context)
        {
            var typeRef = new AstTypeReference(context);

            AstIdentifier? identifier = null;

            // .NET/C# does only supports number based enums
            if (context.STR() != null)
                identifier = AstIdentifierIntrinsic.Str;
            if (context.F64() != null)
                identifier = AstIdentifierIntrinsic.F64;
            if (context.F32() != null)
                identifier = AstIdentifierIntrinsic.F32;

            if (context.I8() != null)
                identifier = AstIdentifierIntrinsic.I8;
            if (context.I16() != null)
                identifier = AstIdentifierIntrinsic.I16;
            if (context.I64() != null)
                identifier = AstIdentifierIntrinsic.I64;
            if (context.I32() != null)
                identifier = AstIdentifierIntrinsic.I32;
            if (context.U8() != null)
                identifier = AstIdentifierIntrinsic.U8;
            if (context.U16() != null)
                identifier = AstIdentifierIntrinsic.U16;
            if (context.U64() != null)
                identifier = AstIdentifierIntrinsic.U64;
            if (context.U32() != null)
                identifier = AstIdentifierIntrinsic.U32;

            if (identifier == null)
            {
                _builderContext.CompilerContext.InvalidEnumBaseType(typeRef);
                return null;
            }

            _builderContext.SetCurrent(typeRef);
            _builderContext.AddIdentifier(identifier);

            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var trSite = _builderContext.GetCurrent<IAstTypeReferenceSite>();
            trSite.SetTypeReference(typeRef);

            var symbolsSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbolsSite.Symbols.Find(typeRef);
            if (entry != null)
            {
                entry.AddNode(typeRef);
                typeRef.SetSymbol(entry);
            }
            else
            {
                symbolsSite.Symbols.Add(typeRef);
            }
            return typeRef;
        }

        public override object? VisitStruct_def(Struct_defContext context)
        {
            var symbolsSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            var typeDef = new AstTypeDefinitionStruct(context, symbolsSite.Symbols);

            var codeBlock = _builderContext.GetCodeBlock();
            codeBlock.AddItem(typeDef);

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
            var fieldDef = new AstTypeDefinitionStructField(context);

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
            var field = new AstTypeFieldInitialization(context);

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
            var templateParam = new AstTemplateParameterDefinition(context);

            _builderContext.SetCurrent(templateParam);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var template = _builderContext.GetCurrent<IAstTemplateSite>();
            template.AddTemplateParameter(templateParam);

            return templateParam;
        }

        public override object? VisitTemplate_param_use(Template_param_useContext context)
        {
            var templateParam = new AstTemplateParameterReference(context);

            _builderContext.SetCurrent(templateParam);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var template = _builderContext.GetCurrent<IAstTemplateSite>();
            template.AddTemplateParameter(templateParam);

            return templateParam;
        }

        public override object? VisitType_ref(Type_refContext context)
        {
            var typeRef = new AstTypeReference(context);

            _builderContext.SetCurrent(typeRef);
            _ = VisitChildren(context);
            _builderContext.RevertCurrent();

            var template = _builderContext.TryGetCurrent<IAstTemplateSite>();
            if (template != null)
                typeRef.IsTemplateParameter = template.Parameters.Any(
                    p => p.Identifier?.CanonicalName == typeRef.Identifier?.CanonicalName);

            var trSite = _builderContext.GetCurrent<IAstTypeReferenceSite>();
            trSite.SetTypeReference(typeRef);

            var symbolsSite = _builderContext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbolsSite.Symbols.Find(typeRef);
            if (entry != null)
            {
                entry.AddNode(typeRef);
                typeRef.SetSymbol(entry);
            }
            else
            {
                symbolsSite.Symbols.Add(typeRef);
            }

            return typeRef;
        }

        public override object? VisitKnown_types(Known_typesContext context)
        {
            AstIdentifier? identifier = null;

            if (context.BOOL() != null)
                identifier = AstIdentifierIntrinsic.Bool;
            if (context.STR() != null)
                identifier = AstIdentifierIntrinsic.Str;
            if (context.F64() != null)
                identifier = AstIdentifierIntrinsic.F64;
            if (context.F32() != null)
                identifier = AstIdentifierIntrinsic.F32;
            if (context.I8() != null)
                identifier = AstIdentifierIntrinsic.I8;
            if (context.I16() != null)
                identifier = AstIdentifierIntrinsic.I16;
            if (context.I64() != null)
                identifier = AstIdentifierIntrinsic.I64;
            if (context.I32() != null)
                identifier = AstIdentifierIntrinsic.I32;
            if (context.U8() != null)
                identifier = AstIdentifierIntrinsic.U8;
            if (context.U16() != null)
                identifier = AstIdentifierIntrinsic.U16;
            if (context.U64() != null)
                identifier = AstIdentifierIntrinsic.U64;
            if (context.U32() != null)
                identifier = AstIdentifierIntrinsic.U32;

            if (identifier != null)
            {
                _builderContext.AddIdentifier(identifier);
            }

            return identifier;
        }
    }
}