using NUnit.Framework;
using System.Linq;
using static System.Linq.Enumerable;
using Is = NUnit.Framework.Is;

namespace Bravasoft.Functional.Tests
{
    public class IndexedTests
    {
        [Test]
        public void CanIndexEnumerableOfIntegers()
        {
            foreach (var (value, index) in Range(0, 3).Indexed())
            {
                Assert.That(value, Is.EqualTo(index));
            }
        }

        [Test]
        public void WorskWithLinqOperators()
        {
            var evens = Range(1, 3).Indexed().Where(x => x.Index % 2 == 0);
        }
    }
}
