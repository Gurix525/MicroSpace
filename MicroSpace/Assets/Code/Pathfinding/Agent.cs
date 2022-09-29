using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using DelaunatorSharp;
using Assets.Code.ExtensionMethods;

namespace Assets.Code.Pathfinding
{
    public class Agent : MonoBehaviour
    {
        [SerializeField] private float _movingSpeed = 2F;

        [SerializeField] private Vector2 _targetPosition;
        [SerializeField] private NavMesh _navMesh;
        [SerializeField] private Path _originalPath = new();
        [SerializeField] private Path _adjustedPath = new();

        private List<BoxCollider2D> _colliders;
        private BoxCollider2D _targetCollider;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _colliders = FindObjectsOfType<BoxCollider2D>().ToList();
                float closestColliderDistance = _colliders.Min(x => Vector2.Distance(x.transform.position, GetMousePosition()));
                _targetCollider = _colliders.Find(x => Vector2.Distance(x.transform.position, GetMousePosition()) == closestColliderDistance);
            }

            if (_originalPath.Count > 0)
            {
                for (int i = 0; i < _originalPath.Nodes.Count - 1; i++)
                {
                    DrawEdge(new Edge(0, _originalPath.Nodes[i].ToPoint(),
                        _originalPath.Nodes[i + 1].ToPoint()), Color.green);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_targetCollider != null)
            {
                var hit = Physics2D.Linecast(transform.position, _targetPosition);
                if (hit != false)
                {
                    transform.parent = hit.transform.GetChild(1);
                    _navMesh = hit.transform.GetChild(0).GetComponent<NavMesh>();
                }
                _targetPosition = _targetCollider.transform.position;
                FindPath();
            }
            if (_originalPath.Count > 0)
            {
                _adjustedPath.Nodes.Clear();
                _originalPath.Nodes.ForEach(
                    x => _adjustedPath.Nodes.Add(x));
                //_adjustedPath.Nodes.ForEach(
                //    x => x.Position += (Vector2)_navMesh
                //    .transform.parent.localPosition);

                if (Vector2.Distance(_adjustedPath[0], (Vector2)transform.position) < 0.1F)
                    _adjustedPath.Nodes.RemoveAt(0);
                if (Vector2.Distance(transform.position, _targetPosition) > 1.5F)
                    transform.Translate(
                        (_adjustedPath[0] - (Vector2)transform.position)
                        .normalized * Time.fixedDeltaTime * _movingSpeed);
            }
        }

        private void FindPath()
        {
            _navMesh.UpdateMesh();
            _originalPath = _navMesh.FindPath(transform.position, _targetPosition);
        }

        private void FindStaightPath()
        {
            _originalPath = new Path();
            _originalPath.Nodes.Add(transform.position);
            _originalPath.Nodes.Add(_targetPosition);
        }

        private Vector3 GetMousePosition()
        {
            Vector3 v3 = Input.mousePosition;
            v3.z = 10;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            return v3;
        }

        private void DrawEdge(IEdge edge, Color color)
        {
            Debug.DrawLine(((Point)edge.P).ToVector2(), ((Point)edge.Q).ToVector2(),
                color);
        }
    }
}