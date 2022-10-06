using Assets.Code.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Maths
{
    public struct Square : IEquatable<Square>
    {
        #region Fields

        private readonly Vector2[] _vertices;

        #endregion Fields

        #region Constructors

        public Square(Vector2[] vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));
            if (vertices.Length != 4)
                throw new ArgumentException("Vertices length must equal to 4.");
            _vertices = vertices;
        }

        public Square(Vector3 position, float halfOfEdgeLength, Vector3 rotation)
        {
            _vertices = new Vector2[4];
            _vertices[0] = position + new Vector3(-halfOfEdgeLength, -halfOfEdgeLength, 0);
            _vertices[1] = position + new Vector3(-halfOfEdgeLength, halfOfEdgeLength, 0);
            _vertices[2] = position + new Vector3(halfOfEdgeLength, -halfOfEdgeLength, 0);
            _vertices[3] = position + new Vector3(halfOfEdgeLength, halfOfEdgeLength, 0);
            for (int i = 0; i < 4; i++)
                _vertices[i] = ((Vector3)_vertices[i]).RotateAroundPivot(position, rotation);
        }

        #endregion Constructors

        #region Properties

        public Vector2 A => _vertices[0];
        public Vector2 B => _vertices[1];
        public Vector2 C => _vertices[2];
        public Vector2 D => _vertices[3];
        public Vector2[] Vertices => _vertices;

        public Line[] Lines => new Line[4]
        {
            new(A, B),
            new(B, C),
            new(C, D),
            new(D, A)
        };

        public Vector2 this[int index] => _vertices[index];

        #endregion Properties

        #region Public

        public bool IsIntersecting(Line other, out Vector2[] intersections)
        {
            return Geometry.AreLineAndSquareIntersecting(other, this, out intersections);
        }

        public bool IsIntersecting(Square other, out Vector2[] intersections)
        {
            return Geometry.AreSquaresIntersecting(this, other, out intersections);
        }

        public override bool Equals(object other)
        {
            if (other is not Square)
                return false;
            return Equals((Square)other);
        }

        public bool Equals(Square other)
        {
            Vector2[] thisVerticesSorted = Vertices.OrderBy(x => x).ToArray();
            Vector2[] otherVerticesSorted = other.Vertices.OrderBy(x => x).ToArray();
            return thisVerticesSorted.SequenceEqual(otherVerticesSorted);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_vertices);
        }

        public static bool operator ==(Square l, Square r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(Square l, Square r)
        {
            return !l.Equals(r);
        }

        public override string ToString()
        {
            return $"{A}:{B}:{C}:{D}";
        }

        #endregion Public
    }
}