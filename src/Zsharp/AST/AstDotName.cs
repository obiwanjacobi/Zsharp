using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstDotName : IEnumerable<string>
    {
        public const char Separator = '.';

        private AstDotName(string[] parts)
        {
            Ast.Guard(parts, "Parts is null");
            Ast.Guard(parts.Length > 0, "Parts is empty");
            _parts = parts;
        }

        public AstDotName(string canonical)
        {
            Ast.Guard(canonical, "Canonical is null");
            _parts = canonical.Split(Separator).ToArray();
        }

        public static AstDotName FromText(string text)
        {
            Ast.Guard(text, "Text is null");
            return new AstDotName(
                text.Split(Separator).Select(ToCanonical).ToArray());
        }

        public int Count => _parts.Length;

        private readonly string[] _parts;
        public IEnumerable<string> Parts => _parts;

        public bool IsDotName => _parts.Length > 1;

        // not including last part
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

        // last part
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
            return String.Join(Separator, _parts.Skip(offset).Take(length));
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var part in _parts)
            {
                yield return part;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
