using UnityEngine;

namespace Miscellaneous
{
    public static class References
    {
        public static int WallsLayer { get; private set; }

        public static int FloorsLayer { get; private set; }

        public static Transform World { get; set; }

        public static Rigidbody2D FocusedSatellite { get; set; }

        public static Rigidbody2D Target { get; set; }

        public static void Initialize()
        {
            WallsLayer = LayerMask.NameToLayer("Walls");
            FloorsLayer = LayerMask.NameToLayer("Floors");
        }
    }
}