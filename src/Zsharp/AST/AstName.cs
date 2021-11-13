using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public enum AstNameKind
    {
        /// <summary>From source code.</summary>
        Local,
        /// <summary>From imported or to exported.</summary>
        External,
        /// <summary>Internal representation.</summary>
        Canonical,
    }

    public class AstName
    {
        public const char Separator = '.';
        public const char TemplateDelimiter = '%';
        public const char GenericDelimiter = '`';
        public const char ArgumentDelimiter = ';';

        private AstName(NamePart[] parts, AstNameKind nameKind)
        {
            Prefix = String.Empty;
            Postfix = String.Empty;
            Kind = nameKind;

            _parts = parts;
            ParseParts();
        }
        /// <summary>For external names</summary>
        private AstName(string @namespace, string name, AstNameKind nameKind)
        {
            Ast.Guard(!String.IsNullOrEmpty(name), "Name cannot be empty or null.");
            Prefix = String.Empty;
            Postfix = String.Empty;
            Kind = nameKind;

            _parts = @namespace.Replace("`", "%")
                .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Append(name.Replace("`", "%"))
                .Select(p => new NamePart(p))
                .ToArray();

            ParseParts();
        }
        public AstName(AstName nameToCopy)
        {
            _parts = new NamePart[nameToCopy._parts.Length];
            nameToCopy._parts.CopyTo(_parts, 0);
            _nameIndex = nameToCopy._nameIndex;
            Kind = nameToCopy.Kind;
            Prefix = nameToCopy.Prefix;
            Postfix = nameToCopy.Postfix;
        }

        private readonly NamePart[] _parts;
        public IEnumerable<string> Parts => _parts.Select(p => p.ToString()!);

        public AstNameKind Kind { get; }

        public bool IsDotName => _parts?.Length > 1;

        public string Prefix { get; set; }
        public string Postfix { get; set; }

        public int GetArgumentCount()
        {
            if (String.IsNullOrEmpty(Postfix))
                return 0;

            string[] parts;
            if (Postfix.Contains(ArgumentDelimiter))
            {
                parts = Postfix.Split(new[] { ArgumentDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Any()) return parts.Length;
            }

            parts = Postfix.Split(new[] { TemplateDelimiter, GenericDelimiter }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Select(p => Int32.Parse(p)).Sum();
        }

        private int _nameIndex;
        public string Namespace => PartsToString(0, _nameIndex);
        public string Symbol => _parts[_nameIndex].Name;
        public string Name => $"{Prefix}{Symbol}{Postfix}";
        public string FullName
            => String.IsNullOrEmpty(Namespace) ? $"{Name}" : $"{Namespace}.{Name}";

        public string WithoutPostfix
            => String.IsNullOrEmpty(Namespace) ? $"{Prefix}{Symbol}" : $"{Namespace}.{Prefix}{Symbol}";

        public override string ToString()
            => FullName;

        public AstName ToCanonical()
        {
            var canonical = new AstName(_parts.Select(PartToCanonical).ToArray(), AstNameKind.Canonical);
            canonical.Prefix = Prefix;
            canonical.Postfix = Postfix;
            return canonical;
        }

        public static AstName FromLocal(string name, string @namespace = "")
            => new(@namespace, name, AstNameKind.Local);

        public static AstName FromCanoncal(string name, string @namespace = "")
            => new(@namespace, name, AstNameKind.Canonical);

        public static AstName FromExternal(string @namespace, string name)
            => new(@namespace, name, AstNameKind.External);

        public static AstName FromParts(string[] parts, AstNameKind nameKind)
            => new(parts.Select(p => new NamePart(p)).ToArray(), nameKind);

        public static AstName ParseFullName(string fullName, AstNameKind nameKind = AstNameKind.Local)
        {
            var parts = fullName
                .Split(Separator)
                .Select(p => new NamePart(p))
                .ToArray();

            return new AstName(parts, nameKind);
        }

        private static readonly AstName _empty = new(new NamePart[0], AstNameKind.Local);
        public static AstName Empty => _empty;

        private static NamePart PartToCanonical(NamePart part)
        {
            if (String.IsNullOrEmpty(part.Name))
                return new NamePart(part);

            // remove discards '_'
            var canonical = part.Name.Replace("_", String.Empty);
            // preserve casing of first letter
            return new NamePart(canonical[0] + canonical[1..].ToLowerInvariant())
            {
                Prefix = part.Prefix,
                Postfix = part.Postfix
            };
        }

        private void ParseParts()
        {
            var length = _parts.Length;
            if (length == 0)
            {
                _nameIndex = -1;
                return;
            }

            if (length > 1)
                _nameIndex = length - 1;
            else
                _nameIndex = 0;

            var namePart = _parts[_nameIndex];

            if (namePart.Prefix.Length == 0)
            {
                Prefix = ParsePrefix(namePart.Name);
                namePart.Prefix = Prefix;
                namePart.Name = namePart.Name[Prefix.Length..];
            }
            if (namePart.Postfix.Length == 0)
            {
                Postfix = ParsePostfix(namePart.Name);
                namePart.Postfix = Postfix;
                namePart.Name = namePart.Name[..^Postfix.Length];
            }
        }

        private string ParsePrefix(string name)
        {
            foreach (var prefix in Prefixes)
            {
                if (name.StartsWith(prefix))
                {
                    return prefix;
                }
            }

            return String.Empty;
        }

        private static string ParsePostfix(string symbolName)
        {
            var postfix = String.Empty;

            var parts = symbolName.Split(new[] { TemplateDelimiter, GenericDelimiter, ArgumentDelimiter });
            if (parts.Length > 1)
            {
                // includes and replace delimiter
                postfix = symbolName[parts[0].Length..]
                    .Replace(GenericDelimiter, TemplateDelimiter);
            }

            return postfix;
        }

        private string PartsToString(int offset, int length)
            => String.Join(Separator, _parts.Skip(offset).Take(length));

        private static readonly string[] Prefixes = { "get_", "set_", "init_", "op_" };

        //---------------------------------------------------------------------

        private class NamePart
        {
            public NamePart(string name)
            {
                Prefix = String.Empty;
                Name = name;
                Postfix = String.Empty;
            }

            public NamePart(NamePart part)
            {
                Prefix = part.Prefix;
                Name = part.Name;
                Postfix = part.Postfix;
            }

            public string Prefix;
            public string Name;
            public string Postfix;

            public override string ToString()
                => $"{Prefix}{Name}{Postfix}";
        }
    }
}
