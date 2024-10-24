using Cysharp.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace Bravasoft.Functional.Async.Tests
{
    public static class F<T>
    {
        public static readonly Func<T, T> Identity = x => x;
        public static readonly Func<T, bool> True = _ => true;
        public static readonly Func<T, bool> False = _ => false;
    }

    public static class A<T>
    {
        public static readonly Action<T> NoOp = _ => { };
    }

    public class AsyncResultTests : MonoBehaviour
    {
        [Test]

        public void IterThrowsForDefaultInstance()
        {
            AsyncResult<int> result = default;

            Func<Task> act = async () => { await result.Iter(A<int>.NoOp); };

            act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test]
        public void IterRunsForOkInstance() => UniTask.ToCoroutine(async () =>
        {
            AsyncResult<int> result = Get42Async().ToAsyncResult();

            int value = 0;
            
            await result.Iter(x => value = x);

            value.Should().Be(42);
        });

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
        public IEnumerator HasNoneOptionErrorWhenInitilizedFromDefaultOption() => UniTask.ToCoroutine(async () =>
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

        [UnityTest]
        public IEnumerator IterIsNotExecutedWhenExceptionIsThrown() => UniTask.ToCoroutine(async () =>
        {
            var result = Throwing().ToAsyncResult();
            var value = 0;
            await result.Iter(x => value = x);
            value.Should().Be(0);
        });

        public class ParseError : Error
        {
            public ParseError(string message) : base(message)
            {
            }
        }

        async UniTask<Result<int>> TryParseAsync(string str)
        {
            await UniTask.Delay(10);
            if (int.TryParse(str, out int value))
            {
                return value;
            }

            return new ParseError("Failed to parse");
        }

        [UnityTest]
        public IEnumerator FailedAsyncParsingMatchesError() => UniTask.ToCoroutine(async () =>
        {
            var result = TryParseAsync("not a number").ToAsyncResult();
            
            var value = await result.Match(_ => false, e => e switch
            {
                ParseError pe => true,
                _ => false
            });
            
            value.Should().BeTrue();
        });

        [UnityTest]
        public IEnumerator LinqFlowBothGood() => UniTask.ToCoroutine(async () =>
        {
            var result = from _1 in TryParseAsync("42").ToAsyncResult()
                         from _2 in TryParseAsync("42").ToAsyncResult()
                         select _1 + _2;

            var value = await result.Match(x => x, e => 0);

            value.Should().Be(84);
        });

        [UnityTest]
        public IEnumerator LinqFlowFirstBad() => UniTask.ToCoroutine(async () =>
        {
            var result = from _1 in TryParseAsync("not a number").ToAsyncResult()
                         from _2 in TryParseAsync("42").ToAsyncResult()
                         select _1 + _2;

            var value = false;
            
            await result.IterError<ParseError>(_ => value = true);

            value.Should().BeTrue();
        });

        [UnityTest]
        public IEnumerator LinqFlowSecondBad() => UniTask.ToCoroutine(async () =>
        {
            var result = from _1 in TryParseAsync("42").ToAsyncResult()
                         from _2 in TryParseAsync("not a number").ToAsyncResult()
                         select _1 + _2;
            var value = false;

            await result.IterError<ParseError>(_ => value = true);
            value.Should().BeTrue();
        });

        [UnityTest]
        public IEnumerator ConvertsValidToOkResult() => UniTask.ToCoroutine(async () =>
        {
            var asyncResult = Get42Async().ToAsyncResult();
            
            var result = await asyncResult.ToResult();
            
            result.Should().Be(Result<int>.Ok(42));
        });
    }
}
