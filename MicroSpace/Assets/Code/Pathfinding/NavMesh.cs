using Assets.Code.ExtensionMethods;
using DelaunatorSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Assets.Code.Pathfinding
{
    public class NavMesh : MonoBehaviour
    {
        [SerializeField] private List<Agent> _agents;
        private BoxCollider2D[] _colliders = { };
        private List<(Vector2 A, Vector2 B)> _invalidEdges = new();
        [SerializeField] private float _lineDrawTime = 5F;
        [SerializeField] private float _navBorder = 0.5F;
        [SerializeField] private float _navMeshSizeFromCenter = 50;
        private List<Node> _nodes = new();
        private List<Vector2> _nodesPositions = new();
        [SerializeField] private Material _simpleMaterial;
        private List<(Vector2 A, Vector2 B)> _validEdges = new();
        private List<Vector2> _vertices = new();
        [SerializeField] private float _verticeSize = 0.1F;
        private float _vOffset => _verticeSize / 2;

        public Path FindPath(Vector2 startPos, Vector2 endPos)
        {
            Node start = FindClosestNode(startPos);
            Node end = FindClosestNode(endPos);

            Path path = new Path();

            List<TempNode> open = new();
            List<TempNode> closed = new();

            open.Add(new(start, 0, 0, null));

            IterateOverNodes(open, closed, endPos, end);

            var finalNode = closed.Find(x => x.Node == end);

            if (finalNode != null)
            {
                TraceParent(finalNode, path);
                // Dodawanie pozycji (ważne żeby było przed skracaniem ścieżki)
                path.Nodes.Insert(0, startPos); // Dodaję pozycję startową
                path.Nodes.Add(endPos); // Dodaję pozycję końcową
                ShortenPath(path);

                // DEBUG
                //for (int i = 0; i < path.Nodes.Count - 1; i++)
                //{
                //    DrawEdge(new Edge(0, path.Nodes[i].Position.ToPoint(),
                //        path.Nodes[i + 1].Position.ToPoint()), Color.green);
                //}
                // DEBUG END

                return path;
            }
            else return null;
        }

        private void AddBoxVertice(BoxCollider2D boxCollider, Vector3 offset)
        {
            Vector2 v = (boxCollider.transform.position + offset).
                RotateAroundPivot(boxCollider.transform.position, boxCollider.transform.rotation.eulerAngles);
            foreach (var item in _vertices)
            {
                if (Vector2.Distance(item, v) < 0.01F) return;
            }
            foreach (var item in _colliders)
            {
                Vector2 cp = item.transform.position;
                if (Vector2.Distance(cp, v) < 0.45F + _navBorder)
                    return;
            }
            _vertices.Add(v);
        }

        private void AddEdgesFromTriangle(Triangle triangle,
            List<(Vector2, Vector2)> validEdges,
            List<(Vector2, Vector2)> invalidEdges,
            Delaunator delaunator)
        {
            bool isValid = true;
            var centroid = delaunator.GetCentroid(triangle.Index);
            foreach (var box in _colliders)
            {
                if (Vector2.Distance(((Point)centroid).ToVector2(), box.transform.position) < 0.05F + 2 * _navBorder)
                {
                    isValid = false;
                    break;
                }
            }

            if (isValid)
            {
                validEdges.Add(new(((Point)triangle.Points.ElementAt(0)).ToVector2(),
                    ((Point)triangle.Points.ElementAt(1)).ToVector2()));
                validEdges.Add(new(((Point)triangle.Points.ElementAt(1)).ToVector2(),
                    ((Point)triangle.Points.ElementAt(2)).ToVector2()));
                validEdges.Add(new(((Point)triangle.Points.ElementAt(2)).ToVector2(),
                    ((Point)triangle.Points.ElementAt(0)).ToVector2()));
            }
            else
            {
                invalidEdges.Add(new(((Point)triangle.Points.ElementAt(0)).ToVector2(),
                    ((Point)triangle.Points.ElementAt(1)).ToVector2()));
                invalidEdges.Add(new(((Point)triangle.Points.ElementAt(1)).ToVector2(),
                    ((Point)triangle.Points.ElementAt(2)).ToVector2()));
                invalidEdges.Add(new(((Point)triangle.Points.ElementAt(2)).ToVector2(),
                    ((Point)triangle.Points.ElementAt(0)).ToVector2()));
            }
        }

        private float CrossProduct2D(Vector2 a, Vector2 b)
        {
            return a.x * b.y - b.x * a.y;
        }

        private void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        private bool Approximately(float a, float b, float tolerance = 1e-5F)
        {
            return Mathf.Abs(a - b) <= tolerance;
        }

        private bool AreLinesIntersecting(Vector2 A, Vector2 B, Vector2 C, Vector2 D, out Vector2 intersection)
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

            float cross_rs = CrossProduct2D(r, s);

            if (Approximately(cross_rs, 0f))
            {
                // Parallel lines
                if (Approximately(CrossProduct2D(qminusp, r), 0f))
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
                float t = CrossProduct2D(qminusp, s) / cross_rs;
                float u = CrossProduct2D(qminusp, r) / cross_rs;
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

        private void Awake()
        {
            _vertices = new List<Vector2>()
            {
                new Vector2(-_navMeshSizeFromCenter, -_navMeshSizeFromCenter),
                new Vector2(_navMeshSizeFromCenter, -_navMeshSizeFromCenter),
                new Vector2(-_navMeshSizeFromCenter, _navMeshSizeFromCenter),
                new Vector2(_navMeshSizeFromCenter, _navMeshSizeFromCenter)
            };
        }

        // DEBUG
        private void DrawEdge(IEdge edge, Color color)
        {
            Debug.DrawLine(((Point)edge.P).ToVector2(), ((Point)edge.Q).ToVector2(),
                color, _lineDrawTime);
        }

        // DEBUG
        private void DrawTriangle(Triangle triangleToDraw, Delaunator delaunator)
        {
            GameObject triangle = new GameObject();
            triangle.transform.parent = transform;
            var mesh = triangle.AddComponent<MeshFilter>().mesh;
            var renderer = triangle.AddComponent<MeshRenderer>();
            mesh.Clear();
            IPoint[] iPoints = new List<IPoint>(triangleToDraw.Points).ToArray();
            Point[] points = new Point[iPoints.Length];
            for (int i = 0; i < iPoints.Length; i++)
                points[i] = (Point)iPoints[i];
            mesh.vertices = new Vector3[]
            {
                points[0].ToVector2(),
                points[1].ToVector2(),
                points[2].ToVector2()
            };
            mesh.uv = new Vector2[]
            {
                points[0].ToVector2(),
                points[1].ToVector2(),
                points[2].ToVector2()
            };
            mesh.triangles = new int[] { 0, 1, 2 };

            System.Random rand = new();

            renderer.material = new Material(_simpleMaterial);
            renderer.material.color = new Color32(0, 0, 127, 0);

            // Random color to triangles
            //renderer.material.color = Color.HSVToRGB(
            //        (float)rand.Next(3600) / 3600,
            //        (float)rand.Next(20, 55) / 100,
            //        (float)rand.Next(85, 95)) / 100;

            var centroid = delaunator.GetCentroid(triangleToDraw.Index);

            foreach (var box in _colliders)
            {
                if (Vector2.Distance(((Point)centroid).ToVector2(), box.transform.position) < 0.05F + 2 * _navBorder)
                {
                    renderer.material.color = Color.black;
                    return;
                }
            }

            //bool isForbidden = false;

            //foreach (var collider in _colliders)
            //{
            //    int i = 0;
            //    foreach (var point in mesh.vertices)
            //    {
            //        if (Vector2.Distance(point, collider.transform.position) > 1.273F)
            //            break;
            //        i++;
            //    }
            //    if (i == 3)
            //    {
            //        isForbidden = true;
            //        break;
            //    }
            //}

            //if (!isForbidden)
            //    renderer.material.color = Color.HSVToRGB(
            //        (float)rand.Next(3600) / 3600,
            //        (float)rand.Next(20, 35) / 100,
            //        (float)rand.Next(85, 95)) / 100;
            //else
            //    renderer.material.color = Color.black;
        }

        // DEBUG
        private void DrawVertice(Vector2 v)
        {
            GameObject vertice = new GameObject();
            vertice.transform.parent = transform;
            var mesh = vertice.AddComponent<MeshFilter>().mesh;
            var renderer = vertice.AddComponent<MeshRenderer>();
            mesh.Clear();
            mesh.vertices = new Vector3[]
            {
                (Vector3)v + new Vector3(_vOffset, _vOffset, -1),
                (Vector3)v + new Vector3(-_vOffset, _vOffset, -1),
                (Vector3)v + new Vector3(_vOffset, -_vOffset, -1),
                (Vector3)v + new Vector3(-_vOffset, -_vOffset, -1)
            };
            mesh.uv = new Vector2[]
            {
                v + new Vector2(_vOffset, _vOffset),
                v + new Vector2(-_vOffset, _vOffset),
                v + new Vector2(_vOffset, -_vOffset),
                v + new Vector2(-_vOffset, -_vOffset)
            };
            mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };

            renderer.material = new Material(_simpleMaterial);
            renderer.material.color = Color.red;
        }

        private Node FindClosestNode(Vector2 pos) =>
            _nodes.OrderBy(x => Vector2.Distance(x.Position, pos))
            .ToArray()[0];

        private void FindNodes(Triangle triangle, Delaunator delaunator)
        {
            var centroid = delaunator.GetCentroid(triangle.Index);
            foreach (var box in _colliders)
            {
                if (Vector2.Distance(
                    ((Point)centroid).ToVector2(), box.transform.position) <
                    0.05F + 2 * _navBorder)
                {
                    return;
                }
            }

            foreach (Point point in triangle.Points)
            {
                _nodesPositions.Add(point.ToVector2());
            }
        }

        private void FindVertices()
        {
            _vertices = new List<Vector2>()
            {
                new Vector2(-_navMeshSizeFromCenter, -_navMeshSizeFromCenter),
                new Vector2(_navMeshSizeFromCenter, -_navMeshSizeFromCenter),
                new Vector2(-_navMeshSizeFromCenter, _navMeshSizeFromCenter),
                new Vector2(_navMeshSizeFromCenter, _navMeshSizeFromCenter)
            };

            float num = 0.5F + _navBorder;

            foreach (var item in _colliders)
            {
                AddBoxVertice(item, new Vector2(-num, -num));
                AddBoxVertice(item, new Vector2(-num, num));
                AddBoxVertice(item, new Vector2(num, -num));
                AddBoxVertice(item, new Vector2(num, num));
                AddBoxVertice(item, new Vector2(-num, 0));
                AddBoxVertice(item, new Vector2(num, 0));
                AddBoxVertice(item, new Vector2(0, -num));
                AddBoxVertice(item, new Vector2(0, num));
            }
        }

        private void FixedUpdate()
        {
            if (_agents.Count > 0)
                UpdateMesh();
            transform.eulerAngles = Vector2.zero;
            transform.parent.GetChild(1).eulerAngles = Vector2.zero;
        }

        private void IterateOverNodes(List<TempNode> open, List<TempNode> closed,
            Vector2 endPos, Node end)
        {
            while (open.Count > 0)
            {
                open = open.OrderBy(x => x.F).ToList();
                TempNode activeNode = open[0];
                open.RemoveAt(0);

                foreach (var connectedNode in activeNode.Node.ConnectedNodes)
                {
                    var successor = new TempNode(connectedNode.Key,
                        connectedNode.Value + activeNode.G,
                        Vector2.Distance(connectedNode.Key.Position, endPos),
                        activeNode);

                    if (successor.Node == end)
                    {
                        closed.Add(activeNode);
                        closed.Add(successor);
                        return;
                    }

                    bool isWorse = false;

                    foreach (TempNode node in open)
                    {
                        if (node.Node == successor.Node)
                            if (node.F < successor.F)
                            {
                                isWorse = true;
                                break;
                            }
                    }
                    foreach (TempNode node in closed)
                    {
                        if (node.Node == successor.Node)
                            if (node.F < successor.F)
                            {
                                isWorse = true;
                                break;
                            }
                    }

                    if (!isWorse)
                        open.Add(successor);
                }

                closed.Add(activeNode);
            }
        }

        private void SetNeighbours((Vector2 A, Vector2 B) edge)
        {
            Vector2 a = edge.A;
            Vector2 b = edge.B;

            Node nodeA = _nodes.Find(x => x.Position == a);
            Node nodeB = _nodes.Find(x => x.Position == b);

            float distance = Vector2.Distance(a, b);
            if (!nodeA.ConnectedNodes.ContainsKey(nodeB))
            {
                nodeA.ConnectedNodes.Add(nodeB, distance);
                nodeB.ConnectedNodes.Add(nodeA, distance);
            }
        }

        private void ShortenWholePath(Path path)
        {
            for (int i = 0; i < path.Count - 2; i++)
            {
                for (int j = path.Count - 1; j > i + 1; j--)
                {
                    if (!isLineObstructed((path[i], path[j])))
                    {
                        path.Nodes.RemoveRange(i + 1, j - i - 1);
                        break;
                    }
                }
            }
        }

        private void ShortenPath(Path path)
        {
            for (int i = path.Count - 1; i >= 1; i--)
            {
                if (!isLineObstructed((path[i], path[0])))
                {
                    path.Nodes.RemoveRange(1, i - 1);
                    break;
                }
            }
        }

        private bool isLineObstructed((Vector2 A, Vector2 B) edge)
        {
            foreach (var invalidEdge in _invalidEdges)
            {
                if (AreLinesIntersecting(edge.A, edge.B, invalidEdge.A, invalidEdge.B,
                    out Vector2 intersection))
                    return true;
            }
            return false;
        }

        private void TraceParent(TempNode node, Path path)
        {
            path.Nodes.Insert(0, node.Node.Position);
            if (node.Parent != null)
                TraceParent(node.Parent, path);
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.T))
            //{
            //    _colliders = FindObjectsOfType<BoxCollider2D>();
            //    UpdateMesh();
            //}
            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    var path = FindPath(new(-20, -20), new(20, 20));
            //    //foreach (var item in path.Nodes)
            //    //    DrawVertice(item.Position);
            //    if (path != null)
            //    {
            //        for (int i = 0; i < path.Nodes.Count - 1; i++)
            //        {
            //            DrawEdge(new Edge(0, path.Nodes[i].Position.ToPoint(),
            //                path.Nodes[i + 1].Position.ToPoint()), Color.green);
            //        }
            //    }
            //    else Debug.Log("Path not found.");
            //}
        }

        public void UpdateMesh()
        {
            _colliders = FindObjectsOfType<BoxCollider2D>();
            FindVertices();

            foreach (Transform item in transform)
                Destroy(item.gameObject);

            float size = _navMeshSizeFromCenter;
            IPoint[] Points = new IPoint[_vertices.Count];
            for (int i = 0; i < Points.Length; i++)
                Points[i] = _vertices[i].ToPoint();
            Delaunator delaunator = new(Points);

            _nodes.Clear();
            _nodesPositions.Clear();
            delaunator.ForEachTriangle(x => FindNodes((Triangle)x, delaunator));
            _nodesPositions = _nodesPositions.Distinct().ToList();
            _nodesPositions = _nodesPositions.OrderBy(x => x.x)
                .ThenBy(x => x.y).ToList();
            foreach (var item in _nodesPositions)
            {
                _nodes.Add(new(item));
            }

            _validEdges = new(); // Do wyszukiwania ścieżki
            _invalidEdges = new(); // Do skracania ścieżki
            delaunator.ForEachTriangle(x => AddEdgesFromTriangle(
                (Triangle)x, _validEdges, _invalidEdges, delaunator));
            _validEdges = _validEdges.Distinct().ToList();
            _invalidEdges = _invalidEdges.Distinct().ToList();
            foreach (var item in _validEdges)
            {
                SetNeighbours(item);
            }
            for (int i = 0; i < _nodes.Count; i++)
            {
                if (_nodes[i].ConnectedNodes.Count == 0)
                {
                    _nodes.RemoveAt(i);
                    i--;
                }
            }

            // DEBUG
            //_nodes.ForEach(x => DrawVertice(x.Position));
            //delaunator.ForEachTriangle(x => DrawTriangle((Triangle)x, delaunator));
            //_vertices.ForEach(x => DrawVertice(x));
            //delaunator.ForEachTriangleEdge(x => DrawEdge(x));
            //_validEdges.ForEach(
            //    x => DrawEdge(new Edge(0, x.A.ToPoint(), x.B.ToPoint()), Color.red));
        }

        private class TempNode
        {
            public TempNode(Node node, float g, float h, TempNode parent)
            {
                Node = node;
                G = g;
                H = h;
                F = G + H;
                Parent = parent;
            }

            public float F { get; }
            public float G { get; set; }
            public float H { get; set; }
            public Node Node { get; }
            public TempNode Parent { get; set; }

            public override string ToString()
            {
                return Node.ToString();
            }
        }
    }
}