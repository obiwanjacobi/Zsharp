using System;
using System.Collections.Generic;
using System.Linq;

namespace Zsharp.Runtime
{
    public abstract class Opt
    {
        public const string NothingText = "<Nothing>";

        public bool HasValue { get; protected set; }

        public static bool operator true(Opt opt)
            => opt.HasValue;
        public static bool operator false(Opt opt)
            => !opt.HasValue;
    }

    public sealed class Opt<T> : Opt,
        IEnumerable<T>, IEquatable<Opt<T>>, IEquatable<T>
    {
        public Opt() { }
        public Opt(T value)
        { 
            _value = value ?? throw new ArgumentNullException(nameof(value));
            HasValue = true;
        }
        public Opt(IEnumerable<T> valueOrEmpty)
        { 
            _value = valueOrEmpty.SingleOrDefault();
            HasValue = valueOrEmpty.Any();
        }

        // Nullable<T> interface
        private readonly T? _value;
        public T Value
        {
            get
            {
                if (HasValue)
                    return _value!;
                throw new InvalidOperationException($"The Opt<{typeof(T).Name}> has no Value.");
            }
        }

        public T? GetValueOrDefault()
            => _value;
        public T GetValueOrDefault(T defaultValue)
            => HasValue ? _value! : defaultValue;

        public bool TryGetValue(out T? value)
        {
            if (HasValue)
            {
                value = _value!;
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
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (HasValue)
                return new Opt<R>(selector(_value!));

            return Opt<R>.Nothing;
        }

        public IEnumerable<R> SelectMany<R>(Func<T, IEnumerable<R>> selector)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (HasValue)
                return selector(_value!);

            // Opt<T> implements IEnumerable<T>
            return Opt<R>.Nothing;
        }

        public Opt<T> Where(Func<T, bool> predicate)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            if (HasValue && predicate(_value!))
                return this;

            return Nothing;
        }

        // Map == Select
        public Opt<R> Map<R>(Func<T, R> selector)
            => Select(selector);

        public Opt<R> Bind<R>(Func<T, Opt<R>> selector)
        {
            if (selector is null)
                throw new ArgumentNullException(nameof(selector));

            if (HasValue)
                return selector(_value!);

            return Opt<R>.Nothing;
        }

        // (ternary) conditional operator works same as Match
        // val = optVal ? true : false
        public R? Match<R>(Func<T, R> some, R? none = default)
            => HasValue ? some(_value!) : none;

        public override string? ToString()
            => HasValue ? _value!.ToString() : NothingText;

        // operator overloads
        public static implicit operator Opt<T>(T value)
            => value is not null ? new(value) : new();
        public static explicit operator T(Opt<T> option)
            => option.Value;
        public static bool operator ==(Opt<T> option, Opt<T> value)
            => option.Equals(value);
        public static bool operator !=(Opt<T> option, Opt<T> value)
            => !option.Equals(value);

        public IEnumerator<T> GetEnumerator()
            => HasValue
            ? new SingleValueEnumerator<T>(_value!)
            : Enumerable.Empty<T>().GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool Equals(T? other)
            => HasValue && _value!.Equals(other);

        public bool Equals(Opt<T>? other)
        {
            if (other is not null)
            { 
                if(HasValue && other.HasValue)
                    return _value!.Equals(other.Value);
                
                // both nothing is equal
                return HasValue == other.HasValue;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return !HasValue;
            if (obj is Opt<T> option)
                return Equals(option);
            if (obj is T value)
                return Equals(value);
            return false;
        }

        public override int GetHashCode()
            => HasValue ? _value!.GetHashCode() : 0;

        public static readonly Opt<T> Nothing = new();
    }
}
