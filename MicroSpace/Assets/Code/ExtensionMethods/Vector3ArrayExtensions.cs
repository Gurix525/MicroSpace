using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExtensionMethods
{
    public static class Vector3ArrayExtensions
    {
        public static float GetPathLength(this Vector3[] path)
        {
            if (path == null)
                return 0;
            float length = 0;
            for (int i = 0; i < path.Length - 1; i++)
            {
                length += Vector3.Distance(path[i], path[i + 1]);
            }
            return length;
        }
    }
}