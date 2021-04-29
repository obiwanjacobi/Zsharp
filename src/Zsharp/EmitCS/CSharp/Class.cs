using System.Collections.Generic;

namespace Zsharp.EmitCS.CSharp
{


    internal class Class
    {
        public Class(string name, ClassKeyword keyword)
        {
            Name = name;
            Keyword = keyword;
        }

        public string Name { get; }

        public string? BaseTypeName { get; set; }

        public ClassKeyword Keyword { get; }

        public AccessModifiers AccessModifiers { get; set; }

        public ClassModifiers ClassModifiers { get; set; }

        private readonly List<Field> _fields = new();
        public IEnumerable<Field> Fields => _fields;

        public void AddField(Field field)
        {
            _fields.Add(field);
        }

        private readonly List<Property> _properties = new();
        public IEnumerable<Property> Properties => _properties;

        public void AddProperty(Property property)
        {
            _properties.Add(property);
        }

        private readonly List<Method> _methods = new();
        public IEnumerable<Method> Methods => _methods;

        public void AddMethod(Method method)
        {
            _methods.Add(method);
        }

        private readonly List<Enum> _enums = new();
        public IEnumerable<Enum> Enums => _enums;

        public void AddEnum(Enum @enum)
        {
            _enums.Add(@enum);
        }

        private readonly List<Class> _classes = new();
        public IEnumerable<Class> Classes => _classes;

        public void AddClass(Class @class)
        {
            _classes.Add(@class);
        }
    }
}
