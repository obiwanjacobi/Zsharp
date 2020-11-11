using System.Linq;
using System.Text;

namespace Zsharp.AST
{
    public abstract class AstFunctionDefinition : AstFunction<AstFunctionParameterDefinition>
    {
        public override string? ToString()
        {
            var txt = new StringBuilder();

            txt.Append(Identifier.Name);
            txt.Append(": (");

            for (int i = 0; i < Parameters.Count(); i++)
            {
                if (i > 0)
                    txt.Append(", ");

                var p = Parameters.ElementAt(i);
                txt.Append(p.Identifier.Name);
                txt.Append(": ");
                txt.Append(p.TypeReference.Identifier.Name);
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
