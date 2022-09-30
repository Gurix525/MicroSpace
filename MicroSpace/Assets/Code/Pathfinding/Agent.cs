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
        [SerializeField] private Path _path = new();

        private List<BoxCollider2D> _colliders;
        private BoxCollider2D _targetCollider;
        private float _deltaTimeSincePathUpdate = 0F;
        private Vector2 _originalPos;
        private Vector2 _deltaNavMeshPos;

        private bool _hasNavMeshChanged = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _deltaTimeSincePathUpdate = 1F;
                _colliders = FindObjectsOfType<BoxCollider2D>().ToList();
                float closestColliderDistance = _colliders.Min(x => Vector2.Distance(x.transform.position, GetMousePosition()));
                _targetCollider = _colliders.Find(x => Vector2.Distance(x.transform.position, GetMousePosition()) == closestColliderDistance);
            }

            if (_path.Count > 0)
            {
                DrawEdge(new Edge(0, ((Vector2)transform.position).ToPoint(),
                    _path.Nodes[0].ToPoint()), Color.green);
                for (int i = 0; i < _path.Nodes.Count - 1; i++)
                {
                    DrawEdge(new Edge(0, _path.Nodes[i].ToPoint(),
                        _path.Nodes[i + 1].ToPoint()), Color.green);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_path != null)
                UpdatePath();
            if (_targetCollider != null)
            {
                _targetPosition = _targetCollider.transform.position;
                var hit = Physics2D.Linecast(transform.position, _targetPosition);
                if (hit != false)
                {
                    transform.parent = hit.transform.GetChild(1);
                    var newNavMesh = hit.transform.GetChild(0).GetComponent<NavMesh>();
                    if (newNavMesh != _navMesh)
                    {
                        _hasNavMeshChanged = true;
                        _deltaTimeSincePathUpdate = 1F;
                    }
                    _navMesh = newNavMesh;
                }
                if (_deltaTimeSincePathUpdate >= 1F)
                {
                    FindPath();
                    _deltaTimeSincePathUpdate -= 1F;
                }
            }
            if (_navMesh != null)
            {
                if (!_hasNavMeshChanged)
                    _deltaNavMeshPos = (Vector2)transform.parent.parent.position - _originalPos;
                else
                    _hasNavMeshChanged = false;
                _originalPos = transform.parent.parent.position;
            }
            _deltaTimeSincePathUpdate += Time.fixedDeltaTime;
            if (_path.Count > 0)
            {
                //_path.Nodes.Clear();
                //_path.Nodes.ForEach(
                //    x => _path.Nodes.Add(x));
                //_adjustedPath.Nodes.ForEach(
                //    x => x.Position += (Vector2)_navMesh
                //    .transform.parent.localPosition);

                if (Vector2.Distance(_path[0], (Vector2)transform.position) < 0.1F)
                    _path.Nodes.RemoveAt(0);
                if (Vector2.Distance(transform.position, _targetPosition) > 1.5F)
                    transform.Translate(
                        (_path[0] - (Vector2)transform.position)
                        .normalized * Time.fixedDeltaTime * _movingSpeed);
            }
        }

        private void FindPath()
        {
            _navMesh.UpdateMesh();
            _path = _navMesh.FindPath(transform.position, _targetPosition);
        }

        private void UpdatePath()
        {
            for (int i = 0; i < _path.Count; i++)
                _path[i] = _path[i] + _deltaNavMeshPos;
        }

        private void FindStraightPath()
        {
            _path = new Path();
            _path.Nodes.Add(transform.position);
            _path.Nodes.Add(_targetPosition);
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