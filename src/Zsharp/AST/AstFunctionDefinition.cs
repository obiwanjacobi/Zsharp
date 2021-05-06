using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zsharp.AST
{
    public abstract class AstFunctionDefinition : AstFunction<AstFunctionParameterDefinition, AstTemplateParameterDefinition>
    {
        public new IEnumerable<AstTemplateParameterDefinition> TemplateParameters
            => base.TemplateParameters.Cast<AstTemplateParameterDefinition>();

        public override bool TryAddTemplateParameter(AstTemplateParameter? templateParameter)
        {
            if (TemplateParameters.SingleOrDefault(p => p.Identifier?.CanonicalName ==
                    ((AstTemplateParameterDefinition?)templateParameter)?.Identifier?.CanonicalName) == null &&
                base.TryAddTemplateParameter(templateParameter))
            {
                Identifier!.TemplateParameterCount = TemplateParameters.Count();
                return true;
            }
            return false;
        }

        public virtual bool IsIntrinsic => false;

        public virtual bool IsExternal => false;

        public override void Accept(AstVisitor visitor)
        {
            visitor.VisitFunctionDefinition(this);
        }

        public override string? ToString()
        {
            var txt = new StringBuilder();

            txt.Append(Identifier!.Name);
            txt.Append(": ");

            if (IsTemplate)
            {
                txt.Append("<");
                for (int i = 0; i < TemplateParameters.Count(); i++)
                {
                    if (i > 0)
                        txt.Append(", ");

                    var p = TemplateParameters.ElementAt(i);
                    txt.Append(p.Identifier!.Name);
                }
                txt.Append(">");
            }

            txt.Append("(");
            for (int i = 0; i < Parameters.Count(); i++)
            {
                if (i > 0)
                    txt.Append(", ");

                var p = Parameters.ElementAt(i);
                txt.Append(p.Identifier!.Name);
                txt.Append(": ");
                txt.Append(p.TypeReference!.Identifier!.Name);
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
