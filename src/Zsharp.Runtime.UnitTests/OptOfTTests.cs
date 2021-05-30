using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Zsharp.Runtime.UnitTests
{
    [TestClass]
    public class OptOfTTests
    {
        [TestMethod]
        public void RT_Opt_RefType_NotNothing()
        {
            var opt = new Opt<string>("42");
            opt.ToString().Should().NotBe(Opt.NothingText);
        }

        [TestMethod]
        public void RT_Opt_RefType_Nothing()
        {
            var opt = new Opt<string>();
            opt.ToString().Should().Be(Opt.NothingText);
        }

        [TestMethod]
        public void RT_Opt_ValueType_NotNothing()
        {
            var opt = new Opt<int>(42);
            opt.ToString().Should().NotBe(Opt.NothingText);
        }

        [TestMethod]
        public void RT_Opt_ValueType_Nothing()
        {
            var opt = new Opt<int>();
            opt.ToString().Should().Be(Opt.NothingText);
        }

        [TestMethod]
        public void RT_Opt_RefType_True()
        {
            var opt = new Opt<string>("42");
            opt.HasValue.Should().BeTrue();
            var hasValue = opt ? true : false;
            hasValue.Should().BeTrue();
        }

        [TestMethod]
        public void RT_Opt_RefType_False()
        {
            var opt = new Opt<string>();
            opt.HasValue.Should().BeFalse();
            var hasValue = opt ? true : false;
            hasValue.Should().BeFalse();
        }

        [TestMethod]
        public void RT_Opt_ValueType_True()
        {
            var opt = new Opt<int>(42);
            opt.HasValue.Should().BeTrue();
            var hasValue = opt ? true : false;
            hasValue.Should().BeTrue();
        }

        [TestMethod]
        public void RT_Opt_ValueType_False()
        {
            var opt = new Opt<int>();
            opt.HasValue.Should().BeFalse();
            var hasValue = opt ? true : false;
            hasValue.Should().BeFalse();
        }

        [TestMethod]
        public void RT_Opt_NullableValueType_Value()
        {
            int? i = 42;
            var opt = new Opt<int?>(i);
            var val = from j in opt select j;
            // no way to unwrap a Nullable<T> :-(
            val.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void RT_Opt_Composition()
        {
            var optTxt = new Opt<string>("hello");
            var optNum = new Opt<int>(42);

            var maybeEnum = from t in optTxt
                            from n in optNum
                            select (text: t, number: n);

            var maybe = new Opt<(string text, int number)>(maybeEnum);
            maybe.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void RT_Opt_Composition_Incomplete()
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
