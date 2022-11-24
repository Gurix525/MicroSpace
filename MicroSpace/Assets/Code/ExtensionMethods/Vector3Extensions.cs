using UnityEngine;

namespace ExtensionMethods
{
    public static class Vector3Extensions
    {
        public static Vector3 RotateAroundPivot(
            this Vector3 point,
            Vector3 pivot,
            Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }

        public static Vector3 RotateAroundPivot(
            this Vector3 point,
            Vector2 pivot,
            Vector3 angles)
        {
            return Quaternion
                .Euler(angles) * (point - (Vector3)pivot) + (Vector3)pivot;
        }

        public static Vector3 RotateAroudPivot(
            this Vector3 point,
            Vector3 pivot,
            float zAngles)
        {
            return Quaternion
                .Euler(new(0, 0, zAngles)) * (point - pivot) + pivot;
        }

        public static Vector3 RotateAroudPivot(
            this Vector3 point,
            Vector2 pivot,
            float zAngles)
        {
            return Quaternion
                .Euler(new(0, 0, zAngles)) * (point - (Vector3)pivot)
                + (Vector3)pivot;
        }

        public static Vector3 Round(this Vector3 point)
        {
            return new Vector3(
                Mathf.Round(point.x),
                Mathf.Round(point.y),
                Mathf.Round(point.z));
        }
    }
}