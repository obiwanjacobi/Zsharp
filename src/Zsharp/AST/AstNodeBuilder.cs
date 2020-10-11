using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using static ZsharpParser;

namespace Zsharp.AST
{
    public class AstNodeBuilder : ZsharpBaseVisitor<object?>
    {
        private readonly AstBuilderContext _builderCtx;
        private readonly string _namespace;

        public AstNodeBuilder(AstBuilderContext context, string ns)
        {
            _builderCtx = context;
            _namespace = ns;
        }

        public IEnumerable<AstError> Errors => _builderCtx.Errors;
        public bool HasErrors => _builderCtx.HasErrors;

        private static bool IsEmpty(ParserRuleContext ctx)
        {
            return ctx.children.Count == 0;
        }

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

        public override object? VisitFile(FileContext ctx)
        {
            var file = new AstFile(_namespace, _builderCtx.IntrinsicSymbols, ctx);
            _builderCtx.SetCurrent(file);

            var any = base.VisitChildren(ctx);

            _builderCtx.RevertCurrent();
            //guard(!hasCurrent());

            return file;
        }

        public override object? VisitStatement_import(Statement_importContext ctx)
        {
            var file = _builderCtx.GetCurrent<AstFile>();
            file.AddImport(ctx);

            var entry = file.Symbols.AddSymbol(ctx.module_name().GetText(), AstSymbolKind.NotSet, null);
            entry.SymbolLocality = AstSymbolLocality.Imported;

            return null;
        }

        public override object? VisitStatement_export(Statement_exportContext ctx)
        {
            var file = _builderCtx.GetCurrent<AstFile>();
            file.AddExport(ctx);

            var entry = file.Symbols.AddSymbol(ctx.identifier_func().GetText(), AstSymbolKind.NotSet, null);
            entry.SymbolLocality = AstSymbolLocality.Exported;

            return null;
        }

        public override object? VisitFunction_def(Function_defContext ctx)
        {
            var file = _builderCtx.GetCurrent<AstFile>();
            var function = new AstFunction(ctx);

            bool success = file.AddFunction(function);
            Ast.Guard(success, "AddFunction() failed");

            _builderCtx.SetCurrent(function);

            // process identifier first (needed for symbol)
            var identifier = ctx.identifier_func();
            VisitIdentifier_func(identifier);
            Ast.Guard(function.Identifier, "Function Identifier is not set.");

            var symbolTable = _builderCtx.GetCurrent<IAstSymbolTableSite>();
            var entry = symbolTable.AddSymbol(function.Identifier!.Name, AstSymbolKind.Function, function);

            if (ctx.Parent is Function_def_exportContext)
            {
                entry.SymbolLocality = AstSymbolLocality.Exported;
            }

            var any = VisitChildrenExcept(ctx, identifier);
            _builderCtx.RevertCurrent();
            return any;
        }

        public override object? VisitCodeblock(CodeblockContext ctx)
        {
            if (IsEmpty(ctx))
            {
                _builderCtx.AddError(ctx, AstError.EmptyCodeBlock);
                return null;
            }

            var stSite = _builderCtx.GetCurrent<IAstSymbolTableSite>();
            var symbols = stSite.Symbols;
            string scopeName = symbols.Namespace;

            var cbSite = _builderCtx.GetCurrent<IAstCodeBlockSite>();
            var parent = cbSite as AstFunction;
            if (parent?.Identifier != null)
            {
                scopeName = parent.Identifier.Name;
            }

            var codeBlock = new AstCodeBlock(scopeName, symbols, ctx);
            bool success = cbSite.SetCodeBlock(codeBlock);
            Ast.Guard(success, "SetCodeBlock() failed");

            _builderCtx.SetCurrent(codeBlock);
            var any = base.VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_if(Statement_ifContext ctx)
        {
            var indent = _builderCtx.CheckIndent(ctx, ctx.indent());
            var codeBlock = _builderCtx.GetCodeBlock(indent);

            var branch = new AstBranchConditional(ctx);
            bool success = codeBlock.AddItem(branch);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed");

            _builderCtx.SetCurrent(branch);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_else(Statement_elseContext ctx)
        {
            var indent = _builderCtx.CheckIndent(ctx, ctx.indent());
            var branch = _builderCtx.GetCurrent<AstBranchConditional>();
            Ast.Guard(indent == branch.Indent, "Indentation mismatch CodeBlock <=> Branch");

            var subBr = new AstBranchConditional(ctx);
            bool success = branch.AddSubBranch(subBr);
            Ast.Guard(success, "AddSubBranch() failed");

            _builderCtx.SetCurrent(subBr);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_elseif(Statement_elseifContext ctx)
        {
            var indent = _builderCtx.CheckIndent(ctx, ctx.indent());
            var branch = _builderCtx.GetCurrent<AstBranchConditional>();
            Ast.Guard(indent == branch.Indent, "Indentation mismatch CodeBlock <=> Branch");

            var subBr = new AstBranchConditional(ctx);
            bool success = branch.AddSubBranch(subBr);
            Ast.Guard(success, "AddSubBranch() failed");

            _builderCtx.SetCurrent(subBr);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_return(Statement_returnContext ctx)
        {
            var indent = _builderCtx.CheckIndent(ctx, ctx.indent());
            var codeBlock = _builderCtx.GetCodeBlock(indent);

            var branch = new AstBranchExpression(ctx);
            bool success = codeBlock.AddItem(branch);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

            _builderCtx.SetCurrent(branch);
            var any = base.VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            return any;
        }

        public override object? VisitStatement_break(Statement_breakContext ctx)
        {
            var indent = _builderCtx.CheckIndent(ctx, ctx.indent());
            var codeBlock = _builderCtx.GetCodeBlock(indent);

            var branch = new AstBranch(ctx);
            bool success = codeBlock.AddItem(branch);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

            return VisitChildren(ctx);
        }

        public override object? VisitStatement_continue(Statement_continueContext ctx)
        {
            var indent = _builderCtx.CheckIndent(ctx, ctx.indent());
            var codeBlock = _builderCtx.GetCodeBlock(indent);

            var branch = new AstBranch(ctx);
            bool success = codeBlock.AddItem(branch);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

            return VisitChildren(ctx);
        }

        public override object? VisitIdentifier_type(Identifier_typeContext ctx)
        {
            bool success = _builderCtx.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(Type) failed");
            return null;
        }

        public override object? VisitIdentifier_var(Identifier_varContext ctx)
        {
            bool success = _builderCtx.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(Variable) failed");
            return null;
        }

        public override object? VisitIdentifier_param(Identifier_paramContext ctx)
        {
            bool success = _builderCtx.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(Parameter) failed");
            return null;
        }

        public override object? VisitIdentifier_func(Identifier_funcContext ctx)
        {
            bool success = _builderCtx.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(Function) failed");
            return null;
        }

        public override object? VisitIdentifier_field(Identifier_fieldContext ctx)
        {
            bool success = _builderCtx.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(Field) failed");
            return null;
        }

        public override object? VisitIdentifier_enumoption(Identifier_enumoptionContext ctx)
        {
            bool success = _builderCtx.AddIdentifier(new AstIdentifier(ctx));
            Ast.Guard(success, "AddIdentifier(EnumOption) failed");
            return null;
        }

        public override object? VisitFunction_parameter(Function_parameterContext ctx)
        {
            var funcParam = new AstFunctionParameter(ctx);
            var function = _builderCtx.GetCurrent<AstFunction>();
            function.AddParameter(funcParam);

            _builderCtx.SetCurrent(funcParam);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            return any;
        }

        public override object? VisitFunction_parameter_self(Function_parameter_selfContext ctx)
        {
            var function = _builderCtx.GetCurrent<AstFunction>();
            var funcParam = new AstFunctionParameter(ctx);
            // Todo: make self static and clone
            funcParam.SetIdentifier(new AstIdentifierIntrinsic("self", AstIdentifierType.Parameter));
            function.AddParameter(funcParam);

            _builderCtx.SetCurrent(funcParam);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            return any;
        }

        public override object? VisitVariable_def(Variable_defContext ctx)
        {
            var indent = _builderCtx.CheckIndent(ctx, ctx.indent());
            var codeBlock = _builderCtx.GetCodeBlock(indent);

            _builderCtx.SetCurrent(codeBlock);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            return any;
        }

        public override object? VisitVariable_def_typed(Variable_def_typedContext ctx)
        {
            var variable = new AstVariableDefinition(ctx);
            var codeBlock = _builderCtx.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            bool success = codeBlock!.AddItem(variable);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

            _builderCtx.SetCurrent(variable);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();

            var symbols = _builderCtx.GetCurrent<IAstSymbolTableSite>();
            var entry = symbols.AddSymbol(variable, AstSymbolKind.Variable, variable);

            return any;
        }

        public override object? VisitVariable_def_typed_init(Variable_def_typed_initContext ctx)
        {
            var assign = new AstAssignment(ctx);
            var codeBlock = _builderCtx.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            bool success = codeBlock!.AddItem(assign);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

            _builderCtx.SetCurrent(assign);

            var variable = new AstVariableDefinition(ctx);
            success = assign.SetVariable(variable);
            Ast.Guard(success, "SetVariable() failed");

            _builderCtx.SetCurrent(variable);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            _builderCtx.RevertCurrent();

            var symbols = _builderCtx.GetCurrent<IAstSymbolTableSite>();
            var entry = symbols.AddSymbol(variable, AstSymbolKind.Variable, variable);

            return any;
        }

        public override object? VisitVariable_assign_auto(Variable_assign_autoContext ctx)
        {
            var assign = new AstAssignment(ctx);
            var codeBlock = _builderCtx.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            bool success = codeBlock!.AddItem(assign);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");
            _builderCtx.SetCurrent(assign);

            AstVariable variable;
            if (ctx.Parent is Variable_assignContext)
            {
                variable = new AstVariableReference(ctx);
            }
            else
            {
                variable = new AstVariableDefinition(ctx);
            }

            success = assign.SetVariable(variable);
            Ast.Guard(success, "SetVariable() failed");

            _builderCtx.SetCurrent(variable);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            _builderCtx.RevertCurrent();

            var symbols = _builderCtx.GetCurrent<IAstSymbolTableSite>();
            var entry = symbols.AddSymbol(variable, AstSymbolKind.Variable, variable);

            return any;
        }

        public override object? VisitVariable_assign(Variable_assignContext ctx)
        {
            var indent = _builderCtx.CheckIndent(ctx, ctx.indent());
            var codeBlock = _builderCtx.GetCodeBlock(indent);

            _builderCtx.SetCurrent(codeBlock);
            var any = VisitChildren(ctx);
            _builderCtx.RevertCurrent();
            return any;
        }


        public override object? VisitExpression_value(Expression_valueContext ctx)
        {
            var builder = new AstExpressionBuilder(_builderCtx);
            var expr = builder.Build(ctx);
            if (expr != null)
            {
                var site = _builderCtx.GetCurrent<IAstExpressionSite>();
                bool success = site.SetExpression(expr);
                Ast.Guard(success, "SetExpression() failed");
            }
            return null;
        }

        public override object? VisitExpression_logic(Expression_logicContext ctx)
        {
            var builder = new AstExpressionBuilder(_builderCtx);
            var expr = builder.Build(ctx);
            if (expr != null)
            {
                var site = _builderCtx.GetCurrent<IAstExpressionSite>();
                bool success = site.SetExpression(expr);
                Ast.Guard(success, "SetExpression() failed");
            }
            return null;
        }

        public override object? VisitType_ref_use(Type_ref_useContext ctx)
        {
            var type = AstTypeReference.Create(ctx);
            var trSite = _builderCtx.GetCurrent<IAstTypeReferenceSite>();
            trSite.SetTypeReference(type);
            return null;
        }
    }
}