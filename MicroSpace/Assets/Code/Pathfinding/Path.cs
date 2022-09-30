using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Pathfinding
{
    public class Path
    {
        public Path()
        {
            Nodes = new();
        }

        public Path(Node start)
        {
            Nodes = new();
            Nodes.Add(start.Position);
        }

        //public List<Node> Nodes { get; set; }

        public List<Vector2> Nodes { get; set; }

        public int Count => Nodes.Count;

        public Vector2 this[int index]
        {
            get => Nodes[index];
            set
            {
                Nodes[index] = value;
            }
        }
    }
}