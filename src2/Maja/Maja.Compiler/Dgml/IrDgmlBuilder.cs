using Maja.Compiler.IR;

namespace Maja.Dgml;

internal sealed class IrDgmlBuilder
{
    private IrDgmlBuilder()
    { }

    private static ObjectDgmlBuilder.ObjectConfiguration _config = new()
    {
        MaxNavigationDepth = 8,
        IncludeAllProperties = true,
        IterateCollections = true,
        Properties = new[]
        {
            new ObjectDgmlBuilder.PropertyConfiguration
            {
                Name = "Syntax",
                Exclude = true
            },
            new ObjectDgmlBuilder.PropertyConfiguration
            {
                Name = "Parent",
                Exclude = true
            }
        }
    };

    public static void Save(IrNode node, string filePath = "interrep.dgml")
    {
        ObjectDgmlBuilder.Save(_config, node, filePath);
    }
}
