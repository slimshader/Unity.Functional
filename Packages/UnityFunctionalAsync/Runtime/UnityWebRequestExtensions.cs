using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine.Networking;

namespace Bravasoft.Functional.Unity
{
    public static class UnityWebRequestExtensions
    {
        public static async UniTask<Result<UnityWebRequest>> TrySend(this UnityWebRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await request.SendWebRequest().WithCancellation(cancellationToken);

                return response.result switch
                {
                    UnityWebRequest.Result.ConnectionError => Result.Fail(new Error(response.error)),
                    UnityWebRequest.Result.ProtocolError => Result.Fail(new Error(response.error)),
                    UnityWebRequest.Result.DataProcessingError => Result.Fail(new Error(response.error)),
                    _ => Result.Ok(request)
                };
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
