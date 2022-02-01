using NUnit.Framework;
using System.Linq;
using UnityEngine.TestTools.Constraints;
using Is = NUnit.Framework.Is;
using UnityIs = UnityEngine.TestTools.Constraints.Is;

namespace Bravasoft.Unity.Functional.Tests
{
    public class IndexedTests
    {
        [Test]
        public void CanIndexEnumerableOfIntegers()
        {
            foreach (var (value, index) in Enumerable.Range(0, 3).Indexed())
            {
                Assert.That(value, Is.EqualTo(index));
            }
        }

        [Test]
        public void IndexedDoesNotAllocate()
        {
            var range = Enumerable.Range(1, 3);
            var sum = 0;

            TestDelegate func = () =>
            {
                foreach (var (value, index) in range.Indexed())
                {
                    sum += value * index;
                }
            };

            Assert.That(func, UnityIs.Not.AllocatingGCMemory());
            Assert.That(sum, Is.EqualTo(8));

        }

        [Test]
        public void WorskWithLinqOperators()
        {
            var evens =  Enumerable.Range(1, 3).Indexed().Where(x => x.Index % 2 == 0);
        }
    }
}
