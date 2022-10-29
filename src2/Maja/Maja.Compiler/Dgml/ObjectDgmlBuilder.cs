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
    public class ObjecConfiguration
    {
        // names of properties on objects to include
        //public IEnumerable<string> Properties { get; set; }
    }
}
