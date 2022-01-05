using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using Is = NUnit.Framework.Is;
using UnityIs = UnityEngine.TestTools.Constraints.Is;

namespace Bravasoft
{

    public static class IndexedExtensions
    {
        public static IndexedEnumerator<T> Indexed<T>(this IEnumerable<T> ts) =>
            new IndexedEnumerator<T>(ts.GetEnumerator());
    }
}

namespace Bravasoft.Tests
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
    }
}
