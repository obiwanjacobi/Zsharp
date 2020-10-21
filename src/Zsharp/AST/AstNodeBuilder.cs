using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
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

        public IEnumerable<AstError> Errors => _buildercontext.Errors;

        public bool HasErrors => _buildercontext.HasErrors;

        private static bool IsEmpty(ParserRuleContext context)
        {
            return context.children.Count == 0;
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

        // use as generic error handler
        protected override bool ShouldVisitNextChild(IRuleNode node, object? currentResult)
        {
            if (node is ParserRuleContext context &&
                context.exception != null)
            {
                _buildercontext.AddError(context, AstError.SyntaxError);
                // usually pointless to continue
                return false;
            }
            return true;
        }

        public override object? VisitFile(FileContext context)
        {
            var file = new AstFile(_namespace, _buildercontext.IntrinsicSymbols, context);

            _buildercontext.SetCurrent(file);
            base.VisitChildren(context);
            _buildercontext.RevertCurrent();
            //guard(!hasCurrent());

            return file;
        }

        public override object? VisitStatement_import(Statement_importContext context)
        {
            var file = _buildercontext.GetCurrent<AstFile>();
            file.AddImport(context);

            var entry = file.Symbols.AddSymbol(context.module_name().GetText(), AstSymbolKind.NotSet, null);
            entry.SymbolLocality = AstSymbolLocality.Imported;

            return null;
        }

        public override object? VisitStatement_export(Statement_exportContext context)
        {
            var file = _buildercontext.GetCurrent<AstFile>();
            file.AddExport(context);

            var entry = file.Symbols.AddSymbol(context.identifier_func().GetText(), AstSymbolKind.NotSet, null);
            entry.SymbolLocality = AstSymbolLocality.Exported;

            return null;
        }

        public override object? VisitFunction_def(Function_defContext context)
        {
            var file = _buildercontext.GetCurrent<AstFile>();
            var function = new AstFunction(context);

            bool success = file.AddFunction(function);
            Ast.Guard(success, "AddFunction() failed");

            _buildercontext.SetCurrent(function);

            // process identifier first (needed for symbol)
            var identifier = context.identifier_func();
            VisitIdentifier_func(identifier);
            Ast.Guard(function.Identifier, "Function Identifier is not set.");

            var symbolTable = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbolTable.AddSymbol(function.Identifier!.Name, AstSymbolKind.Function, function);
            success = function.SetSymbol(entry);
            Ast.Guard(success, "SetSymbol() failed");

            if (context.Parent is Function_def_exportContext)
            {
                entry.SymbolLocality = AstSymbolLocality.Exported;
            }

            var any = VisitChildrenExcept(context, identifier);
            _buildercontext.RevertCurrent();
            return any;
        }

        public override object? VisitCodeblock(CodeblockContext context)
        {
            if (IsEmpty(context))
            {
                _buildercontext.AddError(context, AstError.EmptyCodeBlock);
                return null;
            }

            var stSite = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            var symbols = stSite.Symbols;
            string scopeName = symbols.Namespace;

            var cbSite = _buildercontext.GetCurrent<IAstCodeBlockSite>();
            var parent = cbSite as AstFunction;
            if (parent?.Identifier != null)
            {
                scopeName = parent.Identifier.Name;
            }

            var codeBlock = new AstCodeBlock(scopeName, symbols, context);
            bool success = cbSite.SetCodeBlock(codeBlock);
            Ast.Guard(success, "SetCodeBlock() failed");

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
            bool success = codeBlock.AddItem(branch);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed");

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
            bool success = branch.AddSubBranch(subBr);
            Ast.Guard(success, "AddSubBranch() failed");

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
            bool success = branch.AddSubBranch(subBr);
            Ast.Guard(success, "AddSubBranch() failed");

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
            bool success = codeBlock.AddItem(branch);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

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
            bool success = codeBlock.AddItem(branch);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

            return VisitChildren(context);
        }

        public override object? VisitStatement_continue(Statement_continueContext context)
        {
            var indent = _buildercontext.CheckIndent(context, context.indent());
            var codeBlock = _buildercontext.GetCodeBlock(indent);

            var branch = new AstBranch(context);
            bool success = codeBlock.AddItem(branch);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

            return VisitChildren(context);
        }

        public override object? VisitIdentifier_type(Identifier_typeContext context)
        {
            bool success = _buildercontext.AddIdentifier(new AstIdentifier(context));
            Ast.Guard(success, "AddIdentifier(Type) failed");
            return null;
        }

        public override object? VisitIdentifier_var(Identifier_varContext context)
        {
            bool success = _buildercontext.AddIdentifier(new AstIdentifier(context));
            Ast.Guard(success, "AddIdentifier(Variable) failed");
            return null;
        }

        public override object? VisitIdentifier_param(Identifier_paramContext context)
        {
            bool success = _buildercontext.AddIdentifier(new AstIdentifier(context));
            Ast.Guard(success, "AddIdentifier(Parameter) failed");
            return null;
        }

        public override object? VisitIdentifier_func(Identifier_funcContext context)
        {
            bool success = _buildercontext.AddIdentifier(new AstIdentifier(context));
            Ast.Guard(success, "AddIdentifier(Function) failed");
            return null;
        }

        public override object? VisitIdentifier_field(Identifier_fieldContext context)
        {
            bool success = _buildercontext.AddIdentifier(new AstIdentifier(context));
            Ast.Guard(success, "AddIdentifier(Field) failed");
            return null;
        }

        public override object? VisitIdentifier_enumoption(Identifier_enumoptionContext context)
        {
            bool success = _buildercontext.AddIdentifier(new AstIdentifier(context));
            Ast.Guard(success, "AddIdentifier(EnumOption) failed");
            return null;
        }

        public override object? VisitFunction_parameter(Function_parameterContext context)
        {
            var funcParam = new AstFunctionParameter(context);
            var function = _buildercontext.GetCurrent<AstFunction>();
            function.AddParameter(funcParam);

            _buildercontext.SetCurrent(funcParam);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            return any;
        }

        public override object? VisitFunction_parameter_self(Function_parameter_selfContext context)
        {
            var function = _buildercontext.GetCurrent<AstFunction>();
            var funcParam = new AstFunctionParameter(context);
            // Todo: make self static and clone
            funcParam.SetIdentifier(new AstIdentifierIntrinsic("self", AstIdentifierType.Parameter));
            function.AddParameter(funcParam);

            _buildercontext.SetCurrent(funcParam);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
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

        public override object? VisitVariable_def_typed(Variable_def_typedContext context)
        {
            var variable = new AstVariableDefinition(context);
            var codeBlock = _buildercontext.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            bool success = codeBlock!.AddItem(variable);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

            _buildercontext.SetCurrent(variable);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();

            var symbols = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbols.AddSymbol(variable, AstSymbolKind.Variable, variable);
            success = variable.SetSymbol(entry);
            Ast.Guard(success, "SetSymbol() failed");

            return any;
        }

        public override object? VisitVariable_def_typed_init(Variable_def_typed_initContext context)
        {
            var assign = new AstAssignment(context);
            var codeBlock = _buildercontext.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            bool success = codeBlock!.AddItem(assign);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");

            _buildercontext.SetCurrent(assign);

            var variable = new AstVariableDefinition(context);
            success = assign.SetVariable(variable);
            Ast.Guard(success, "SetVariable() failed");

            _buildercontext.SetCurrent(variable);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            _buildercontext.RevertCurrent();

            var symbols = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbols.AddSymbol(variable, AstSymbolKind.Variable, variable);
            success = variable.SetSymbol(entry);
            Ast.Guard(success, "SetSymbol() failed");

            return any;
        }

        public override object? VisitVariable_assign_auto(Variable_assign_autoContext context)
        {
            var assign = new AstAssignment(context);
            var codeBlock = _buildercontext.GetCodeBlock();
            Ast.Guard(codeBlock, "BuilderContext did not have a CodeBlock.");

            bool success = codeBlock!.AddItem(assign);
            Ast.Guard(success, "AstCodeBlock.AddItem() failed.");
            _buildercontext.SetCurrent(assign);

            var variable = new AstVariableReference(context);
            success = assign.SetVariable(variable);
            Ast.Guard(success, "SetVariable() failed");

            _buildercontext.SetCurrent(variable);
            var any = VisitChildren(context);
            _buildercontext.RevertCurrent();
            _buildercontext.RevertCurrent();

            var symbols = _buildercontext.GetCurrent<IAstSymbolTableSite>();
            var entry = symbols.AddSymbol(variable, AstSymbolKind.Variable, variable);
            success = variable.SetSymbol(entry);
            Ast.Guard(success, "SetSymbol() failed");

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
                bool success = site.SetExpression(expr);
                Ast.Guard(success, "SetExpression() failed");
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
                bool success = site.SetExpression(expr);
                Ast.Guard(success, "SetExpression() failed");
            }
            return null;
        }

        public override object? VisitType_ref_use(Type_ref_useContext context)
        {
            var type = AstTypeReference.Create(context);
            var trSite = _buildercontext.GetCurrent<IAstTypeReferenceSite>();
            bool success = trSite.SetTypeReference(type);
            Ast.Guard(success, "SetTypeReference() failed");
            return null;
        }
    }
}