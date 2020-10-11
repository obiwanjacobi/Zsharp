using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Zsharp.Dgml
{
    public enum Group
    {
        NotSet,
        Expanded,
        Collapsed,
    }

    public class Graph
    {
        public string? Title { get; set; }
        public List<Node> Nodes { get; } = new List<Node>();
        public List<Link> Links { get; } = new List<Link>();
        public List<Category> Categories { get; } = new List<Category>();

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

            //  <Nodes />
            writer.WriteStartElement("Nodes");
            //writer.WriteEndElement();
            foreach (var node in Nodes)
            {
                node.Write(writer);
            }
            writer.WriteEndElement(/*"Nodes"*/);

            //  <Links />
            writer.WriteStartElement("Links");
            //writer.WriteEndElement();
            foreach (var link in Links)
            {
                link.Write(writer);
            }
            writer.WriteEndElement(/*"Links"*/);

            //  <Categories />
            writer.WriteStartElement("Categories");
            //writer.WriteEndElement();
            foreach (var category in Categories)
            {
                category.Write(writer);
            }
            //writer.WriteEndElement(/*"Categories"*/);

            //  <Styles />

            // </DirectedGraph>
            writer.WriteEndElement(/*"DirectedGraph"*/);
            writer.WriteEndDocument();
        }
    }

    public class Node
    {
        public string? Id { get; set; }
        public string? Label { get; set; }
        public string? Category { get; set; }
        public string? Shape { get; set; }
        public string? Style { get; set; }
        public Group Group { get; set; }

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
                writer.WriteAttributeString("Group", Group == Group.Collapsed ? "Collapsed" : "Expanded");
            }
            writer.WriteEndElement();
        }
    }

    public class Link
    {
        public string? Source { get; set; }
        public string? Target { get; set; }
        public string? Label { get; set; }
        public string? Category { get; set; }

        internal void Write(XmlWriter writer)
        {
            writer.WriteStartElement("Link");
            writer.WriteAttributeString("Source", Source);
            writer.WriteAttributeString("Target", Target);
            writer.WriteAttributeString("Label", Label);
            writer.WriteAttributeString("Category", Category);
            writer.WriteEndElement();
        }
    }

    public class Category
    {
        public string? Id { get; set; }
        public string? Label { get; set; }
        public bool IsContainment { get; set; }

        internal void Write(XmlWriter writer)
        {
            writer.WriteStartElement("Category");
            writer.WriteAttributeString("Source", Id);
            writer.WriteAttributeString("Target", Label);
            if (IsContainment)
            {
                writer.WriteAttributeString("IsContainment", "True");
            }
            writer.WriteEndElement();
        }
    };
}
