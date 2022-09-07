using Assets.Code.ExtensionMethods;
using DelaunatorSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Pathfinding
{
    public class NavMesh : MonoBehaviour
    {
        [SerializeField] private List<Agent> _agents;
        [SerializeField] private float _navMeshSizeFromCenter = 50;
        [SerializeField] private Material _simpleMaterial;
        [SerializeField] private float _navBorder = 0.5f;

        private List<Vector2> _vertices = new();
        private BoxCollider2D[] _colliders = { };

        private void FixedUpdate()
        {
            if (_agents.Count > 0)
                UpdateMesh();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                UpdateMesh();
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

            delaunator.ForEachTriangle(x => DrawTriangle((Triangle)x));
        }

        private void DrawTriangle(Triangle triangleToDraw)
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

            bool isForbidden = false;

            foreach (var collider in _colliders)
            {
                int i = 0;
                foreach (var point in mesh.vertices)
                {
                    if (Vector2.Distance(point, collider.transform.position) > 1.273F)
                        break;
                    i++;
                }
                if (i == 3)
                {
                    isForbidden = true;
                    break;
                }
            }

            if (!isForbidden)
                renderer.material.color = Color.HSVToRGB(
                    (float)rand.Next(3600) / 3600,
                    (float)rand.Next(20, 35) / 100,
                    (float)rand.Next(85, 95)) / 100;
            else
                renderer.material.color = Color.black;
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
            _colliders = FindObjectsOfType<BoxCollider2D>();

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
            _vertices.Add(v);
        }
    }
}