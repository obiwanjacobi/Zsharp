using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.AST
{
    public class AstDotName
    {
        public AstDotName(string identifier)
        {
            _parts = identifier.Split('.');
        }

        public int Count => _parts.Length;

        private readonly string[] _parts;

        public IEnumerable<string> Parts => _parts;

        public string ModuleName
        {
            get
            {
                if (_parts.Length > 1)
                {
                    return Join(0, _parts.Length - 1);
                }
                return _parts[0];
            }
        }

        public string Symbol
        {
            get
            {
                if (_parts.Length > 1)
                {
                    return _parts[^1];
                }
                return String.Empty;
            }
        }

        public override string ToString()
        {
            return Join(0, _parts.Length);
        }

        private string Join(int offset, int length)
        {
            return String.Join('.', _parts.Skip(offset).Take(length));
        }
    }
}
