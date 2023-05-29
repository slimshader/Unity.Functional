using UnityEngine;

namespace Bravasoft.Functional.Unity
{
    public static class GO
    {
        public static Option<GameObject> TryFind(string name) =>
            Prelude.Optional(GameObject.Find(name));

        public static Option<GameObject> TryFindGameObjectWithTag(string tag) =>
            Prelude.Optional(GameObject.FindGameObjectWithTag(tag));

        public static Option<GameObject> TryFindWithTag(string tag) =>
            Prelude.Optional(GameObject.FindWithTag(tag));
    }
}
