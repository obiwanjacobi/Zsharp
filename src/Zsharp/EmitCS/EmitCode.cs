﻿using System;
using System.IO;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public class EmitCode : AstVisitor
    {
        public EmitCode(string assemblyName)
        {
            Context = new EmitContext(assemblyName, null);
        }

        public EmitContext Context { get; private set; }

        public override void VisitModulePublic(AstModulePublic module)
        {
            // usings / imports
            Context.Imports(module.Symbol!.SymbolTable);

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
            var functionDef = function.FunctionDefinition!;
            var name = functionDef.Identifier!.CanonicalName;

            if (functionDef.IsExternal)
            {
                name = ((AstFunctionDefinitionExternal)functionDef).ExternalName.FullName;
            }

            Context.CodeBuilder.CsBuilder.Append($"{name}(");

            VisitChildren(function);

            Context.CodeBuilder.CsBuilder.EndLine(")");
        }

        public override void VisitVariableDefinition(AstVariableDefinition variable)
        {
            if (variable.IsTopLevel())
            {
                Context.ModuleClass.AddField(variable);
            }
            else
            {
                Context.CodeBuilder.AddVariable(variable);
            }

            if (variable.ParentAs<AstAssignment>() == null)
            {
                // prevent 'use of unassigned variable' error
                Context.CodeBuilder.CsBuilder.EndLine(" = default");
            }
        }

        public override void VisitVariableReference(AstVariableReference variable)
        {
            var expressionVisitor = new EmitExpression(Context.CodeBuilder.CsBuilder);
            expressionVisitor.VisitVariableReference(variable);
        }

        public override void VisitAssignment(AstAssignment assign)
        {
            IDisposable? builderScope = null;

            if (assign.Variable is AstVariableDefinition varDef)
            {
                VisitVariableDefinition(varDef);
                if (assign.IsTopLevel())
                {
                    var field = Context.ModuleClass.ModuleClass.Fields
                        .Single(f => f.Name == varDef.Identifier!.CanonicalName);

                    builderScope = Context.SetBuilder(field.ValueBuilder);
                }
            }
            else
            {
                VisitVariableReference((AstVariableReference)assign.Variable!);
            }

            if (assign.HasFields)
            {
                Ast.Guard(assign.Variable!.TypeReference!.TypeDefinition!.IsStruct, "Expect Struct.");

                Context.CodeBuilder.CsBuilder.AppendLine(
                    $" = new {assign.Variable.TypeReference.ToCode()}");
                Context.CodeBuilder.CsBuilder.WriteIndent();
                Context.CodeBuilder.CsBuilder.StartScope();

                foreach (var field in assign.Fields)
                {
                    VisitTypeFieldInitialization(field);
                }

                Context.CodeBuilder.CsBuilder.EndScope();
            }

            if (assign.Expression != null)
            {
                Context.CodeBuilder.CsBuilder.Append(" = ");
                VisitExpression(assign.Expression);
            }

            Context.CodeBuilder.CsBuilder.EndLine();

            builderScope?.Dispose();
        }

        public override void VisitTypeFieldInitialization(AstTypeFieldInitialization field)
        {
            var assign = field.ParentAs<AstAssignment>();
            if (assign != null)
            {
                Context.CodeBuilder.CsBuilder.Append(
                    $"{field.Identifier!.CanonicalName} = ");
                VisitChildren(field);
                Context.CodeBuilder.CsBuilder.AppendLine(",");
            }
        }

        public override void VisitBranchExpression(AstBranchExpression branch)
        {
            Context.CodeBuilder.CsBuilder.WriteIndent();
            Context.CodeBuilder.StartBranch(branch);

            if (branch.HasExpression)
            {
                VisitChildren(branch);
            }

            Context.CodeBuilder.CsBuilder.EndLine();
        }

        public override void VisitBranchConditional(AstBranchConditional branch)
        {
            if (branch.HasExpression)
            {
                Context.CodeBuilder.StartBranch(branch);
                Context.CodeBuilder.CsBuilder.Append("(");

                Visit(branch.Expression!);

                Context.CodeBuilder.CsBuilder.StartScope(")");

                Visit(branch.CodeBlock!);

                Context.CodeBuilder.CsBuilder.EndScope();

                if (branch.HasSubBranch)
                {
                    var subHasExpression = branch.SubBranch!.HasExpression;
                    Context.CodeBuilder.CsBuilder.StartBranch(BranchStatement.Else);

                    if (!subHasExpression)
                        Context.CodeBuilder.CsBuilder.StartScope();

                    Visit(branch.SubBranch!);

                    if (!subHasExpression)
                        Context.CodeBuilder.CsBuilder.EndScope();
                }
            }
            else
                Visit(branch.CodeBlock!);
        }

        public override void VisitExpression(AstExpression expression)
            => new EmitExpression(Context.CodeBuilder.CsBuilder).VisitExpression(expression);

        public override void VisitTypeDefinitionEnum(AstTypeDefinitionEnum enumType)
            => Context.ModuleClass.AddEnum(enumType);

        public override void VisitTypeDefinitionStruct(AstTypeDefinitionStruct structType)
            => Context.ModuleClass.AddStruct(structType);

        public void SaveAs(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            File.WriteAllText(filePath, ToString());
        }

        public override string ToString()
        {
            return Context.Namespace.ToCode();
        }
    }
}