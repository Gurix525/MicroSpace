using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Pathfinding
{
    public struct Line
    {
        public static readonly Line Zero = new Line(Vector2.zero, Vector2.zero);

        public Vector2 A { get; }
        public Vector2 B { get; }

        public Line(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }
    }
}