using System;
using System.Linq;
using System.Text;

namespace Maja.Compiler.EmitCS.CSharp;

internal sealed class CSharpSerializer
{
    private readonly CSharpWriter _writer;

    public CSharpSerializer(CSharpWriter writer)
    {
        _writer = writer;
    }

    private StringBuilder Tab()
        => _writer.Tab();

    private void Using(string usingName)
    {
        Tab()
            .Append("using ")
            .Append(usingName)
            ;
        _writer.EndOfLine();
    }

    public void Write(Namespace @namespace)
    {
        bool hasNamespace = !String.IsNullOrEmpty(@namespace.Name);

        if (hasNamespace)
        {
            Tab().Append("namespace ").AppendLine(@namespace.Name);
            _writer.OpenScope();
        }

        foreach (var use in @namespace.Usings)
        {
            Using(use);
        }

        foreach (var @enum in @namespace.Enums)
        {
            Write(@enum);
        }

        foreach (var type in @namespace.Types)
        {
            Write(type);
        }

        if (hasNamespace)
            _writer.CloseScope();
    }

    public void Write(Enum @enum)
    {
        Tab()
            .Append(@enum.AccessModifiers.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append("enum")
            .Append(CSharpWriter.SpaceChar)
            .Append(@enum.Name);

        if (@enum.BaseTypeName is not null)
        {
            _writer.Write(" : ")
                .Write(@enum.BaseTypeName);
        }
        _writer.Newline();
        _writer.OpenScope();

        foreach (var option in @enum.Options)
        {
            Tab().Append(option.Name);

            if (option.Value is not null)
                _writer.Assignment().Write(option.Value);

            _writer.WriteComma().Newline();
        }

        _writer.CloseScope();
    }

    public void Write(CSharp.Type type)
    {
        Tab()
            .Append(type.AccessModifiers.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append(type.TypeModifiers.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append(type.Keyword.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append(type.Name)
            ;

        if (!String.IsNullOrEmpty(type.BaseTypeName))
            _writer.Write(" : ").Write(type.BaseTypeName);

        _writer.Newline();
        _writer.OpenScope();

        foreach (var fld in type.Fields)
        {
            Write(fld);
        }

        foreach (var prop in type.Properties)
        {
            Write(prop);
        }

        foreach (var method in type.Methods)
        {
            Write(method);
        }

        foreach (var subType in type.Types)
        {
            Write(subType);
        }

        foreach (var enumType in type.Enums)
        {
            Write(enumType);
        }

        _writer.CloseScope();
    }

    public void Write(Method method)
    {
        Tab()
            .Append(method.AccessModifiers.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append(method.MethodModifiers.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append(method.TypeName)
            .Append(CSharpWriter.SpaceChar)
            .Append(method.Name);

        int i = 0;
        if (method.TypeParameters.Any())
        {
            _writer.Write('<');
            foreach (var typeParam in method.TypeParameters)
            {
                if (i > 0) _writer.WriteComma();
                _writer.Write(typeParam.TypeName);
                i++;
            }
            _writer.Write('>');
        }

        i = 0;
        _writer.Write('(');
        foreach (var param in method.Parameters)
        {
            if (i > 0) _writer.WriteComma();
            Write(param);
            i++;
        }
        _writer.Write(')');
        _writer.Newline();

        _writer.OpenScope();
        method.Body.Render(_writer);
        _writer.Newline();
        _writer.CloseScope();
    }

    public void Write(Parameter parameter)
    {
        _writer
            .Write(parameter.TypeName)
            .Write(CSharpWriter.SpaceChar)
            .Write(parameter.Name);
    }

    public void Write(Property prop)
    {
        Tab()
            .Append(prop.AccessModifiers.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append(prop.FieldModifiers.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append(prop.TypeName)
            .Append(CSharpWriter.SpaceChar)
            .Append(prop.Name)
            .Append(CSharpWriter.SpaceChar)
            .Append(CSharpWriter.OpenScopeChar)
            .Append(" get; set; ")
            .Append(CSharpWriter.CloseScopeChar)
            ;

        if (!String.IsNullOrEmpty(prop.InitialValue))
        {
            _writer
                .Write(" = ")
                .Write(prop.InitialValue)
                .Write(CSharpWriter.SemiColonChar)
                ;
        }

        _writer.Newline();
    }

    public void Write(Field field)
    {
        Tab()
            .Append(field.AccessModifiers.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append(field.FieldModifiers.ToCode())
            .Append(CSharpWriter.SpaceChar)
            .Append(field.TypeName)
            .Append(CSharpWriter.SpaceChar)
            .Append(field.Name);

        if (!String.IsNullOrEmpty(field.InitialValue))
        {
            _writer
                .Assignment()
                .Write(field.InitialValue)
                ;
        }

        _writer.EndOfLine();
    }
}
