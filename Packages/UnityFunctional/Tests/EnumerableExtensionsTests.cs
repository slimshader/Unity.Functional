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
        public void TryFindSuccess()
        {
            var ints = Range(0, 10);
            var result = ints.TryFind(i => i == 5);

            Assert.That(result, Is.EqualTo(Some(5)));
        }

        [Test]
        public void TryFindFailure()
        {
            var ints = Range(0, 10);
            var result = ints.TryFind(i => i == 10);

            Assert.That(result, Is.EqualTo(None<int>()));
        }
    }
}
