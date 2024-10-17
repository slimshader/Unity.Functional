using Bravasoft.Functional.Errors;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Bravasoft.Functional
{
    public delegate Result<T> IO<Env, T>(Env env);

    public sealed class IoError : ExceptionError
    {
        public IoError(Exception exception) : base(exception) { }
    }

    public static class IO
    {
        public static IO<Env, T> Of<T, Env>(T value) =>
            (Env _) => value;

        public static Result<T> Run<Env, T>(this IO<Env, T> io, Env env)
        {
            try
            {
                return io(env);
            }
            catch (Exception e)
            {
                return Result.Fail(new IoError(e));
            }
        }
    }
}

namespace Bravasoft.Functional.Async.Tests
{
    public class AsyncResultTests : MonoBehaviour
    {
        [Test]

        public void IterThrowsForDefaultInstance()
        {
            AsyncResult<int> result = default;

            bool wasRun = false;

            result
                .Invoking(x => x.Iter(_ => wasRun = true))
                .Should().Throw<InvalidOperationException>();

            wasRun.Should().BeFalse();
        }

        [Test]
        public void IterRunsForOkInstance()
        {
            AsyncResult<int> result = AsyncResult<int>.Ok(42);            
            int value = 0;

            result.Iter(x => value = x);

            value.Should().Be(42);
        }

        async UniTask<int> Get42Async()
        {
            await UniTask.Delay(10);
            return 42;
        }

        [UnityTest]
        public IEnumerator IterRunsForOkInstanceAsync() => UniTask.ToCoroutine(async () =>
        {
            AsyncResult<int> result = Get42Async().ToAsyncResult();

            int value = 0;

            await result.Iter(x => value = x);
 
            value.Should().Be(42);
        });

        [UnityTest]
        public IEnumerator CanAsyncForeachValidInstance() => UniTask.ToCoroutine(async () =>
        {
            AsyncResult<int> result = Get42Async().ToAsyncResult();
            int value = 0;
            await foreach (var x in result)
            {
                value = x;
            }
            value.Should().Be(42);
        });

        [UnityTest]
        public IEnumerator CanNotAsyncForeachValidInstance() => UniTask.ToCoroutine(async () =>
        {
            AsyncResult<int> result = default;
            int value = 0;
            await foreach (var x in result)
            {
                value = x;
            }
            value.Should().Be(0);
        });

    }
}
