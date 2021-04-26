﻿using System;
using System.Collections.Generic;

namespace Zsharp.AST
{
    public class AstTypeDefinitionIntrinsic : AstTypeDefinition,
        IAstTemplateSite
    {
        public AstTypeDefinitionIntrinsic(AstIdentifier identifier, Type systemType,
            AstTemplateParameterDefinition? templateParameter = null, bool isUnsigned = false)
            : base(identifier)
        {
            IsUnsigned = isUnsigned;
            SystemType = systemType;

            if (templateParameter != null)
                AddTemplateParameter(templateParameter);
        }

        public override bool IsIntrinsic => true;

        public override bool IsUnsigned { get; }

        public Type SystemType { get; private set; }

        public static readonly AstTypeDefinitionIntrinsic U8 = new(AstIdentifierIntrinsic.U8, typeof(Byte), isUnsigned: true);
        public static readonly AstTypeDefinitionIntrinsic U16 = new(AstIdentifierIntrinsic.U16, typeof(UInt16), isUnsigned: true);
        public static readonly AstTypeDefinitionIntrinsic U64 = new(AstIdentifierIntrinsic.U64, typeof(UInt64), isUnsigned: true);
        public static readonly AstTypeDefinitionIntrinsic U32 = new(AstIdentifierIntrinsic.U32, typeof(UInt32), isUnsigned: true);
        public static readonly AstTypeDefinitionIntrinsic I8 = new(AstIdentifierIntrinsic.I8, typeof(SByte));
        public static readonly AstTypeDefinitionIntrinsic I16 = new(AstIdentifierIntrinsic.I16, typeof(Int16));
        public static readonly AstTypeDefinitionIntrinsic I64 = new(AstIdentifierIntrinsic.I64, typeof(Int64));
        public static readonly AstTypeDefinitionIntrinsic I32 = new(AstIdentifierIntrinsic.I32, typeof(Int32));
        public static readonly AstTypeDefinitionIntrinsic F64 = new(AstIdentifierIntrinsic.F64, typeof(Double));
        public static readonly AstTypeDefinitionIntrinsic F32 = new(AstIdentifierIntrinsic.F32, typeof(Single));
        public static readonly AstTypeDefinitionIntrinsic Str = new(AstIdentifierIntrinsic.Str, typeof(String));
        public static readonly AstTypeDefinitionIntrinsic Bool = new(AstIdentifierIntrinsic.Bool, typeof(Boolean));
        public static readonly AstTypeDefinitionIntrinsic Void = new(AstIdentifierIntrinsic.Void, null);
        public static readonly AstTypeDefinitionIntrinsic Array = new(AstIdentifierIntrinsic.Array, typeof(Array), AstTemplateParameterDefinitionIntrinsic.T);

        public static AstTypeDefinitionIntrinsic? Lookup(string typeName)
        {
            return AstDotName.ToCanonical(typeName) switch
            {
                "U8" => U8,
                "U16" => U16,
                "U32" => U32,
                "U64" => U64,
                "I8" => I8,
                "I16" => I16,
                "I32" => I32,
                "I64" => I64,
                "F32" => F32,
                "F64" => F64,
                "Str" => Str,
                "Bool" => Bool,
                "Void" => Void,
                "Array" => Array,
                "Array%1" => Array,
                _ => null
            };
        }

        public static void AddAll(AstSymbolTable symbols)
        {
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.Bool);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.F64);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.F32);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.I16);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.I64);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.I32);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.I8);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.Str);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.U16);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.U64);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.U32);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.U8);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.Void);
            AddIntrinsicSymbol(symbols, AstTypeDefinitionIntrinsic.Array);
        }

        // true when type is a template definition
        public bool IsTemplate => _templateParameters.Count > 0;

        private readonly List<AstTemplateParameter> _templateParameters = new();
        public IEnumerable<AstTemplateParameter> TemplateParameters => _templateParameters;

        public void AddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (!TryAddTemplateParameter(templateParameter))
                throw new InvalidOperationException(
                    "TemplateParameter is already set or null.");
        }

        public bool TryAddTemplateParameter(AstTemplateParameter templateParameter)
        {
            if (templateParameter == null ||
                templateParameter is not AstTemplateParameterDefinition)
                return false;

            //Symbols.Add((AstTemplateParameterDefinition)templateParameter);
            _templateParameters.Add(templateParameter);

            Identifier.TemplateParameterCount = _templateParameters.Count;
            return true;
        }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }

        protected static void AddIntrinsicSymbol(AstSymbolTable symbols, AstTypeDefinitionIntrinsic type)
            => symbols.AddSymbol(type.Identifier!.CanonicalName, AstSymbolKind.Type, type);

        public override bool TrySetSymbol(AstSymbolEntry symbolEntry)
        {
            throw new InvalidOperationException(
                "Intrinsic Type Definitions are static and have no reference to the Symbol Table.");
        }
    }
}