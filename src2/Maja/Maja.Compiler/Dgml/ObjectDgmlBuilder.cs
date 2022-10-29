using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maja.Dgml;

namespace Maja.Compiler.Dgml;

/// <summary>
/// Configure what properties to include for each type
/// -or- override callback methods/pass in delegates.
/// the object tree is walked based on the config - order is important.
/// and nodes are created for each object instance.
/// 
/// Maintain the current and last node (for links).
/// Have a way to specify a category for nodes (can trigger styles).
/// Add custom properties of the object to the nodes (tooltip).
/// </summary>
internal class ObjectDgmlBuilder
{
    public record PropertyConfiguration
    {
        public string Name { get; init; } = String.Empty;
        public bool Navigate { get; init; }
        public bool IsCollection { get; init; }
    }

    public record ObjectConfiguration
    {
        // names of properties on objects to include
        public IEnumerable<PropertyConfiguration> Properties { get; init; }
            = Enumerable.Empty<PropertyConfiguration>();

        public bool IncludeAllProperties { get; init; }

        public int MaxNavigationDepth { get; init; }

        public static readonly ObjectConfiguration Default = new()
        {
            MaxNavigationDepth = 5,
            IncludeAllProperties = true,
            //Properties = Enumerable.Empty<PropertyConfiguration>()
        };
    }

    private readonly DgmlBuilder _builder = new();
    private readonly ObjectConfiguration _config;

    public ObjectDgmlBuilder(ObjectConfiguration? config = null)
    {
        if (config == null)
            config = ObjectConfiguration.Default;

        _config = config;
    }

    public static void Save(object instance, string filePath = "object.dgml")
    {
        var builder = new ObjectDgmlBuilder();
        builder.WriteObject(instance);
        builder._builder.SaveAs(filePath);
    }

    public Node WriteObject(object instance, int depth = 0)
    {
        var type = instance.GetType();

        var name = GetObjectInstanceName(instance);
        var objNode = _builder.CreateNode(type.Name, name, type.Name);

        foreach (var prop in type.GetProperties())
        {
            if (CanWriteProperty(instance, prop))
            {
                var propNode = WriteProperty(instance, prop, depth);
                _ = _builder.CreateLink(objNode.Id, propNode.Id);
            }
        }

        return objNode;
    }

    private Node WriteProperty(object instance, PropertyInfo prop, int depth)
    {
        var propVal = prop.GetValue(instance);

        var type = prop.PropertyType.Name;
        var propNode = _builder.CreateNode(type, $"{prop.Name}={propVal}", type);

        if (propVal != null && CanNavigateProperty(instance, prop, depth))
        {
            var subNode = WriteObject(propVal, depth + 1);
            _ = _builder.CreateLink(propNode.Id, subNode.Id);
        }

        return propNode;
    }

    private bool CanWriteProperty(object instance, PropertyInfo prop)
    {
        try
        {
            _ = prop.GetValue(instance);
        }
        catch
        {
            return false;
        }

        if (_config.IncludeAllProperties)
            return true;

        return _config.Properties.Any(p => p.Name == prop.Name);
    }

    private bool CanNavigateProperty(object instance, PropertyInfo prop, int depth)
    {
        if (prop.PropertyType.Namespace.StartsWith("System"))
            return false;
        if (prop.DeclaringType.Namespace.StartsWith("System"))
            return false;

        if (_config.MaxNavigationDepth < depth)
            return false;

        if (_config.IncludeAllProperties)
            return true;

        var propCfg = _config.Properties.SingleOrDefault(p => p.Name == prop.Name);
        if (propCfg is not null)
            return propCfg.Navigate;

        return false;
    }

    private string GetObjectInstanceName(object instance)
    {
        // for now...
        return instance.ToString() ?? instance.GetType().Name;
    }
}
