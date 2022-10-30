using System;
using System.Collections;
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
        // name to identify the property
        public string Name { get; init; } = String.Empty;
        // skip property if true
        public bool Exclude { get; init; }
        // continue on property values if true
        public bool Navigate { get; init; }
    }

    public record ObjectConfiguration
    {
        // names of properties on objects to include
        public IEnumerable<PropertyConfiguration> Properties { get; init; }
            = Enumerable.Empty<PropertyConfiguration>();

        // use all properties on the object.
        // property config can still exclude some
        public bool IncludeAllProperties { get; init; }
        // include items in collections if true
        public bool IterateCollections { get; init; }
        // How many nodes deep should the dgml display
        public int MaxNavigationDepth { get; init; }

        // basic default config used if no config is specified
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
        => Save(null, instance, filePath);

    public static void Save(ObjectConfiguration? config, object instance, string filePath = "object.dgml")
    {
        var builder = new ObjectDgmlBuilder(config);
        builder.WriteObject(instance);
        builder._builder.SaveAs(filePath);
    }

    public Node WriteObject(object instance, int depth = 0)
    {
        var type = instance.GetType();

        var name = GetObjectInstanceName(instance);
        var objNode = _builder.CreateNode(type.Name, name, type.Name);

        if (instance is IEnumerable collection)
        {
            foreach (var item in collection)
            {
                var childNode = WriteObject(item);
                _ = _builder.CreateLink(objNode.Id, childNode.Id);
            }
        }
        else
        {
            foreach (var prop in type.GetProperties())
            {
                if (CanWriteProperty(instance, prop))
                {
                    var propNode = WriteProperty(instance, prop, depth);
                    _ = _builder.CreateLink(objNode.Id, propNode.Id);
                }
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

        var propCfg = _config.Properties.SingleOrDefault(p => p.Name == prop.Name);
        if (propCfg is not null)
            return !propCfg.Exclude;

        return _config.IncludeAllProperties;
    }

    private bool CanNavigateProperty(object instance, PropertyInfo prop, int depth)
    {
        if (prop.PropertyType.Name == "String")
            return false;

        if (_config.IterateCollections &&
            prop.PropertyType.IsAssignableTo(typeof(IEnumerable)))
            return true;

        if (prop.PropertyType.Namespace?.StartsWith("System") ?? false)
            return false;
        if (prop.PropertyType.Namespace?.StartsWith("Microsoft") ?? false)
            return false;

        if (_config.MaxNavigationDepth < depth)
            return false;

        var propCfg = _config.Properties.SingleOrDefault(p => p.Name == prop.Name);
        if (propCfg is not null)
            return !propCfg.Exclude && propCfg.Navigate;

        return _config.IncludeAllProperties;
    }

    private string GetObjectInstanceName(object instance)
    {
        // for now...
        return instance.ToString() ?? instance.GetType().Name;
    }
}
