using System;

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

        public AstName(string @namespace, string name, AstNameKind nameKind)
        {
            Namespace = @namespace;
            Name = name;
            Kind = nameKind;
        }
        public AstName(AstName nameToCopy)
        {
            Name = nameToCopy.Name;
            Namespace = nameToCopy.Namespace;
            Kind = nameToCopy.Kind;
        }

        public AstNameKind Kind { get; }

        public bool IsDotName 
            => !String.IsNullOrEmpty(Namespace);

        public string Namespace { get; }
        public string Name { get; }
        public string FullName
            => $"{Namespace}.{Name}";

        public override string ToString()
            => FullName;

        public static AstName ParseLocal(string fullName)
            => Parse(fullName, AstNameKind.Local);

        public static AstName Parse(string fullName, AstNameKind nameKind)
        {
            var parts = fullName.Split(Separator);
            var name = parts[^1];
            var ns = parts.Length > 1
                ? String.Join(Separator, parts[..^1])
                : String.Empty;
            return new AstName(ns, name, nameKind);
        }
    }
}
