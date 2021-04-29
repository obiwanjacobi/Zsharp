using System.Collections.Generic;

namespace Zsharp.EmitCS.CSharp
{
    internal class Enum
    {
        public AccessModifiers AccessModifiers { get; set; }

        public string Name { get; set; }

        public string? BaseTypeName { get; set; }

        private readonly List<EnumOption> _options = new();
        public IEnumerable<EnumOption> Options => _options;

        public void AddOption(EnumOption option)
        {
            _options.Add(option);
        }
    }

    internal class EnumOption
    {
        public string Name { get; set; }
        public string? Value { get; set; }
    }
}
