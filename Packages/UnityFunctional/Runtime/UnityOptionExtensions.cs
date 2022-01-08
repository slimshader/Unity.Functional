using UnityEngine;

namespace Bravasoft.UnityFunctional
{
    public static class UnityOptionExtensions
    {
        public static Option<T> TryGetComponent<T>(this GameObject go) =>
            go.TryGetComponent(out T c) ? Option.Some(c) : Option.None;

        public static Option<T> TryGetComponentInParent<T>(this GameObject go) =>
            Option.Of(go.GetComponentInParent<T>());

        public static Option<T> TryGetComponentInChildren<T>(this GameObject go) =>
            Option.Of(go.GetComponentInChildren<T>());
    }

}
