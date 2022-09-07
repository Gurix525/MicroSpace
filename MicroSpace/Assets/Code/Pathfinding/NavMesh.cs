using Assets.Code.ExtensionMethods;
using DelaunatorSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Code.Pathfinding
{
    public class NavMesh : MonoBehaviour
    {
        [SerializeField] private List<Agent> _agents;
        [SerializeField] private float _navMeshSizeFromCenter = 50;
        [SerializeField] private Material _simpleMaterial;
        [SerializeField] private float _navBorder = 0.5F;
        [SerializeField] private float _verticeSize = 0.1F;
        [SerializeField] private float _lineDrawTime = 5F;

        private List<Vector2> _vertices = new();
        private BoxCollider2D[] _colliders = { };

        private float _vOffset => _verticeSize / 2;

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

        private void FixedUpdate()
        {
            if (_agents.Count > 0)
                UpdateMesh();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                _colliders = FindObjectsOfType<BoxCollider2D>();
                UpdateMesh();
            }
        }

        private void UpdateMesh()
        {
            FindVertices();

            foreach (Transform item in transform)
                Destroy(item.gameObject);

            float size = _navMeshSizeFromCenter;
            IPoint[] Points = new IPoint[_vertices.Count];
            for (int i = 0; i < Points.Length; i++)
                Points[i] = _vertices[i].ToPoint();
            Delaunator delaunator = new(Points);

            // Rysowanie do debugowania
            delaunator.ForEachTriangle(x => DrawTriangle((Triangle)x, delaunator));
            //_vertices.ForEach(x => DrawVertice(x));
            //delaunator.ForEachTriangleEdge(x => DrawEdge(x));
        }

        private void DrawEdge(IEdge edge)
        {
            Debug.DrawLine(((Point)edge.P).ToVector2(), ((Point)edge.Q).ToVector2(),
                Color.red, _lineDrawTime);
        }

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
    }
}