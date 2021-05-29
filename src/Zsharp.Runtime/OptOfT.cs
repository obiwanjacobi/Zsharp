using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.Runtime
{
    public abstract class Opt
    {
        public const string Nothing = "Nothing";

        private readonly bool _hasValue;
        public bool HasValue => _hasValue;

        internal protected Opt(bool hasValue)
            => _hasValue = hasValue;
    }

    public sealed class Opt<T> : Opt, IEnumerable<T>
    {
        private readonly T _value;

        public Opt() : base(false) { }
        public Opt(T value) : base(true)
            => _value = value ?? throw new ArgumentNullException(nameof(value));
        public Opt(IEnumerable<T> valueOrEmpty) : base(valueOrEmpty.Any())
            => _value = valueOrEmpty.SingleOrDefault();

        // linq support
        public Opt<R> Select<R>(Func<T, R> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (HasValue)
                return new Opt<R>(selector(_value));

            return new Opt<R>();
        }

        // Map == Select
        public Opt<R> Map<R>(Func<T, R> selector)
            => Select<R>(selector);

        // (ternary) conditional operator works same as Match
        // val = optVal ? true : false
        public R Match<R>(R none, Func<T, R> some)
            => HasValue ? some(_value) : none;

        public override string ToString()
        {
            if (HasValue)
                return _value.ToString();
            return Nothing;
        }

        // operator overloads
        public static implicit operator Opt<T>(T value)
            => new(value);
        public static bool operator true(Opt<T> opt)
            => opt.HasValue;
        public static bool operator false(Opt<T> opt)
            => !opt.HasValue;

        public IEnumerator<T> GetEnumerator()
            => HasValue
            ? new SingleValueEnumerator<T>(_value)
            : Enumerable.Empty<T>().GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
