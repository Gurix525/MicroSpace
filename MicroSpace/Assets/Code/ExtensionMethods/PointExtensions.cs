using DelaunatorSharp;
using UnityEngine;

namespace ExtensionMethods
{
    public static class PointExtensions
    {
        public static Vector2 ToVector2(this Point point)
            => new((float)point.X, (float)point.Y);
    }
}