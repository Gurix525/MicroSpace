using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code.ExtensionMethods
{
    public static class Vector3Extensions
    {
        public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }

        public static Vector3 Round(this Vector3 point) =>
            new Vector3(
                Mathf.Round(point.x),
                Mathf.Round(point.y),
                Mathf.Round(point.z));
    }
}