using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

namespace Maths
{
    public static class Geometry
    {
        #region Public

        /// <summary>
        /// Sprawdza, czy podane linie AB i CD się przecinają
        /// </summary>
        /// <returns>True, jeśli linie się przecinają oraz miejsce przecięcia</returns>
        public static bool AreLinesIntersecting(Vector2 A, Vector2 B,
            Vector2 C, Vector2 D, out Vector2 intersection)
        {
            if (A == C || A == D || B == C || B == D)
            {
                intersection = Vector2.zero;
                return false; // Lines have a common end, so technically are intersecting, but we don't need it
            }

            var p = A;
            var r = B - A;
            var q = C;
            var s = D - C;
            var qminusp = q - p;

            float cross_rs = GetCrossProduct2D(r, s);

            if (Approximately(cross_rs, 0f))
            {
                // Parallel lines
                if (Approximately(GetCrossProduct2D(qminusp, r), 0f))
                {
                    // Co-linear lines, could overlap
                    float rdotr = Vector2.Dot(r, r);
                    float sdotr = Vector2.Dot(s, r);
                    // this means lines are co-linear
                    // they may or may not be overlapping
                    float t0 = Vector2.Dot(qminusp, r / rdotr);
                    float t1 = t0 + sdotr / rdotr;
                    if (sdotr < 0)
                    {
                        // lines were facing in different directions so t1 > t0, swap to simplify check
                        Swap(ref t0, ref t1);
                    }

                    if (t0 <= 1 && t1 >= 0)
                    {
                        // Nice half-way point intersection
                        float t = Mathf.Lerp(Mathf.Max(0, t0), Mathf.Min(1, t1), 0.5f);
                        intersection = p + t * r;
                        return true;
                    }
                    else
                    {
                        // Co-linear but disjoint
                        intersection = Vector2.zero;
                        return false;
                    }
                }
                else
                {
                    // Just parallel in different places, cannot intersect
                    intersection = Vector2.zero;
                    return false;
                }
            }
            else
            {
                // Not parallel, calculate t and u
                float t = GetCrossProduct2D(qminusp, s) / cross_rs;
                float u = GetCrossProduct2D(qminusp, r) / cross_rs;
                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    intersection = p + t * r;
                    return true;
                }
                else
                {
                    // Lines only cross outside segment range
                    intersection = Vector2.zero;
                    return false;
                }
            }
        }

        public static bool AreLinesIntersecting(Vector2 A, Vector2 B,
            Vector2 C, Vector2 D)
        {
            return AreLinesIntersecting(A, B, C, D, out _);
        }

        /// <summary>
        /// Sprawdza, czy podane linie AB i CD się przecinają
        /// </summary>
        /// <returns>True, jeśli linie się przecinają oraz miejsce przecięcia</returns>
        public static bool AreLinesIntersecting(
            Line first, Line second, out Vector2 intersection)
        {
            return AreLinesIntersecting(first.A, first.B, second.A, second.B, out intersection);
        }

        public static bool AreLinesIntersecting(
            Line first, Line second)
        {
            return AreLinesIntersecting(first, second, out _);
        }

        /// <summary>
        /// Sprawdza, czy podana linia i kwadrat się przecinają
        /// </summary>
        /// <returns>True, jeśli linia i kwadrat się przecinają oraz miejsca przecięcia</returns>
        public static bool AreLineAndSquareIntersecting(
            Line line, Square square, out Vector2[] intersections)
        {
            bool isIntersecting = false;
            List<Vector2> newIntersections = new();
            foreach (Line item in square.Lines)
            {
                if (line.IsIntersecting(item, out Vector2 newIntersection))
                {
                    isIntersecting = true;
                    newIntersections.Add(newIntersection);
                }
            }
            if (newIntersections.Count == 0)
                intersections = null;
            else
                intersections = newIntersections.ToArray();
            return isIntersecting;
        }

        public static bool AreLineAndSquareIntersecting(
            Line line, Square square)
        {
            foreach (Line item in square.Lines)
            {
                if (line.IsIntersecting(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Sprawdza, czy podane kwadraty się przecinają
        /// </summary>
        /// <returns>True, jeśli kwadraty się przecinają oraz miejsca przecięcia</returns>
        public static bool AreSquaresIntersecting(
            Square first, Square second, out Vector2[] intersections)
        {
            bool isIntersecting = false;
            List<Vector2> newIntersections = new();
            foreach (var itemA in first.Lines)
            {
                foreach (var itemB in second.Lines)
                {
                    if (itemA.IsIntersecting(itemB, out Vector2 newIntersection))
                    {
                        isIntersecting = true;
                        newIntersections.Add(newIntersection);
                    }
                }
            }
            if (newIntersections.Count == 0)
                intersections = null;
            else
                intersections = newIntersections.ToArray();
            return isIntersecting;
        }

        public static bool AreSquaresIntersecting(
            Square first, Square second)
        {
            foreach (var itemA in first.Lines)
            {
                foreach (var itemB in second.Lines)
                {
                    if (itemA.IsIntersecting(itemB))
                        return true;
                }
            }
            return false;
        }

        public static float GetAngle(Vector2 lhs, Vector2 rhs)
        {
            float sign = -Math.Sign(rhs.x);
            double dot = (double)lhs.x * rhs.x + (double)lhs.y * rhs.y;
            double lhsMagnitude = Math.Sqrt(Math.Pow(lhs.x, 2) + Math.Pow(lhs.y, 2));
            double rhsMagnitude = Math.Sqrt(Math.Pow(rhs.x, 2) + Math.Pow(rhs.y, 2));
            double cos = dot / (lhsMagnitude * rhsMagnitude);
            double radians = Math.Acos(cos);
            double angle = radians * 180D / Math.PI;
            return (float)angle * sign;
        }

        #endregion Public

        #region Private

        /// <summary>
        /// Funkcja dla <code>AreLinesIntersecting()</code>
        /// </summary>
        private static float GetCrossProduct2D(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        /// <summary>
        /// Funkcja dla <code>AreLinesIntersecting()</code>
        /// </summary>
        private static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        /// <summary>
        /// Funkcja dla <code>AreLinesIntersecting()</code>
        /// </summary>
        private static bool Approximately(float a, float b, float tolerance = 1e-5F)
        {
            return Mathf.Abs(a - b) <= tolerance;
        }

        #endregion Private
    }
}