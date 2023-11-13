namespace Bravasoft.Functional.Unity
{
    public static class ComponentExtensions
    {
        public static Option<T> TryGetComponent<T>(this UnityEngine.Component component) =>
            component.TryGetComponent(out T c) ? c : Core.None;

        public static Option<T> TryGetComponentInParent<T>(this UnityEngine.Component component) =>
            Prelude.Optional(component.GetComponentInParent<T>());

        public static Option<T> TryGetComponentInChildren<T>(this UnityEngine.Component component) =>
            Prelude.Optional(component.GetComponentInChildren<T>());
    }

}
