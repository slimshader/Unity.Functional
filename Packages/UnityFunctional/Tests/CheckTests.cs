using NUnit.Framework;

namespace Bravasoft.Functional.Tests
{
    public class CheckTests
    {
        [Test]
        public void IsNullForIntIsFalse()
        {
            Assert.IsFalse(Check.IsNull(5));
        }

        struct TestStruct
        {
            public int Value;
        }

        [Test]
        public void IsNullForStructIsFalse()
        {
            var s = new TestStruct();

            Assert.IsFalse(Check.IsNull(s));
        }

        class TestClass
        {
            public int Value;
        }

        [Test]
        public void IsNullForReferencesChecksForActuallNull()
        {
            var c = new TestClass();

            Assert.IsFalse(Check.IsNull(c));

            c = null;

            Assert.IsTrue(Check.IsNull(c));
        }
    }
}
