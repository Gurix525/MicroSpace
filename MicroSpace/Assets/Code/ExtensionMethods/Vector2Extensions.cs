using DelaunatorSharp;
using UnityEngine;

namespace ExtensionMethods
{
    public static class Vector2Extensions
    {
        public static Vector2 RotateAroundPivot(
            this Vector2 point,
            Vector2 pivot,
            float zAngles)
        {
            return (Vector2)((Vector3)point).RotateAroudPivot(pivot, zAngles);
        }

        public static Point ToPoint(this Vector2 v)
        {
            return new Point(v.x, v.y);
        }
    }
}