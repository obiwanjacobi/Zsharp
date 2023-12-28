using System.Collections.Generic;
using System.Text;
using Maja.Compiler.IR;

namespace Maja.Compiler.Eval;

internal sealed class EvalTypeInstance
{
    public EvalTypeInstance(IrDeclarationType typeDeclaration)
    {
        TypeDeclaration = typeDeclaration;
    }

    public IrDeclarationType TypeDeclaration { get; }

    public Dictionary<string, object> Fields { get; set; }

    public override string ToString()
    {
        var txt = new StringBuilder();

        txt.Append(TypeDeclaration.Symbol.Name.Value);
        txt.AppendLine();

        foreach(var kvp in Fields)
        {
            txt.Append("  ");
            txt.Append(kvp.Key);
            txt.Append(" = ");
            txt.Append(kvp.Value);
            txt.AppendLine();
        }

        return txt.ToString();
    }
}
