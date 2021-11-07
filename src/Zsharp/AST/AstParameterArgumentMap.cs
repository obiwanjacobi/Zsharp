using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstParameterArgumentMap<ParameterT, ArgumentT>
        where ParameterT : class, IAstIdentifierSite
        where ArgumentT: class
    {
        private readonly ParameterT[] _parameters;
        private readonly ArgumentT[] _arguments;

        public AstParameterArgumentMap(
            IEnumerable<ParameterT> parameters,
            IEnumerable<ArgumentT> arguments)
        {
            _parameters = parameters?.ToArray() ?? throw new ArgumentNullException(nameof(parameters));
            _arguments = arguments?.ToArray() ?? throw new ArgumentNullException(nameof(arguments));

            Ast.Guard(_arguments.Length <= _parameters.Length, "There can never be more template arguments than parameters");
        }

        public int Count 
            => _parameters.Length;

        public bool IsMatched
            => _parameters.Length == _arguments.Length;

        public IEnumerable<ParameterT> Parameters
            => _parameters;

        public ParameterT ParameterAt(int index)
        {
            if (index < 0 || index >= _parameters.Length)
                throw new ArgumentOutOfRangeException(nameof(index), 
                    $"Parameter index {index} is out of range (0-{_parameters.Length - 1})");

            return _parameters[index];
        }

        public ParameterT LookupParameter(ArgumentT argument)
            => ParameterAt(IndexOf(argument));

        public int IndexOf(ParameterT parameter)
            => Array.IndexOf(_parameters, parameter);

        public IEnumerable<ArgumentT> Arguments
            => _arguments;

        public ArgumentT? ArgumentAt(int index)
        {
            if (index < 0 || index >= _parameters.Length)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Argument index {index} is out of range (0-{_parameters.Length - 1})");

            if (index >= _arguments.Length)
                return null;

            return _arguments[index];
        }

        public ArgumentT? LookupArgument(AstIdentifier parameterName)
        {
            var parameter = FindByName(_parameters, parameterName.CanonicalFullName);
            if (parameter is not null)
            {
                return LookupArgument(parameter);
            }
            return null;
        }

        public ArgumentT? LookupArgument(ParameterT parameter)
            => ArgumentAt(IndexOf(parameter));

        public int IndexOf(ArgumentT argument)
            => Array.IndexOf(_arguments, argument);

        public (ParameterT parameter, ArgumentT? argument) this[int index]
            => (parameter: ParameterAt(index), argument: ArgumentAt(index));

        private static T? FindByName<T>(T[] source, string canonicalFullName)
            where T : class, IAstIdentifierSite
        {
            foreach (IAstIdentifierSite item in source)
            {
                if (item.Identifier.CanonicalFullName == canonicalFullName)
                {
                    return (T)item;
                }
            }
            return null;
        }
    }
}
