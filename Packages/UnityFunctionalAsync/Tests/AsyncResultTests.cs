using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;
using static Bravasoft.Functional.AsyncPrelude;

namespace Bravasoft.Functional.Tests
{
    public class AsyncResultTests
    {
        [Test]
        public async Task Select_From_FailedAsync_Results_In_Failed()
        {
            var f1 = AsyncFail<int>("error");

            var f2 = from i in f1 select i * 2;

            (await f2).IsOk.Should().BeFalse();
        }

        [Test]
        public async Task Select_From_OkAsync_Results_In_Ok()
        {
            var f1 = AsyncOk(1);

            var f2 = from i in f1 select i * 2;

            (await f2).IsOk.Should().BeTrue();
            (await f2).DefaultIfFailed.Should().Be(2);
        }
    }
}
