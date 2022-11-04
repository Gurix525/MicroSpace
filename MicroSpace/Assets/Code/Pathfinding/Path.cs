using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Path : List<Vector2>
    {
        public Path()
        {
        }

        public Path(Node start)
        {
            Add(start.Position);
        }

        public Path(Vector2 start)
        {
            Add(start);
        }

        public Path(IEnumerable<Vector2> path)
        {
            foreach (var item in path)
                Add(item);
        }
    }
}