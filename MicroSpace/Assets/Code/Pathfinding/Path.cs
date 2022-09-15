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
            Nodes.Push(start);
        }

        public Stack<Node> Nodes { get; set; }

        public Node this[int index]
        {
            get => Nodes.ElementAt(index);
        }
    }
}