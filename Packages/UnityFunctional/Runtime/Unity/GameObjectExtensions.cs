namespace Bravasoft.Functional.Unity
{
    public static class GameObjectExtensions
    {
        public static Option<T> TryGetComponent<T>(this UnityEngine.GameObject go) =>
            go.TryGetComponent(out T c) ? Option.Some(c) : Option.None;

        public static Option<T> TryGetComponentInParent<T>(this UnityEngine.GameObject go) =>
            Prelude.Optional(go.GetComponentInParent<T>());

        public static Option<T> TryGetComponentInChildren<T>(this UnityEngine.GameObject go) =>
            Prelude.Optional(go.GetComponentInChildren<T>());
    }

}
