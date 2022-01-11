using NUnit.Framework;

namespace Bravasoft.Unity.Functional.Tests
{
    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void FailedIsNotOk()
        {
            var result = Result<int>.Fail(new Error());

            Assert.That(result.IsOk, Is.False);
        }
    }
}