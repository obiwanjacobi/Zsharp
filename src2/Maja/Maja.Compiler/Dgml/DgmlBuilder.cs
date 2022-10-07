using System;
using System.IO;

namespace Maja.Dgml;

public class DgmlBuilder
{
    protected const string ContainsCategory = "Contains";
    protected const Group DefaultGroup = Group.Collapsed;

    private readonly Graph _graph;
    private int _id;

    public DgmlBuilder(string? title = null)
    {
        _graph = new();
        _graph.Title = title;
    }

    public virtual void CreateCommon()
    {
        var category = CreateCategory(ContainsCategory, ContainsCategory);
        category.IsContainment = true;
    }

    public void Serialize(Stream output)
    {
        _graph.Serialize(output);
    }

    public void SaveAs(string filePath)
    {
        filePath = EnsureUniqueName(filePath);
        using var stream = File.Create(filePath);
        Serialize(stream);
    }

    private static string EnsureUniqueName(string filePath)
    {
        while (File.Exists(filePath))
        {
            var name = Path.GetFileNameWithoutExtension(filePath);
            int i = name.Length - 1;
            // find digit position
            while (i > 0 && Char.IsDigit(name[i])) i--;

            i++;
            string digits = "1";
            if (i < name.Length)
            { 
                var number = int.Parse(name[i..]);
                number++;
                digits = number.ToString();
            }

            var newName = String.Concat(name.AsSpan(0, i), digits);
            filePath = filePath.Replace(name, newName);
        }
        return filePath;
    }

    public Node CreateNode(string id, string label, string? typeName = null)
    {
        var node = new Node
        {
            Id = id + NextId().ToString(),
            Label = label
        };
        if (typeName is not null)
        {
            node.TypeName = typeName;
        }

        _graph.Nodes.Add(node);
        return node;
    }

    public Link CreateLink(string sourceId, string targetId, string? category = null)
    {
        var link = new Link
        {
            Source = sourceId,
            Target = targetId
        };
        if (category is not null)
        {
            link.Category = category;
        }

        _graph.Links.Add(link);
        return link;
    }

    public Category CreateCategory(string id, string label)
    {
        var category = new Category
        {
            Id = id + NextId().ToString(),
            Label = label
        };

        _graph.Categories.Add(category);
        return category;
    }

    public Link? FindLink(string sourceId, string targetId)
    {
        foreach (var l in _graph.Links)
        {
            if (l.Source == sourceId && l.Target == targetId)
            {
                return l;
            }
        }

        return null;
    }

    private int NextId() => ++_id;
}
