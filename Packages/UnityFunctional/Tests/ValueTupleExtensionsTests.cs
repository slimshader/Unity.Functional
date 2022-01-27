using NUnit.Framework;

namespace Bravasoft.Unity.Functional.Tests
{
    public class ValueTupleExtensionsTests
    {
        [Test]
        public void CanAppendToSingleValue()
        {
            var t = 1.ToTuple().Append(2);

            Assert.That(t, Is.EqualTo((1, 2)));
        }

        [Test]
        public void CanAppendToDoubleValue()
        {
            var t = 1.ToTuple().Append(2).Append(3);

            Assert.That(t, Is.EqualTo((1, 2, 3)));
        }

        [Test]
        public void CanAppendToTripleValue()
        {
            var t = 1.ToTuple().Append(2).Append(3).Append(4);

            Assert.That(t, Is.EqualTo((1, 2, 3, 4)));
        }
    }
}
