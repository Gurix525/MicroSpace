using System;
using UnityEngine;

namespace Maths
{
    public struct Line : IEquatable<Line>
    {
        #region Properties

        public static Line Zero => new Line(Vector2.zero, Vector2.zero);

        public Vector2 A { get; }
        public Vector2 B { get; }

        #endregion Properties

        #region Constructors

        public Line(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }

        #endregion Constructors

        #region Public

        public bool IsIntersecting(Line other, out Vector2 intersection)
        {
            return Geometry.AreLinesIntersecting(this, other, out intersection);
        }

        public bool IsIntersecting(Line other)
        {
            return Geometry.AreLinesIntersecting(this, other);
        }

        public bool IsIntersecting(Square other, out Vector2[] intersection)
        {
            return Geometry.AreLineAndSquareIntersecting(this, other, out intersection);
        }

        public bool IsIntersecting(Square other)
        {
            return Geometry.AreLineAndSquareIntersecting(this, other);
        }

        public override bool Equals(object other)
        {
            if (other is not Line)
                return false;
            return Equals((Line)other);
        }

        public bool Equals(Line other)
        {
            return A == other.A && B == other.B ||
                A == other.B && B == other.A;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(A.GetHashCode(), B.GetHashCode());
        }

        public override string ToString()
        {
            return $"{A}:{B}";
        }

        public static bool operator ==(Line l, Line r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(Line l, Line r)
        {
            return !l.Equals(r);
        }

        #endregion Public
    }
}