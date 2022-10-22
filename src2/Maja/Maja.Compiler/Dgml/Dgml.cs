using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Maja.Dgml;

public class Graph
{
    public string? Title { get; set; }
    public string? Background { get; set; }

    public List<Node> Nodes { get; } = new List<Node>();
    public List<Link> Links { get; } = new List<Link>();
    public List<Category> Categories { get; } = new List<Category>();
    public List<Style> Styles { get; } = new List<Style>();

    public void Serialize(Stream output)
    {
        using var writer = XmlWriter.Create(output);
        Write(writer);
    }

    internal void Write(XmlWriter writer)
    {
        writer.WriteStartDocument();

        // <DirectedGraph xmlns="http://schemas.microsoft.com/vs/2009/dgml">
        writer.WriteStartElement("DirectedGraph", "http://schemas.microsoft.com/vs/2009/dgml");
        if (!String.IsNullOrWhiteSpace(Background))
            writer.WriteAttributeString("Background", Background);

        //  <Nodes />
        writer.WriteStartElement("Nodes");
        foreach (var node in Nodes)
        {
            node.Write(writer);
        }
        writer.WriteEndElement(/*"Nodes"*/);

        //  <Links />
        writer.WriteStartElement("Links");
        foreach (var link in Links)
        {
            link.Write(writer);
        }
        writer.WriteEndElement(/*"Links"*/);

        //  <Categories />
        writer.WriteStartElement("Categories");
        foreach (var category in Categories)
        {
            category.Write(writer);
        }
        writer.WriteEndElement(/*"Categories"*/);

        //  <Styles />
        writer.WriteStartElement("Styles");
        foreach (var style in Styles)
        {
            style.Write(writer);
        }
        writer.WriteEndElement(/*"Styles"*/);

        // </DirectedGraph>
        writer.WriteEndElement(/*"DirectedGraph"*/);
        writer.WriteEndDocument();
    }
}

public class Font
{
    public string? Family { get; set; }
    public string? Weight { get; set; }
    public string? Size { get; set; }
    /// <summary>
    /// Italic, Bold
    /// </summary>
    public string? Style { get; set; }

}

public enum NodeStyle
{
    Plain,
    Glass
}

public enum Group
{
    NotSet,
    Expanded,
    Collapsed,
}

public class Node
{
    public string Id { get; init; } = String.Empty;
    public string? Label { get; set; }
    public string? Category { get; set; }
    public Group Group { get; set; }

    public NodeStyle Style { get; set; }

    public string? Background { get; set; }
    public string? Foreground { get; set; }
    public string? Stroke { get; set; }
    public string? StrokeThickness { get; set; }

    public Font? Font { get; set; }

    // extensions
    public string? TypeName { get; set; }

    internal void Write(XmlWriter writer)
    {
        writer.WriteStartElement("Node");
        writer.WriteAttributeString("Id", Id);
        if (!String.IsNullOrEmpty(TypeName))
        {
            writer.WriteAttributeString("Label", "[" + TypeName + "] " + Label);
        }
        else
        {
            writer.WriteAttributeString("Label", Label);
        }
        writer.WriteAttributeString("Category", Category);
        if (Group != Group.NotSet)
        {
            writer.WriteAttributeString("Group", Group.ToString());
        }
        writer.WriteAttributeString("Style", Style.ToString());

        if (!String.IsNullOrWhiteSpace(Background))
            writer.WriteAttributeString("Background", Background);
        if (!String.IsNullOrWhiteSpace(Foreground))
            writer.WriteAttributeString("Foreground", Foreground);
        if (!String.IsNullOrWhiteSpace(Stroke))
            writer.WriteAttributeString("Stroke", Stroke);
        if (!String.IsNullOrWhiteSpace(StrokeThickness))
            writer.WriteAttributeString("StrokeThickness", StrokeThickness);

        if (Font is not null)
        {
            if (!String.IsNullOrWhiteSpace(Font.Family))
                writer.WriteAttributeString("FontFamily", Font.Family);
            if (!String.IsNullOrWhiteSpace(Font.Size))
                writer.WriteAttributeString("FontSize", Font.Size);
            if (!String.IsNullOrWhiteSpace(Font.Style))
                writer.WriteAttributeString("FontStyle", Font.Style);
            if (!String.IsNullOrWhiteSpace(Font.Weight))
                writer.WriteAttributeString("FontWeight", Font.Weight);
        }

        writer.WriteEndElement();
    }
}

public class Link
{
    public string Source { get; init; } = String.Empty;
    public string Target { get; init; } = String.Empty;
    public string? Label { get; set; }
    public string? Category { get; set; }

    public string? Stroke { get; set; }
    public string? StrokeThickness { get; set; }
    public int[]? StrokeDashArray { get; set; }


    internal void Write(XmlWriter writer)
    {
        writer.WriteStartElement("Link");
        writer.WriteAttributeString("Source", Source);
        writer.WriteAttributeString("Target", Target);
        writer.WriteAttributeString("Label", Label);
        writer.WriteAttributeString("Category", Category);

        if (!String.IsNullOrWhiteSpace(Stroke))
            writer.WriteAttributeString("Stroke", Stroke);
        if (!String.IsNullOrWhiteSpace(StrokeThickness))
            writer.WriteAttributeString("StrokeThickness", StrokeThickness);
        if (StrokeDashArray?.Length > 0)
            writer.WriteAttributeString("StrokeDashArray",
                String.Join(",", StrokeDashArray.Select(n => n.ToString())));

        writer.WriteEndElement();
    }
}

public class Category
{
    public string Id { get; init; } = String.Empty;
    public string? Label { get; set; }
    public bool IsContainment { get; set; }

    internal void Write(XmlWriter writer)
    {
        writer.WriteStartElement("Category");
        writer.WriteAttributeString("Id", Id);
        writer.WriteAttributeString("Label", Label);
        if (IsContainment)
        {
            writer.WriteAttributeString("IsContainment", "True");
        }
        writer.WriteEndElement();
    }
}

public enum StyleTargetType
{
    Node, Link, Graph
}

public class Style
{
    public StyleTargetType TargetType { get; set; }
    // Name in Legend box
    public string? GroupLabel { get; set; }
    // Name in style picker box
    public string? ValueLabel { get; set; }

    ///<summary>
    ///<Expression> ::= <BinaryExpression> | \<UnaryExpression> | "("<Expression>")" | <MemberBindings> | <Literal> | \<Number>
    ///<BinaryExpression> ::= <Expression> <Operator> <Expression>
    ///<UnaryExpression> ::= "!" <Expression> | "+" <Expression> | "-" <Expression>
    ///<Operator> ::= "<" | "<=" | "=" | ">=" | ">" | "!=" | "or" | "and" | "+" | "*" | "/" | "-"
    ///<MemberBindings> ::= <MemberBindings> | <MemberBinding> "." <MemberBinding>
    ///<MemberBinding> ::= <MethodCall> | <PropertyGet>
    ///<MethodCall> ::= <Identifier> "(" <MethodArgs> ")"
    ///<PropertyGet> ::= <Identifier>
    ///<MethodArgs> ::= <Expression> | <Expression> "," <MethodArgs> | <empty>
    ///<Identifier> ::= [^. ]*
    ///<Literal> ::= single or double-quoted string literal
    ///<Number> ::= string of digits with optional decimal point
    /// </summary>
    public string? Condition { get; set; }
    /// <summary>
    /// Makes Condition: HasCategory('<category>')
    /// </summary>
    /// <param name="category">Id of the Category</param>
    public void HasCategory(string category)
        => Condition = $"HasCategory('{category}')";

    // property=value
    public Dictionary<String, String> Setters { get; } = new Dictionary<string, string>();

    internal void Write(XmlWriter writer)
    {
        writer.WriteStartElement("Style");
        writer.WriteAttributeString("TargetType", TargetType.ToString());
        if (!String.IsNullOrWhiteSpace(GroupLabel))
            writer.WriteAttributeString("GroupLabel", GroupLabel);
        if (!String.IsNullOrWhiteSpace(ValueLabel))
            writer.WriteAttributeString("ValueLabel", ValueLabel);

        if (!String.IsNullOrWhiteSpace(Condition))
        {
            writer.WriteStartElement("Condition");
            writer.WriteAttributeString("Expression", Condition);
            writer.WriteEndElement();
        }

        foreach (var setter in Setters)
        {
            writer.WriteStartElement("Setter");
            writer.WriteAttributeString("Property", setter.Key);
            writer.WriteAttributeString("Value", setter.Value);
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
    }
}