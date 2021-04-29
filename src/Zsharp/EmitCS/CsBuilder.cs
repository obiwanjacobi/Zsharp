using System;
using System.IO;
using System.Linq;

namespace Zsharp.EmitCS
{
    internal sealed class CsBuilder
    {
        private readonly TextWriter _writer;

        public CsBuilder(int indent = 0)
        {
            Indent = indent;
            _writer = new StringWriter();
        }

        public int Indent { get; private set; }

        public override string ToString()
        {
            return _writer.ToString() ?? String.Empty;
        }

        public void Append(string codeText)
        {
            _writer.Write(codeText);
        }

        public void AppendLine(string codeText)
        {
            _writer.WriteLine(codeText);
        }

        /// <summary>
        /// using [<paramref name="alias"/> =] <paramref name="name"/>;
        /// </summary>
        /// <param name="name"></param>
        /// <param name="alias"></param>
        public void Using(string name, string? alias = null)
        {
            _writer.Write("using ");
            if (!String.IsNullOrEmpty(alias))
                _writer.Write($"{alias} = ");
            _writer.Write(name);
            EndLine();
        }

        /// <summary>
        /// namespace <paramref name="ns"/>
        /// {
        /// </summary>
        /// <param name="ns">Dotted name.</param>
        public void StartNamespace(string ns)
        {
            Indent = 0;
            _writer.WriteLine($"namespace {ns}");
            _writer.WriteLine("{");
            IncrementIndent();
        }

        /// <summary>
        /// <paramref name="access"/> <paramref name="modifiers"/> <paramref name="keyword"/> <paramref name="className"/> [: <paramref name="baseNames"/>]
        /// {
        /// </summary>
        /// <param name="access">Access modifiers</param>
        /// <param name="modifiers">class modifiers</param>
        /// <param name="keyword">class, record or struct</param>
        /// <param name="className">Class name</param>
        /// <param name="baseNames">Base class and/or interface names.</param>
        public void StartClass(AccessModifiers access, ClassModifiers modifiers, ClassKeyword keyword, string className, params string[]? baseNames)
        {
            WriteIndent();
            _writer.Write($"{access.ToCode()} {modifiers.ToCode()} {keyword.ToCode()} {className}");
            if (baseNames != null && baseNames.Length > 0)
                _writer.Write($" : {String.Join(", ", baseNames)}");
            _writer.WriteLine();
            WriteIndent();
            _writer.WriteLine("{");
            IncrementIndent();
        }

        /// <summary>
        /// <paramref name="access"/>> enum <paramref name="enumName"/> [: <paramref name="baseName"/>]
        /// {
        /// </summary>
        /// <param name="access">Access modifiers</param>
        /// <param name="enumName">Name of the enum</param>
        /// <param name="baseName">optional base type name</param>
        public void StartEnum(AccessModifiers access, string enumName, string? baseName)
        {
            WriteIndent();
            _writer.Write($"{access.ToCode()} enum {enumName}");
            if (!String.IsNullOrEmpty(baseName))
                _writer.Write($" : {baseName}");
            _writer.WriteLine();
            WriteIndent();
            _writer.WriteLine("{");
            IncrementIndent();
        }

        /// <summary>
        /// <paramref name="access"/> <paramref name="modifiers"/> <paramref name="retType"/> <paramref name="methodName"/> (<paramref name="parameters"/>)
        /// {
        /// </summary>
        /// <param name="access">Access modifiers</param>
        /// <param name="modifiers">Method modifiers</param>
        /// <param name="retType">Return typename</param>
        /// <param name="methodName">Name of method</param>
        /// <param name="parameters">Optional parameters.</param>
        public void StartMethod(AccessModifiers access, MethodModifiers modifiers, string retType, string methodName, params (string name, string type)[]? parameters)
        {
            WriteIndent();
            _writer.Write($"{access.ToCode()} {modifiers.ToCode()} {retType} {methodName}(");
            if (parameters != null && parameters.Length > 0)
            {
                _writer.Write(String.Join(", ", parameters.Select(p => $"{p.type} {p.name}")));
            }
            _writer.WriteLine(")");
            WriteIndent();
            _writer.WriteLine("{");
            IncrementIndent();
        }

        public void Property(AccessModifiers access, string typeName, string fieldName)
        {
            WriteIndent();
            _writer.Write($"{access.ToCode()} {typeName} {fieldName} {{ get; set; }}");
        }

        /// <summary>
        /// <paramref name="access"/> <paramref name="modifiers"/> <paramref name="typeName"/> <paramref name="fieldName"/>
        /// </summary>
        /// <param name="access">Access modifiers</param>
        /// <param name="modifiers">Field modifiers</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="typeName">Type of the field</param>
        public void StartField(AccessModifiers access, FieldModifiers modifiers, string typeName, string fieldName)
        {
            WriteIndent();
            _writer.Write($"{access.ToCode()} {modifiers.ToCode()} {typeName} {fieldName}");
        }

        /// <summary>
        /// <paramref name="typeName"/> <paramref name="variableName"/>
        /// </summary>
        /// <param name="typeName">Name of Type</param>
        /// <param name="variableName">Name of variable</param>
        public void StartVariable(string typeName, string variableName)
        {
            WriteIndent();
            _writer.Write($"{typeName} {variableName}");
        }

        /// <summary>
        /// <paramref name="branch"/>
        /// </summary>
        /// <param name="branch">Type of branch statement</param>
        public void StartBranch(BranchStatement branch)
        {
            WriteIndent();
            _writer.Write(branch.ToCode());
        }

        /// <summary>
        /// <paramref name="post"/>;
        /// </summary>
        /// <param name="post">optional text before ;</param>
        public void EndLine(string? post = null)
        {
            if (!String.IsNullOrEmpty(post))
                _writer.Write(post);

            _writer.WriteLine(";");
        }

        /// <summary>
        /// <paramref name="post"/> {
        /// </summary>
        /// <param name="post">optional text befor {</param>
        public void StartScope(string? post = null)
        {
            if (!String.IsNullOrEmpty(post))
                _writer.WriteLine(post);

            WriteIndent();
            _writer.WriteLine("{");
            IncrementIndent();
        }

        /// <summary>
        /// }
        /// </summary>
        public void EndScope()
        {
            DecrementIndent();
            WriteIndent();
            _writer.WriteLine("}");
        }

        public void WriteIndent()
        {
            _writer.Write(new String(' ', Indent));
        }

        private void IncrementIndent()
        {
            Indent += 4;
        }

        private void DecrementIndent()
        {
            if (Indent == 0)
                throw new InvalidOperationException("Unbalanced Indent.");

            Indent -= 4;
        }
    }

    internal enum AccessModifiers
    {
        None,
        Private,
        Internal,
        Protected,
        Public
    }

    internal enum ClassKeyword
    {
        Class,
        Record,
        Struct
    }

    internal enum ClassModifiers
    {
        None,
        Static,
    }

    internal enum MethodModifiers
    {
        None,
        Virtual,
        Override,
        Static
    }

    internal enum FieldModifiers
    {
        None,
        ReadOnly,
        Static
    }

    internal enum BranchStatement
    {
        Return,
        Continue,
        Break,
        If,
        Else
    }
}