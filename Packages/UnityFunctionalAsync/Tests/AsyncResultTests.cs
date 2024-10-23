using Cysharp.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

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

        [UnityTest]
        public IEnumerator CanIterValidInceMultipleTimes() => UniTask.ToCoroutine(async () =>
        {
            var result = Get42Async().ToAsyncResult();

            int value = 0;

            await result.Iter(x => value = 1);
            
            value.Should().Be(1);
            
            await result.Iter(x => value = 2);
            
            value.Should().Be(2);
        });

        async UniTask<int> Throwing()
        {
            await UniTask.Delay(10);
            throw new Exception("Test");
        }

        async UniTask<Result<int>> ThrowingResult()
        {
            await UniTask.Delay(10);
            throw new Exception("Test");
        }


        [UnityTest]
        public IEnumerator IfErrorWhenExceptionIsThrown() => UniTask.ToCoroutine(async () =>
        {
            var result = Throwing().ToAsyncResult();

            var value = await result.IfError(_ =>  1);

            value.Should().Be(1);
        });

        [UnityTest]
        public IEnumerator IfErrorWhenExceptionIsNotThrown() => UniTask.ToCoroutine(async () =>
        {
            var result = Get42Async().ToAsyncResult();
            var value = await result.IfError(_ => 1);
            value.Should().Be(42);
        });

        [UnityTest]
        public IEnumerator CanMatchValidValue() => UniTask.ToCoroutine(async () =>
        {
            var result = Get42Async().ToAsyncResult();

            var value = await result.Match(x => 1, _ => 0);

            value.Should().Be(1);
        });

        [UnityTest]
        public IEnumerator CanMatchErrorValue() => UniTask.ToCoroutine(async () =>
        {
            var result = Throwing().ToAsyncResult();
            var value = await result.Match(x => 1, _ => 0);
            value.Should().Be(0);
        });

        [UnityTest]
        public IEnumerator IfHasNoneOptionErrorWhenInitilizedFromDefaultOption() => UniTask.ToCoroutine(async () =>
        {
            var result = default(Option<int>).ToAsyncResult();

            var value = await result.IfError(e => e switch
            {
                NoneOptionError _ => 1,
                _ => 0
            });

            value.Should().Be(1);
        });

        [UnityTest]
        public IEnumerator MatchesOkWhenInitializedFromOkResult() => UniTask.ToCoroutine(async () =>
        {
            var result = Result<int>.Ok(42).ToAsyncResult();
            var value = await result.Match(x => 1, _ => 0);
            value.Should().Be(1);
        });

        [UnityTest]
        public IEnumerator MatchesErrorWhenInitializedFromErrorResult() => UniTask.ToCoroutine(async () =>
        {
            var result = Result<int>.Fail(new Error("Test")).ToAsyncResult();
            var value = await result.Match(x => 1, _ => 0);
            value.Should().Be(0);
        });

        [UnityTest]
        public IEnumerator MatchesOkWhenInitializedFromOkResultTask() => UniTask.ToCoroutine(async () =>
        {
            var result = UniTask.FromResult(Result<int>.Ok(42)).ToAsyncResult();
            var value = await result.Match(x => 1, _ => 0);
            value.Should().Be(1);
        });

        [UnityTest]
        public IEnumerator MatchesErrorWhenInitializedFromErrorResultTask() => UniTask.ToCoroutine(async () =>
        {
            var result = ThrowingResult().ToAsyncResult();
            var value = await result.Match(x => 1, _ => 0);
            value.Should().Be(0);
        });


    }
}
