using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zsharp.AST
{
    [DebuggerDisplay("{CanonicalFullName}")]
    public class AstSymbolName
    {
        private AstSymbolName() { }
        public AstSymbolName(AstSymbolName nameToCopy)
        {
            _native = new AstName(nameToCopy.NativeName);
            _canonical = new AstName(nameToCopy.CanonicalName);
        }
        public AstSymbolName(AstName nativeName)
        {
            _native = nativeName ?? throw new ArgumentNullException(nameof(nativeName));
            if (_native.Kind == AstNameKind.Canonical)
                throw new ArgumentException("Parameter must represent a non-canonical symbol name.", nameof(nativeName));

            _canonical = nativeName.ToCanonical();
        }

        private AstName? _native;
        public AstName NativeName
            => _native ?? throw new InvalidOperationException("NativeName was not set.");

        private AstName? _canonical;
        public AstName CanonicalName
            => _canonical ?? throw new InvalidOperationException("CanonicalName was not set.");

        public string Postfix
        {
            get {  return _native!.Postfix; }
            set
            { 
                _native!.Postfix = value;
                _canonical!.Postfix = value;
            }
        }

        /// <summary>For template/generic definitions: MyType%1</summary>
        public void SetParameterCounts(int templateParameterCount, int genericParameterCount)
        { 
            var postfix = $"{AstName.TemplateDelimiter}{templateParameterCount + genericParameterCount}";
            NativeName.Postfix = postfix;
            CanonicalName.Postfix = postfix;
        }

        /// <summary>For template/generic references: MyType<T></summary>
        public void AddTemplateParameter(string? name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                var postfix = $"{AstName.ParameterDelimiter}{name}";
                NativeName.Postfix += postfix;
                CanonicalName.Postfix += postfix;
            }
        }

        /// <summary>Convert symbolName to a canonical format.</summary>
        public static string ToCanonical(string symbolName)
            => Parse(symbolName, AstNameKind.Local).CanonicalName.FullName;

        /// <summary>Parse a local source code or external symbol name.</summary>
        public static AstSymbolName Parse(string symbolName, AstNameKind nameKind = AstNameKind.Local)
        {
            var native = AstName.ParseFullName(symbolName, nameKind);
            return new AstSymbolName(native);
        }
    }
}
