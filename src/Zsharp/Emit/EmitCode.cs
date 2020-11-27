using Mono.Cecil;
using System;
using System.IO;
using Zsharp.AST;

namespace Zsharp.Emit
{
    public class EmitCode : AstVisitor
    {
        private bool _isInit;

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

            _isInit = true;
            using var scope = Context.AddModule(module);

            VisitChildren(module);
        }

        public override void VisitFunctionDefinition(AstFunctionDefinition function)
        {
            _isInit = false;

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
            var name = variable.Identifier.CanonicalName;

            if (_isInit)
            {
                if (!Context.ModuleClass.HasField(name))
                {
                    Context.ModuleClass.AddField(name, Context.ToTypeReference(variable.TypeReference));
                }
            }
            else
            {
                if (!Context.CodeBuilder.HasVariable(name))
                {
                    Context.CodeBuilder.AddVariable(name, Context.ToTypeReference(variable.TypeReference));
                }
            }
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            base.VisitAssignment(assign);

            var name = assign.Variable.Identifier.CanonicalName;

            if (_isInit)
            {
                FieldDefinition field;

                if (Context.ModuleClass.HasField(name))
                    field = Context.ModuleClass.GetField(name);
                else
                {
                    var varDef = assign.Variable as AstVariableDefinition;
                    if (varDef == null)
                    {
                        var varRef = (AstVariableReference)assign.Variable;
                        varDef = varRef.VariableDefinition;
                    }
                    field = Context.ModuleClass.AddField(name, Context.ToTypeReference(varDef.TypeReference));
                }
                var store = Context.InstructionFactory.StoreField(field);
                Context.CodeBuilder.CodeBlock.Add(store);
            }
            else
            {
                var varDef = Context.CodeBuilder.GetVariable(name);
                var store = Context.InstructionFactory.StoreVariable(varDef);
                Context.CodeBuilder.CodeBlock.Add(store);
            }
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
            if (branch.HasExpression)
            {
                Visit(branch.Expression!);

                CodeBlock nextBlock;
                var builder = Context.CodeBuilder;
                if (branch.ParentAs<AstBranchConditional>() == null)
                {
                    nextBlock = builder.BranchConditional(
                        builder.NewBlockLabel(), builder.NewBlockLabel());
                }
                else    // sub-branch
                {
                    nextBlock = builder.Branch(builder.NewBlockLabel());
                }

                Visit(branch.CodeBlock!);

                builder.CodeBlock = nextBlock;

                if (branch.HasSubBranch)
                {
                    // TODO: the sub-branch's nextBlock should be the root's nextBlock...
                    Visit(branch.SubBranch!);
                }
            }
            else
                Visit(branch.CodeBlock!);
        }

        public override void VisitExpression(AstExpression expression)
            => new EmitExpression(Context, _isInit).VisitExpression(expression);

        public override void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
        {
            Context.ModuleClass.AddTypeEnum(enumType);
        }

        public void SaveAs(string filePath)
        {
            Context.Assembly.Name.Name = Path.GetFileNameWithoutExtension(filePath);
            Context.Assembly.Write(filePath);
        }
    }
}
