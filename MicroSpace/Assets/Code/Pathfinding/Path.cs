using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Nodes.Add(start);
        }

        public List<Node> Nodes { get; set; }

        public int Count => Nodes.Count;

        public Node this[int index]
        {
            get => Nodes[index];
        }
    }
}