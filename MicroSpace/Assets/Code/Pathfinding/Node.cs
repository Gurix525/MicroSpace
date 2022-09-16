using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Pathfinding
{
    public class Node : IEquatable<Node>
    {
        public int Index { get; }
        public Vector2 Position { get; }
        public Dictionary<Node, float> ConnectedNodes { get; set; }

        private static int _index = 0;

        public Node(Vector2 position)
        {
            Index = _index++;
            Position = position;
            ConnectedNodes = new();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is not Node)
                return false;
            return Equals((Node)obj);
        }

        public bool Equals(Node other)
        {
            return Position == other.Position;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Index, Position);
        }

        public static bool operator ==(Node left, Node right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return Position.ToString();
        }
    }
}