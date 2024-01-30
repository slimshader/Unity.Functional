using FluentAssertions;
using NUnit.Framework;

namespace Bravasoft.Functional.Tests
{
    [TestFixture]
    public class ResultTests
    {
        [Test]
        public void DefaultResultIsFailed()
        {
            var result = default(Result<int>);

            result.IsOk.Should().BeFalse();

            result.TryGetError().IsSome.Should().BeTrue();
        }

        [Test]
        public void TwoOkResultsAreWhenSameValueEqual()
        {
            var r1 = Result.Ok("str");
            var r2 = Result.Ok("str");

            Assert.AreEqual(r1, r2);
        }

        [Test]
        public void TwoOkResultsAreNotEqualWhenDifferent()
        {
            var r1 = Result.Ok("str1");
            var r2 = Result.Ok("str2");

            Assert.AreNotEqual(r1, r2);
        }

        [Test]
        public void FailedIsNotOk()
        {
            var result = Result<int>.Fail(new Error());

            Assert.That(result.IsOk, Is.False);
        }

        [Test]
        public void FailedResultConvertsToFalse()
        {
            var result = Result<int>.Fail(new Error());

            Assert.That((bool)result, Is.False);
        }

        class TestError : Error { }

        [Test]
        public void ErrorDerivativeConvertsToFail()
        {
            Result<int> result = new TestError();

            Assert.IsFalse(result.IsOk);
        }

        [Test]
        public void ExceptionDerivativeConvertsToFail()
        {
            Result<int> result = new System.Exception();

            Assert.IsFalse(result.IsOk);
        }


        [Test]
        public void CanCheckExceptionType()
        {
            Result<int> result = new System.Exception();

            Assert.IsTrue(result.IsException<System.Exception>());
        }
    }
}