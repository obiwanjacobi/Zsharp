using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zsharp.AST
{
    public sealed class AstTemplateArgumentMap 
        : AstParameterArgumentMap<AstTemplateParameterDefinition, AstTemplateParameterReference>
    {
        public AstTemplateArgumentMap(
            IEnumerable<AstTemplateParameterDefinition> templateParameters,
            IEnumerable<AstTemplateParameterReference> templateArguments)
            : base(templateParameters, templateArguments)
        { }

        public override string ToString()
        {
            var txt = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                var p = ParameterAt(i);
                var a = ArgumentAt(i);

                if (i > 0)
                    txt.Append(", ");

                txt.Append(p.Identifier.SymbolName.NativeName.Name)
                    .Append("=")
                    .Append(a?.TypeReference.Identifier.SymbolName.NativeName.Name ?? "-");
            }
            return txt.ToString();
        }

        private static readonly AstTemplateArgumentMap _empty
            = new (Enumerable.Empty<AstTemplateParameterDefinition>(), Enumerable.Empty<AstTemplateParameterReference>());

        public static AstTemplateArgumentMap Empty => _empty;
    }
}
