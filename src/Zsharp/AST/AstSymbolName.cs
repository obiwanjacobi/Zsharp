using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstSymbolName : IEnumerable<string>
    {
        public const char Separator = '.';
        public const char TemplateDelimiter = '%';
        public const char ParameterDelimiter = ';';

        private AstSymbolName(string[] parts, string templatePostfix, bool isCanonical)
        {
            _parts = parts;
            TemplatePostfix = templatePostfix;
            IsCanonical = isCanonical;
        }

        public static AstSymbolName Parse(string text, bool toCanonical = false)
        {
            var parts = text.Split(Separator).ToArray();
            var templatePostfix = String.Empty;

            var lastIndex = parts.Length - 1;
            if (lastIndex >= 0)
            {
                var lastPart = parts[lastIndex];
                var postFix = lastPart?.Split(new[] { TemplateDelimiter, ParameterDelimiter });
                if (postFix?.Length == 2)
                {
                    parts[lastIndex] = postFix[0];
                    // includes delimiter(s)
                    templatePostfix = lastPart![postFix[0].Length..];
                }
            }

            if (toCanonical)
                parts = parts.Select(ToCanonical).ToArray();

            return new AstSymbolName(parts, templatePostfix, toCanonical);
        }

        public bool IsCanonical { get; }

        public int Count => _parts.Length;

        private readonly string[] _parts;
        public IEnumerable<string> Parts => _parts;

        public bool IsDotName => _parts.Length > 1;

        public string FullName => ToString(TemplatePostfix);

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

        public string TemplateDefinitionName
        {
            get
            {
                if (TemplatePostfix.Contains(TemplateDelimiter))
                {
                    return ToString(TemplatePostfix);
                }

                if (TemplatePostfix.Contains(ParameterDelimiter))
                {
                    var parts = TemplatePostfix.Split(ParameterDelimiter);
                    return ToString($"{TemplateDelimiter}{parts.Length - 1}");
                }

                return String.Empty;
            }
        }

        public string TemplatePostfix { get; internal set; }

        public void SetTemplateParameterCount(int count)
            => TemplatePostfix = $"{TemplateDelimiter}{count}";

        public void AddTemplateParameter(string? name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                TemplatePostfix += ParameterDelimiter + name;
            }
        }

        public AstSymbolName ToCanonical()
        {
            if (IsCanonical)
                return this;

            var parts = _parts.Select(ToCanonical).ToArray();
            return new AstSymbolName(parts, TemplatePostfix, isCanonical: true);
        }

        private static string ToCanonical(string symbolName)
        {
            if (String.IsNullOrEmpty(symbolName))
                return symbolName;

            var simplified = symbolName.Replace("_", String.Empty);
            var prefix = String.Empty;

            // .NET property getters and setters
            if (symbolName.StartsWith("get_") || symbolName.StartsWith("set_"))
            {
                prefix = simplified.Substring(0, 3);
                simplified = simplified.Substring(3);
            }

            return prefix + simplified[0] + simplified[1..].ToLowerInvariant();
        }

        public override string ToString()
            => ToString(TemplatePostfix);

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var part in _parts)
            {
                yield return part;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private string Join(int offset, int length)
            => String.Join(Separator, _parts.Skip(offset).Take(length));

        private string ToString(string postfix)
            => Join(0, _parts.Length) + postfix;
    }
}
