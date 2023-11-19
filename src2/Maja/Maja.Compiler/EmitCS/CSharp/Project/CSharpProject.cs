using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Maja.Compiler.EmmitCS.CSharp.Project;

internal sealed class CSharpProject
{
    internal CSharpProject()
    {
        TargetFrameworkMoniker = "net7.0";
        TargetPath = String.Empty;
    }

    public SdkType Sdk { get; set; }

    public bool Executable { get; set; }

    public string TargetFrameworkMoniker { get; set; }

    public string TargetPath { get; internal set; }

    private readonly List<string> _references = new();
    public void AddReference(string assemblyName)
    {
        if (!_references.Contains(assemblyName))
            _references.Add(assemblyName);
    }

    private readonly List<KeyValuePair<string, string>> _packages = new();
    public void AddPackage(string packageName, string version)
    {
        _packages.Add(KeyValuePair.Create(packageName, version));
    }

    public void SaveAs(string filePath)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            OmitXmlDeclaration = true
        };
        using var writer = XmlWriter.Create(filePath, settings);
        writer.WriteStartDocument();
        // <Project>
        writer.WriteStartElement("Project");
        writer.WriteAttributeString("Sdk", GetSdkValue());

        // <PropertyGroup>
        writer.WriteStartElement("PropertyGroup");
        if (Executable)
            writer.WriteElementString("OutputType", "Exe");
        writer.WriteElementString("TargetFramework", TargetFrameworkMoniker);
        writer.WriteElementString("Nullable", "annotations");
        writer.WriteEndElement();

        if (_references.Count > 0)
        {
            writer.WriteStartElement("ItemGroup");
            foreach (var reference in _references)
            {
                writer.WriteStartElement("Reference");
                writer.WriteAttributeString("Include", Path.GetFileNameWithoutExtension(reference));
                var path = Path.GetDirectoryName(reference);
                if (!String.IsNullOrEmpty(path))
                    writer.WriteElementString("HintPath", reference);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        if (_packages.Count > 0)
        {
            writer.WriteStartElement("ItemGroup");
            foreach (var package in _packages)
            {
                writer.WriteStartElement("PackageReference");
                writer.WriteAttributeString("Include", package.Key);
                writer.WriteAttributeString("Version", package.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        // </Project>
        writer.WriteEndElement();
        writer.WriteEndDocument();
    }

    private string GetSdkValue()
    {
        const string baseSdk = "Microsoft.NET.Sdk";

        return Sdk switch
        {
            SdkType.Default => baseSdk,
            _ => $"{baseSdk}.{Sdk}"
        };
    }
}

internal enum SdkType
{
    Default,
    Web,
    Razor,
    Worker,
    WindowsDesktop
}