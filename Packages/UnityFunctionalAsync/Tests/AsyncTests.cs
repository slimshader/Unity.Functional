using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;


namespace Bravasoft.Functional.Tests
{
    public class AsyncTests
    {
        [UnityTest]
        public IEnumerator ResultHasValueWhenTaskIsSuccessful()
        {
            async UniTask<int> AsyncInt(int value)
            {
                await UniTask.Delay(100);
                return value;
            }

            Result<int> result = default;

            var task = AsyncInt(1).ToAsyncResult().ContinueWith(x => result = x);

            yield return task.ToCoroutine();

            Assert.IsTrue(result);
        }

        [UnityTest]
        public IEnumerator ResultHasErrorWhenTaskIsFaulted()
        {
            async UniTask<int> AsyncInt(int value)
            {
                await UniTask.Delay(100);
                throw new System.Exception();
            }

            Result<int> result = default;

            var task = AsyncInt(1).ToAsyncResult().ContinueWith(x => result = x);

            yield return task.ToCoroutine();

            Assert.IsFalse(result);
            Assert.IsTrue(result.IsException<System.Exception>());
        }

        [UnityTest]
        public IEnumerator CanRetry()
        {
            int calls = 0;

            async UniTask<int> AsyncInt(int value)
            {
                calls++;
                await UniTask.Delay(100);
                throw new System.Exception();
            }

            var task = Async.Retry(() => AsyncInt(1), 3, 1).ToAsyncResult();

            yield return task.ToCoroutine();

            Assert.AreEqual(4, calls);
        }


        [UnityTest]
        public IEnumerator CanRetryResult()
        {
            int calls = 0;

            async UniTask<int> AsyncInt(int value)
            {
                calls++;
                await UniTask.Delay(100);
                throw new System.Exception();
            }

            var task = Async.Retry(Async.ToAsyncResult(() => AsyncInt(1)), 3, 1);

            yield return task.ToCoroutine();

            Assert.AreEqual(4, calls);
        }

    }
}
