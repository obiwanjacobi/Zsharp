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
        public Opt() : base(false) { }
        public Opt(T value) : base(true)
            => _value = value ?? throw new ArgumentNullException(nameof(value));
        public Opt(IEnumerable<T> valueOrEmpty) : base(valueOrEmpty.Any())
            => _value = valueOrEmpty.SingleOrDefault();

        // Nullable interface
        private readonly T _value;
        public T Value
        {
            get
            {
                if (HasValue)
                    return _value;
                throw new InvalidOperationException("The Option has no value.");
            }
        }

        public T GetValueOrDefault()
            => HasValue ? _value : default;
        public T GetValueOrDefault(T defaultValue)
            => HasValue ? _value : defaultValue;

        public bool TryGetValue(out T value)
        {
            if (HasValue)
            {
                value = _value;
                return true;
            }

            value = default;
            return false;
        }

        public Opt<T> Or(Opt<T> option)
            => HasValue ? this : option;

        // linq support
        public Opt<R> Select<R>(Func<T, R> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (HasValue)
                return new Opt<R>(selector(_value));

            return new Opt<R>();
        }

        public IEnumerable<R> SelectMany<R>(Func<T, IEnumerable<R>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (HasValue)
                return selector(_value);

            return new Opt<R>();
        }

        public Opt<T> Where(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (HasValue && predicate(_value))
                return this;

            return new Opt<T>();
        }

        // Map == Select
        public Opt<R> Map<R>(Func<T, R> selector)
            => Select<R>(selector);

        public Opt<R> Bind<R>(Func<T, Opt<R>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (HasValue)
                return selector(_value);

            return new Opt<R>();
        }

        // (ternary) conditional operator works same as Match
        // val = optVal ? true : false
        public R Match<R>(R none, Func<T, R> some)
            => HasValue ? some(_value) : none;

        public override string ToString()
            => HasValue ? _value.ToString() : Nothing;

        // operator overloads
        public static implicit operator Opt<T>(T value)
            => value != null ? new(value) : new();
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
