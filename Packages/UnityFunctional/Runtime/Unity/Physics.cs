using UnityEngine;

namespace Bravasoft.Functional.Unity
{
    public static class Physics
    {
        public static Option<RaycastHit> TryRaycast(in Ray ray) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit) ? Option.Some(hit) : Option.None;

        public static Option<RaycastHit> TryRaycast(in Ray ray, float maxDistance) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit, maxDistance) ? Option.Some(hit) : Option.None;

        public static Option<RaycastHit> TryRaycast(in Ray ray, float maxDistance, int layerMask) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask) ? Option.Some(hit) : Option.None;

        public static Option<RaycastHit> TryRaycast(in Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, queryTriggerInteraction) ? Option.Some(hit) : Option.None;

        public static Option<RaycastHit> TryRaycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask) =>
            UnityEngine.Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask) ? Option.Some(hit) : Option.None;

        public static Option<RaycastHit> TryRaycast(Vector3 origin, Vector3 direction, float maxDistance) =>
            UnityEngine.Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance) ? Option.Some(hit) : Option.None;

        public static Option<RaycastHit> TryRaycast(Vector3 origin, Vector3 direction) =>
            UnityEngine.Physics.Raycast(origin, direction, out RaycastHit hit) ? Option.Some(hit) : Option.None;
    }
}
