using UnityEngine;

namespace Bravasoft.Functional.Unity
{
    using static Core;
    public static class Physics
    {
        public static Option<RaycastHit> TryRaycast(in Ray ray) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit) ? hit : None;

        public static Option<RaycastHit> TryRaycast(in Ray ray, float maxDistance) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit, maxDistance) ? hit : None;

        public static Option<RaycastHit> TryRaycast(in Ray ray, float maxDistance, int layerMask) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask) ? hit : None;

        public static Option<RaycastHit> TryRaycast(in Ray ray, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction) =>
            UnityEngine.Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, queryTriggerInteraction) ? hit : None;

        public static Option<RaycastHit> TryRaycast(Vector3 origin, Vector3 direction, float maxDistance, int layerMask) =>
            UnityEngine.Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask) ? hit : None;

        public static Option<RaycastHit> TryRaycast(Vector3 origin, Vector3 direction, float maxDistance) =>
            UnityEngine.Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance) ? hit : None;

        public static Option<RaycastHit> TryRaycast(Vector3 origin, Vector3 direction) =>
            UnityEngine.Physics.Raycast(origin, direction, out RaycastHit hit) ? hit : None;
    }
}
