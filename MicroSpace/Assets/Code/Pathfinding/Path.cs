using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Pathfinding
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
    }
}