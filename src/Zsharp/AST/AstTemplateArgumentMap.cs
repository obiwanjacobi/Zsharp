using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public sealed class AstTemplateArgumentMap
    {
        private readonly AstTemplateParameterDefinition[] _templateParameters;
        private readonly AstTemplateParameterReference[] _templateArguments;

        public AstTemplateArgumentMap(
            IEnumerable<AstTemplateParameterDefinition> templateParameters,
            IEnumerable<AstTemplateParameterReference> templateArguments)
        {
            _templateParameters = templateParameters?.ToArray() ?? throw new ArgumentNullException(nameof(templateParameters));
            _templateArguments = templateArguments?.ToArray() ?? throw new ArgumentNullException(nameof(templateArguments));

            Ast.Guard(_templateArguments.Length <= _templateParameters.Length, "There can never be more template arguments than parameters");
        }

        public int Count 
            => _templateParameters.Length;

        public bool IsMatched
            => _templateParameters.Length == _templateArguments.Length;

        public IEnumerable<AstTemplateParameterDefinition> Parameters
            => _templateParameters;

        public AstTemplateParameterDefinition ParameterAt(int index)
        {
            if (index < 0 || index >= _templateParameters.Length)
                throw new ArgumentOutOfRangeException(nameof(index), 
                    $"Parameter index {index} is out of range (0-{_templateParameters.Length - 1})");

            return _templateParameters[index];
        }

        public AstTemplateParameterDefinition LookupParameter(AstTemplateParameterReference templateArgument)
            => ParameterAt(IndexOf(templateArgument));

        public int IndexOf(AstTemplateParameterDefinition templateParameter)
            => Array.IndexOf(_templateParameters, templateParameter);

        public IEnumerable<AstTemplateParameterReference> Arguments
            => _templateArguments;

        public AstTemplateParameterReference? ArgumentAt(int index)
        {
            if (index < 0 || index >= _templateParameters.Length)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Argument index {index} is out of range (0-{_templateParameters.Length - 1})");

            if (index >= _templateArguments.Length)
                return null;

            return _templateArguments[index];
        }

        public AstTemplateParameterReference? LookupArgument(AstIdentifier templateParameterName)
        {
            var parameter = FindByName(_templateParameters, templateParameterName.CanonicalFullName);
            if (parameter is not null)
            {
                return LookupArgument(parameter);
            }
            return null;
        }

        public AstTemplateParameterReference? LookupArgument(AstTemplateParameterDefinition templateParameter)
            => ArgumentAt(IndexOf(templateParameter));

        public int IndexOf(AstTemplateParameterReference templateArgument)
            => Array.IndexOf(_templateArguments, templateArgument);

        public (AstTemplateParameterDefinition parameter, AstTemplateParameterReference? argument) this[int index]
            => (parameter: ParameterAt(index), argument: ArgumentAt(index));

        private static readonly AstTemplateArgumentMap _empty
            = new AstTemplateArgumentMap(Enumerable.Empty<AstTemplateParameterDefinition>(), Enumerable.Empty<AstTemplateParameterReference>());
        public static AstTemplateArgumentMap Empty => _empty;

        private static T? FindByName<T>(T[] source, string canonicalFullName)
            where T : class, IAstIdentifierSite
        {
            foreach (IAstIdentifierSite item in source)
            {
                if (item.Identifier?.CanonicalFullName == canonicalFullName)
                {
                    return (T)item;
                }
            }
            return null;
        }
    }
}
