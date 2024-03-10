using System.Collections.Generic;
using System.Text;
using Maja.Compiler.IR;

namespace Maja.Compiler.Eval;

internal sealed class EvalTypeInstance
{
    public EvalTypeInstance(IrDeclarationType typeDeclaration, Dictionary<string, object> fields)
    {
        TypeDeclaration = typeDeclaration;
        Fields = fields;
    }

    public IrDeclarationType TypeDeclaration { get; }

    public Dictionary<string, object> Fields { get; }

    public T GetFieldValue<T>(string fieldName)
        => (T)Fields[fieldName];

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
