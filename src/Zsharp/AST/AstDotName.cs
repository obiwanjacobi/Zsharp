using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstDotName
    {
        public AstDotName(string identifier)
        {
            _parts = identifier.Split('.')
                .Select(ToCanonical).ToArray();
        }

        public int Count => _parts.Length;

        private readonly string[] _parts;

        public IEnumerable<string> Parts => _parts;

        public string ModuleName
        {
            get
            {
                if (_parts.Length > 1)
                {
                    return Join(0, _parts.Length - 1);
                }
                return _parts[0];
            }
        }

        public string Symbol
        {
            get
            {
                if (_parts.Length > 1)
                {
                    return _parts[^1];
                }
                return String.Empty;
            }
        }

        public override string ToString()
        {
            return Join(0, _parts.Length);
        }

        public static string ToCanonical(string symbolName)
        {
            if (String.IsNullOrEmpty(symbolName))
                return symbolName;

            var simplified = symbolName.Replace("_", String.Empty);
            var prefix = String.Empty;

            // .NET property getters and setters
            if (symbolName.StartsWith("_get") || symbolName.StartsWith("_set"))
            {
                prefix = simplified.Substring(0, 3);
                simplified = simplified.Substring(3);
            }

            return prefix + simplified[0] + simplified[1..].ToLowerInvariant();
        }

        private string Join(int offset, int length)
        {
            return String.Join('.', _parts.Skip(offset).Take(length));
        }
    }
}
