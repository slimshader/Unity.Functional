using UnityEngine;

namespace Bravasoft.Functional.Unity
{
    public static class Physics
    {
        public static Option<RaycastHit> TryRaycast(in Ray ray) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit) ? Option.Some(hit) : Option.None;
    }

}
