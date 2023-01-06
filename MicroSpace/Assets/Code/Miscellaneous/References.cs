using UnityEngine;

namespace Miscellaneous
{
    public static class References
    {
        public static Transform WorldTransform { get; private set; }

        public static void SetWorldTransform(Transform world)
        {
            WorldTransform = world;
        }
    }
}