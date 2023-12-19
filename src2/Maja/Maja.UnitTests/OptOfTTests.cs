using FluentAssertions;
using System.Linq;
using Xunit;

namespace Maja.UnitTests
{
    public class OptOfTTests
    {
        [Fact]
        public void Opt_RefType_NotNothing()
        {
            var opt = new Opt<string>("42");
            opt.ToString().Should().NotBe(Opt.NothingText);
        }

        [Fact]
        public void Opt_RefType_Nothing()
        {
            var opt = new Opt<string>();
            opt.ToString().Should().Be(Opt.NothingText);
        }

        [Fact]
        public void Opt_ValueType_NotNothing()
        {
            var opt = new Opt<int>(42);
            opt.ToString().Should().NotBe(Opt.NothingText);
        }

        [Fact]
        public void Opt_ValueType_Nothing()
        {
            var opt = new Opt<int>();
            opt.ToString().Should().Be(Opt.NothingText);
        }

        [Fact]
        public void Opt_RefType_True()
        {
            var opt = new Opt<string>("42");
            opt.HasValue.Should().BeTrue();
            var hasValue = opt ? true : false;
            hasValue.Should().BeTrue();
        }

        [Fact]
        public void Opt_RefType_False()
        {
            var opt = new Opt<string>();
            opt.HasValue.Should().BeFalse();
            var hasValue = opt ? true : false;
            hasValue.Should().BeFalse();
        }

        [Fact]
        public void Opt_ValueType_True()
        {
            var opt = new Opt<int>(42);
            opt.HasValue.Should().BeTrue();
            var hasValue = opt ? true : false;
            hasValue.Should().BeTrue();
        }

        [Fact]
        public void Opt_ValueType_False()
        {
            var opt = new Opt<int>();
            opt.HasValue.Should().BeFalse();
            var hasValue = opt ? true : false;
            hasValue.Should().BeFalse();
        }

        [Fact]
        public void Opt_NullableValueType_Value()
        {
            int? i = 42;
            var opt = new Opt<int?>(i);
            var val = from j in opt select j;
            // no way to unwrap a Nullable<T> :-(
            val.HasValue.Should().BeTrue();
        }

        [Fact]
        public void Opt_Composition()
        {
            var optTxt = new Opt<string>("hello");
            var optNum = new Opt<int>(42);

            var maybeEnum = from t in optTxt
                            from n in optNum
                            select (text: t, number: n);

            var maybe = new Opt<(string text, int number)>(maybeEnum);
            maybe.HasValue.Should().BeTrue();
        }

        [Fact]
        public void Opt_Composition_Incomplete()
        {
            var optTxt = new Opt<string>("hello");
            var optNum = new Opt<int>();

            var maybeEnum = from t in optTxt
                            from n in optNum
                            select (text: t, number: n);

            var maybe = new Opt<(string text, int number)>(maybeEnum);
            maybe.HasValue.Should().BeFalse();
        }
    }
}
