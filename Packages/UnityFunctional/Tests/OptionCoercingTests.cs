using FluentAssertions;
using NUnit.Framework;

namespace Bravasoft.Functional.Tests
{

    public class OptionCoercingTests
    {
        [Test]
        public void OptionCoercingTests_Some()
        {
            Option<int> option = 1;

            option.IsSome.Should().BeTrue();
        }

        [Test]
        public void OptionCoercingTests_None()
        {
            Option<int> none = Option<int>.None;

            Assert.IsTrue(none.IsNone);
        }

        [Test]
        public void OptionCoercingTests_SomeToValue()
        {
            Option<int> some = 1;

            int value = (int)some;

            Assert.AreEqual(1, value);
        }

        [Test]
        public void OptionCoercingTests_NoneToValue()
        {
            Option<int> none = Option<int>.None;

            Assert.Throws<OptionCastEception>(() => { var i =  (int)none; });
        }

        [Test]
        public void OptionCoercingTests_ValueToSome()
        {
            Option<int> some = 1;

            Assert.IsTrue(some.IsSome);
        }

        [Test]
        public void OptionCoercingTests_ValueToNone()
        {
            Option<int> none = Option<int>.None;

            Assert.IsTrue(none.IsNone);
        }

        //[Test]
        //public void OptionCoercingTests_NullableSome()
        //{
        //    Option<int> some = ((int?)1).AsOption();

        //    Assert.IsTrue(some.IsSome);
        //}

        //[Test]
        //public void OptionCoercingTests_NullableNone()
        //{
        //    Option<int> none = ((int?)null).AsOption();

        //    Assert.IsTrue(none.IsNone);
        //}

        [Test]
        public void CanForeachOveroOption()
        {
            var count = 0;
            foreach (var item in Option<int>.Some(1))
            {
                count++;
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void CanForeachOveroOptionNone()
        {
            var count = 0;
            foreach (var item in Option<int>.None)
            {
                count++;
            }
            Assert.AreEqual(0, count);
        }
    }
}
