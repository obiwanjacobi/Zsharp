using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
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
        private readonly AstBuilderContext _buildercontext;
        private readonly string _namespace;

        public AstNodeBuilder(AstBuilderContext context, string ns)
        {
            _buildercontext = context;
            _namespace = ns;
        }

        public IEnumerable<AstMessage> Errors => _buildercontext.Errors;

        public bool HasErrors => _buildercontext.HasErrors;

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
                _buildercontext.CompilerContext.AddError(context, AstMessage.SyntaxError);
                // usually pointless to continue
                return false;
            }
            return true;
        }

        public override object? VisitFile(FileContext context)
        {
            var file = new AstFile(_namespace, _buildercontext.CompilerContext.IntrinsicSymbols, context);

            _buildercontext.SetCurrent(file);
            base.VisitChildren(context);
            _buildercontext.RevertCurrent();

            return file;
        }

        public override object? VisitStatement_module([NotNull] Statement_moduleContext context)
        {
            _buildercontext.CompilerContext.Modules.AddModule(context);

            return base.VisitStatement_module(context);
        }

        public override object? VisitStatement_import(Statement_importContext context)
        {
            var dotName = new AstDotName(context.module_name().GetText());
            var alias = context.alias_module()?.GetText();

            // if alias then last part of dot name is symbol.
            var moduleName = String.IsNullOrEmpty(alias) ? dotName.ToString() : dotName.ModuleName;
            var module = _buildercontext.CompilerContext.Modules.Import(moduleName);
            if (module == null)
            {
                _buildercontext.CompilerContext.AddError(context,
                        $"Module '{moduleName}' was not found in an external Assembly.");
                return null;
            }

            var symbols = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            if (!String.IsNullOrEmpty(alias))
            {
                // lookup the symbol in the external module
                var entry = module.Symbols.FindEntry(dotName.Symbol);
                if (entry == null)
                {
                    _buildercontext.CompilerContext.AddError(module, context,
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
            var module = _buildercontext.GetCurrent<AstModulePublic>();
            module.AddExport(context);

            var symbols = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbols.Symbols.AddSymbol(context.identifier_func().GetText(), AstSymbolKind.NotSet, null);
            entry.SymbolLocality = AstSymbolLocality.Exported;

            return null;
        }

        public override object? VisitFunction_def(Function_defContext context)
        {
            var file = _buildercontext.GetCurrent<AstFile>();
            var function = new AstFunctionDefinitionImpl(context);

            file.AddFunction(function);
            _buildercontext.SetCurrent(function);

            // process identifier first (needed for symbol)
            var identifier = context.identifier_func();
            VisitIdentifier_func(identifier);
            Ast.Guard(function.Identifier, "Function Identifier is not set.");

            var symbolTable = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbolTable.Symbols.Add(function);

            if (context.Parent is Function_def_exportContext)
            {
                entry.SymbolLocality = AstSymbolLocality.Exported;
            }

            var any = VisitChildrenExcept(context, identifier);
            _buildercontext.RevertCurrent();

            if (context.function_return_type() == null)
            {
                var typeRef = AstTypeReference.Create(AstTypeDefinitionIntrinsic.Void);
                function.SetTypeReference(typeRef);
                symbolTable.Symbols.Add(typeRef);
            }
            return any;
        }

        public override object? VisitCodeblock(CodeblockContext context)
        {
            var stSite = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            var symbols = stSite.Symbols;
            string scopeName = symbols.Namespace;

            var cbSite = _buildercontext.GetCurrent<IAstCodeBlockSite>();
            var parent = cbSite as AstFunctionDefinition;
            if (parent?.Identifier != null)
            {
                scopeName = parent.Identifier.CanonicalName;
            }

            var codeBlock = new AstCodeBlock(scopeName, symbols, context);
            cbSite.SetCodeBlock(codeBlock);

            _buildercontext.SetCurrent(codeBlock);
            var any = base.VisitChildren(context);
            _buildercontext.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_if(Statement_ifContext context)
        {
            var indent = _buildercontext.CheckIndent(context, context.indent());
            var codeBlock = _buildercontext.GetCodeBlock(indent);

            var branch = new AstBranchConditional(context);
            codeBlock.AddItem(branch);

            _buildercontext.SetCurrent(branch);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_else(Statement_elseContext context)
        {
            var indent = _buildercontext.CheckIndent(context, context.indent());
            var branch = _buildercontext.GetCurrent<AstBranchConditional>();
            Ast.Guard(indent == branch.Indent, "Indentation mismatch CodeBlock <=> Branch");

            var subBr = new AstBranchConditional(context);
            branch.AddSubBranch(subBr);

            _buildercontext.SetCurrent(subBr);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_elseif(Statement_elseifContext context)
        {
            var indent = _buildercontext.CheckIndent(context, context.indent());
            var branch = _buildercontext.GetCurrent<AstBranchConditional>();
            Ast.Guard(indent == branch.Indent, "Indentation mismatch CodeBlock <=> Branch");

            var subBr = new AstBranchConditional(context);
            branch.AddSubBranch(subBr);

            _buildercontext.SetCurrent(subBr);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_return(Statement_returnContext context)
        {
            var indent = _buildercontext.CheckIndent(context, context.indent());
            var codeBlock = _buildercontext.GetCodeBlock(indent);

            var branch = new AstBranchExpression(context);
            codeBlock.AddItem(branch);

            _buildercontext.SetCurrent(branch);
            var any = base.VisitChildren(context);
            _buildercontext.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_break(Statement_breakContext context)
        {
            var indent = _buildercontext.CheckIndent(context, context.indent());
            var codeBlock = _buildercontext.GetCodeBlock(indent);

            var branch = new AstBranch(context);
            codeBlock.AddItem(branch);

            return VisitChildren(context);
        }

        public override object? VisitStatement_continue(Statement_continueContext context)
        {
            var indent = _buildercontext.CheckIndent(context, context.indent());
            var codeBlock = _buildercontext.GetCodeBlock(indent);

            var branch = new AstBranch(context);
            codeBlock.AddItem(branch);

            return VisitChildren(context);
        }

        public override object? VisitIdentifier_type(Identifier_typeContext context)
        {
            _buildercontext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_var(Identifier_varContext context)
        {
            _buildercontext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_param(Identifier_paramContext context)
        {
            _buildercontext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_func(Identifier_funcContext context)
        {
            _buildercontext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_field(Identifier_fieldContext context)
        {
            _buildercontext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitIdentifier_enumoption(Identifier_enumoptionContext context)
        {
            _buildercontext.AddIdentifier(new AstIdentifier(context));
            return null;
        }

        public override object? VisitFunction_parameter(Function_parameterContext context)
        {
            var funcParam = new AstFunctionParameterDefinition(context);
            var function = _buildercontext.GetCurrent<AstFunctionDefinitionImpl>();
            function.TryAddParameter(funcParam);

            _buildercontext.SetCurrent(funcParam);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();

            return any;
        }

        public override object? VisitFunction_parameter_self(Function_parameter_selfContext context)
        {
            var function = _buildercontext.GetCurrent<AstFunctionDefinitionImpl>();
            var funcParam = new AstFunctionParameterDefinition(context);
            funcParam.SetIdentifier(AstIdentifierIntrinsic.Self);
            function.TryAddParameter(funcParam);

            _buildercontext.SetCurrent(funcParam);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            return any;
        }

        public override object? VisitFunction_call(Function_callContext context)
        {
            var function = new AstFunctionReference(context);
            var codeBlock = _buildercontext.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            codeBlock!.AddItem(function);

            _buildercontext.SetCurrent(function);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();

            var symbols = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(function);

            return any;
        }

        public override object? VisitFunction_param_use(Function_param_useContext context)
        {
            var param = new AstFunctionParameterReference(context);
            var function = _buildercontext.GetCurrent<AstFunctionReference>();
            function.TryAddParameter(param);

            _buildercontext.SetCurrent(param);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();

            return any;
        }

        public override object? VisitVariable_def_typed(Variable_def_typedContext context)
        {
            var variable = new AstVariableDefinition(context);
            var codeBlock = _buildercontext.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            codeBlock!.AddItem(variable);

            _buildercontext.SetCurrent(variable);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();

            var symbols = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(variable);

            return any;
        }

        public override object? VisitVariable_def_typed_init(Variable_def_typed_initContext context)
        {
            var assign = new AstAssignment(context);
            var codeBlock = _buildercontext.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            codeBlock.AddItem(assign);
            _buildercontext.SetCurrent(assign);

            var variable = new AstVariableDefinition(context);
            assign.SetVariable(variable);

            _buildercontext.SetCurrent(variable);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            _buildercontext.RevertCurrent();

            var symbols = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(variable);

            return any;
        }

        public override object? VisitVariable_assign_auto(Variable_assign_autoContext context)
        {
            var assign = new AstAssignment(context);
            var codeBlock = _buildercontext.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            codeBlock!.AddItem(assign);
            _buildercontext.SetCurrent(assign);

            var variable = new AstVariableReference(context);
            assign.SetVariable(variable);

            _buildercontext.SetCurrent(variable);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            _buildercontext.RevertCurrent();

            var symbols = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            symbols.Symbols.Add(variable);

            return any;
        }

        public override object? VisitVariable_def(Variable_defContext context)
        {
            var indent = _buildercontext.CheckIndent(context, context.indent());
            var codeBlock = _buildercontext.GetCodeBlock(indent);

            _buildercontext.SetCurrent(codeBlock);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            return any;
        }

        public override object? VisitVariable_assign(Variable_assignContext context)
        {
            var indent = _buildercontext.CheckIndent(context, context.indent());
            var codeBlock = _buildercontext.GetCodeBlock(indent);

            _buildercontext.SetCurrent(codeBlock);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            return any;
        }


        public override object? VisitExpression_value(Expression_valueContext context)
        {
            var builder = new AstExpressionBuilder(_buildercontext);
            var expr = builder.Build(context);
            if (expr != null)
            {
                var site = _buildercontext.GetCurrent<IAstExpressionSite>();
                site.SetExpression(expr);
            }
            return null;
        }

        public override object? VisitExpression_logic(Expression_logicContext context)
        {
            var builder = new AstExpressionBuilder(_buildercontext);
            var expr = builder.Build(context);
            if (expr != null)
            {
                var site = _buildercontext.GetCurrent<IAstExpressionSite>();
                site.SetExpression(expr);
            }
            return null;
        }

        public override object? VisitType_ref_use(Type_ref_useContext context)
        {
            var typeRef = AstTypeReference.Create(context);
            var trSite = _buildercontext.GetCurrent<IAstTypeReferenceSite>();
            trSite.SetTypeReference(typeRef);

            var symbolsSite = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbolsSite.Symbols.Find(typeRef);
            if (entry != null)
                typeRef.SetSymbol(entry);

            return null;
        }
    }
}