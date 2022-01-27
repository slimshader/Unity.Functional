using NUnit.Framework;
using System.Linq;

namespace Bravasoft.Unity.Functional.Tests
{
    public class EnumerableExtensionsTests
    {
        [Test]
        public void CanAppendSomeOptionToEmptyEnumerable()
        {
            var ints = Enumerable.Empty<int>().Append(Option<int>.Some(1)).ToList();

            Assert.That(ints.Count, Is.EqualTo(1));
            Assert.That(ints[0], Is.EqualTo(1));
        }
    }
}
