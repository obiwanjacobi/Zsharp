using System;
using Zsharp.AST;

namespace Zsharp.Emit
{
    public class EmitCode : AstVisitor
    {
        public EmitCode(string assemblyName)
        {
            Context = EmitContext.Create(assemblyName);
        }

        public EmitContext? Context { get; private set; }

        public override void VisitModulePublic(AstModulePublic module)
        {
            if (Context == null)
            {
                Context = EmitContext.Create(module.Name);
            }

            using var scope = Context.AddModule(module);

            VisitChildren(module);
        }

        public override void VisitFunctionDefinition(AstFunctionDefinition function)
        {
            using var scope = Context.AddFunction(function);

            VisitChildren(function);
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            VisitChildren(function);

            var method = Context.FindFunction(function.FunctionDefinition);
            var call = Context.InstructionFactory.Call(method);
            Context.CodeBuilder.CodeBlock.Add(call);
        }

        public override void VisitVariableDefinition(AstVariableDefinition variable)
        {
            if (!Context.HasVariable(variable.Identifier.Name))
            {
                Context.AddVariable(variable);
            }
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            base.VisitAssignment(assign);

            var varDef = Context.CodeBuilder.GetVariable(assign.Variable.Identifier.Name);
            var store = Context.InstructionFactory.StoreVariable(varDef);
            Context.CodeBuilder.CodeBlock.Add(store);
        }

        public override void VisitBranchExpression(AstBranchExpression branch)
        {
            if (branch.BranchType == AstBranchType.ExitFunction)
            {
                if (branch.HasExpression)
                {
                    VisitChildren(branch);
                }

                Context.CodeBuilder.Return();
            }
            else
            {
                throw new NotImplementedException(
                    $"Emit Branch '{branch.BranchType}' not implemented.");
            }
        }

        public override void VisitBranchConditional(AstBranchConditional branch)
        {
            Visit(branch.Expression!);

            var builder = Context.CodeBuilder;
            var nextBlock = builder.BranchConditional(
                builder.NewBlockLabel(), builder.NewBlockLabel());

            Visit(branch.CodeBlock!);

            if (branch.HasSubBranch)
            {
                // TODO: the sub-branch's nextBlock should be the root's nextBlock...
                Visit(branch.SubBranch!);
            }

            builder.CodeBlock = nextBlock;
        }

        public override void VisitExpression(AstExpression expression)
            => new EmitExpression(Context).VisitExpression(expression);

        public void SaveAs(string filePath) => Context.Assembly.Write(filePath);
    }
}
