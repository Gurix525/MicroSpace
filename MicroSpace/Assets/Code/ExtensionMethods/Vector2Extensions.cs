using DelaunatorSharp;
using UnityEngine;

namespace ExtensionMethods
{
    public static class Vector2Extensions
    {
        public static Point ToPoint(this Vector2 v)
        {
            return new Point(v.x, v.y);
        }
    }
}