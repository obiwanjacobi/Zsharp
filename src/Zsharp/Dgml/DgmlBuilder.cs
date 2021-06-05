using System.IO;

namespace Zsharp.Dgml
{
    public abstract class DgmlBuilder
    {
        protected const string ContainsCategory = "Contains";
        protected const Group DefaultGroup = Group.Collapsed;

        private readonly Graph _graph;
        private int _id;

        protected DgmlBuilder()
        {
            _graph = new Graph();
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
            using var stream = File.Create(filePath);
            Serialize(stream);
        }

        protected Node CreateNode(string id, string label, string? typeName = null)
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

        protected Link CreateLink(string sourceId, string targetId, string? category = null)
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

        protected Category CreateCategory(string id, string label)
        {
            var category = new Category
            {
                Id = id + NextId().ToString(),
                Label = label
            };

            _graph.Categories.Add(category);
            return category;
        }

        protected Link? FindLink(string sourceId, string targetId)
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
}
