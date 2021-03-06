﻿using System.Collections.Generic;

namespace Zsharp.EmitCS.CSharp
{
    internal class Enum
    {
        public Enum(string name)
        {
            Name = name;
        }

        public AccessModifiers AccessModifiers { get; set; }

        public string Name { get; }

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
        public EnumOption(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public string? Value { get; set; }
    }
}
