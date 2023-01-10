using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtensionMethods
{
    public static class Vector2ArrayExtensions
    {
        public static float GetPathLength(this Vector2[] path)
        {
            if (path == null)
                return float.PositiveInfinity;
            float length = 0;
            for (int i = 0; i < path.Length - 1; i++)
            {
                length += Vector2.Distance(path[i], path[i + 1]);
            }
            return length;
        }
    }
}