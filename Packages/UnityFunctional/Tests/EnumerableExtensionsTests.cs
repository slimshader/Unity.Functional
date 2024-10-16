using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using static System.Linq.Enumerable;

namespace Bravasoft.Functional.Tests
{
    using static Prelude;

    public class EnumerableExtensionsTests
    {
        [Test]
        public void AppendingSome()
        {
            var ints = Empty<int>().Append(Some(1)).ToList();

            ints.Count.Should().Be(1);
            ints[0].Should().Be(1);
        }

        [Test]
        public void AppendingNone()
        {
            var ints = Empty<int>().Append(None<int>()).ToList();

            Assert.That(ints.Count, Is.EqualTo(0));
        }

        [Test]
        public void TryFirstSuccess()
        {
            var ints = Range(0, 10);
            var result = ints.TryFirst();

            result.Should().Equal(0);
        }

        [Test]
        public void TryFirstFailure()
        {
            var ints = Empty<int>();
            var result = ints.TryFirst();

            result.Should().Equal(Option<int>.None);
        }

        [Test]
        public void TryFirstWithPredicateSuccess()
        {
            var ints = Range(0, 10);
            var result = ints.TryFirst(i => i == 5);

            result.Should().Equal(5);
        }

        [Test]
        public void TryFirstWithPredicateFailure()
        {
            var ints = Range(0, 10);
            var result = ints.TryFirst(i => i == 10);

            result.Should().Equal(Option<int>.None);
        }
    }
}
