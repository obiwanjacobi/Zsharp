using Antlr4.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Zsharp.Parser.ZsharpParser;

namespace Zsharp.AST
{
    public class AstFunctionReference : AstFunction<AstFunctionParameterReference, AstTemplateParameterReference>
    {
        public AstFunctionReference(ParserRuleContext context)
        {
            Context = context;
            EnforceReturnValueUse = context.Parent is not Function_call_retval_unusedContext;
        }

        public bool EnforceReturnValueUse { get; }

        public AstFunctionDefinition? FunctionDefinition
        {
            get
            {
                Ast.Guard(Symbol, $"No Symbol is set for Function reference {Identifier!.Name}.");
                var entry = Symbol!;

                if (entry.HasOverloads)
                {
                    return entry.FindOverloadDefinition(this);
                }

                return entry.DefinitionAs<AstFunctionDefinition>();
            }
        }

        public new IEnumerable<AstTemplateParameterReference> TemplateParameters
            => base.TemplateParameters.Cast<AstTemplateParameterReference>();

        public override bool TryAddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (base.TryAddTemplateParameter(templateParameter))
            {
                var templParamRef = (AstTemplateParameterReference)templateParameter;
                Identifier!.AddTemplateParameter(templParamRef.TypeReference?.Identifier?.Name);
                return true;
            }
            return false;
        }

        public override void Accept(AstVisitor visitor) => visitor.VisitFunctionReference(this);

        public override string? ToString()
        {
            var txt = new StringBuilder();

            txt.Append(Identifier!.Name);
            txt.Append(": (");

            for (int i = 0; i < Parameters.Count(); i++)
            {
                if (i > 0)
                    txt.Append(", ");

                var p = Parameters.ElementAt(i);
                txt.Append(p.Expression?.TypeReference?.Identifier?.Name);
            }
            txt.Append(")");

            if (TypeReference?.Identifier != null)
            {
                txt.Append(": ");
                txt.Append(TypeReference.Identifier.Name);
            }

            return txt.ToString();
        }
    }
}
