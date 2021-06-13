using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public enum AstSymbolNameParseOptions
    {
        /// <summary>text is from source code</summary>
        IsSource,
        /// <summary>text is canonical format</summary>
        IsCanonical,
        /// <summary>convert source to to canonical format</summary>
        ToCanonical,
    }

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

        public AstSymbolName(AstSymbolName symbolNameToCopy)
            : this(symbolNameToCopy.Parts.ToArray(), symbolNameToCopy.TemplatePostfix, symbolNameToCopy.IsCanonical)
        { }

        public static AstSymbolName Parse(string text, AstSymbolNameParseOptions options)
        {
            var parts = text.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToArray();
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

            if (options == AstSymbolNameParseOptions.ToCanonical)
                parts = parts.Select(PartToCanonical).ToArray();

            return new AstSymbolName(parts, templatePostfix, options != AstSymbolNameParseOptions.IsSource);
        }

        public static string ToCanonical(string text)
            => Parse(text, AstSymbolNameParseOptions.ToCanonical).ToString();

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

            var parts = _parts.Select(PartToCanonical).ToArray();
            return new AstSymbolName(parts, TemplatePostfix, isCanonical: true);
        }

        private static string PartToCanonical(string part)
        {
            if (String.IsNullOrEmpty(part))
                return part;

            var simplified = part.Replace("_", String.Empty);
            var prefix = String.Empty;

            // .NET property getters and setters
            if (part.StartsWith("get_") || part.StartsWith("set_"))
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
