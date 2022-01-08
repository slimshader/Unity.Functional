using UnityEngine;

namespace Bravasoft.UnityFunctional
{
    public static class Physics
    {
        public static Option<RaycastHit> Raycast(in Ray ray) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit) ? Option.Some(hit) : Option.None;
    }

}
