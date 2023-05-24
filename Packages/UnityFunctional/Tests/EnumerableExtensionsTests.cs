using NUnit.Framework;
using System.Linq;
using static System.Linq.Enumerable;

namespace Bravasoft.Functional.Tests
{
    public class EnumerableExtensionsTests
    {
        [Test]
        public void CanAppendSomeOptionToEmptyEnumerable()
        {
            var ints = Empty<int>().Append(Option<int>.Some(1)).ToList();

            Assert.That(ints.Count, Is.EqualTo(1));
            Assert.That(ints[0], Is.EqualTo(1));
        }
    }
}
