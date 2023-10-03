using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Networking;

namespace Bravasoft.Functional.Unity
{
    public static class Http
    {
        public static UniTask<Result<string>> TrySendGet(Uri uri, CancellationToken cancellationToken = default)
        {
            using (var request = UnityWebRequest.Get(uri))
            {
                return request.TrySend(default).Map(x => x.downloadHandler.text);
            }
        }
    }
}
