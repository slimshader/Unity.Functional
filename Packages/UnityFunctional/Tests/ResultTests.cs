using NUnit.Framework;

namespace Bravasoft.Unity.Functional.Tests
{
    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void FailedIsNotOk()
        {
            var result = Result<int, string>.Fail(string.Empty);

            Assert.That(result.IsOk, Is.False);
        }
    }
}