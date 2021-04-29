using System;
using System.Linq;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    internal static class CsBuilderExtensions
    {
        public static string ToCode(this AccessModifiers access)
            => access == AccessModifiers.None ? String.Empty : access.ToString().ToLowerInvariant();

        public static string ToCode(this ClassModifiers modifiers)
            => modifiers == ClassModifiers.Static ? "static partial" : "partial";

        public static string ToCode(this MethodModifiers modifiers)
            => modifiers == MethodModifiers.None ? String.Empty : modifiers.ToString().ToLowerInvariant();

        public static string ToCode(this FieldModifiers modifiers)
            => modifiers == FieldModifiers.None ? String.Empty : modifiers.ToString().ToLowerInvariant();

        public static string ToCode(this ClassKeyword keyword)
            => keyword.ToString().ToLowerInvariant();

        public static string ToCode(this BranchStatement branch)
            => $"{branch.ToString().ToLowerInvariant()} ";

        public static string ToCode(this AstExpression? expression)
        {
            if (expression == null)
                return String.Empty;

            var builder = new CsBuilder();
            new EmitExpression(builder).VisitExpression(expression);
            return builder.ToString();
        }

        public static string ToCode(this CSharp.Namespace ns)
        {
            var builder = new CsBuilder();
            builder.StartNamespace(ns.Name);

            foreach (var @using in ns.Usings)
            {
                builder.Using(@using);
            }

            foreach (var @class in ns.Classes)
            {
                @class.ToCode(builder);
            }

            builder.EndScope();
            return builder.ToString();
        }

        private static void ToCode(this CSharp.Class @class, CsBuilder builder)
        {
            builder.StartClass(@class.AccessModifiers, @class.ClassModifiers, @class.Keyword, @class.Name);

            foreach (var field in @class.Fields)
            {
                builder.StartField(field.AccessModifiers, field.FieldModifiers, field.TypeName, field.Name);
            }

            foreach (var property in @class.Properties)
            {
                builder.Property(property.AccessModifiers, property.TypeName, property.Name);
            }

            foreach (var method in @class.Methods)
            {
                var parameters = method.Parameters
                    .Select(p => (name: p.Name, type: p.TypeName))
                    .ToArray();
                builder.StartMethod(method.AccessModifiers, method.MethodModifiers, method.TypeName, method.Name, parameters);
                builder.AppendLine(method.Body.ToString());
                builder.EndScope();
            }

            foreach (var @enum in @class.Enums)
            {
                builder.StartEnum(@enum.AccessModifiers, @enum.Name, @enum.BaseTypeName);

                foreach (var option in @enum.Options)
                {
                    builder.Append($"{option.Name}");
                    if (!String.IsNullOrEmpty(option.Value))
                        builder.Append($" = {option.Value}");
                    builder.Append(",");
                }

                builder.EndScope();
            }

            foreach (var nested in @class.Classes)
            {
                nested.ToCode(builder);
            }

            builder.EndScope();
        }
    }
}
