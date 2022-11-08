using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DelaunatorSharp;
using ExtensionMethods;

namespace Pathfinding
{
    public class Agent : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private float _movingSpeed = 2F;

        [SerializeField]
        private float _minDistanceToMove = 1F;

        [SerializeField]
        private float _minDistanceToRemoveNode = 0.2F;

        [SerializeField]
        private float _minDistanceFromCloseShipToChangeShips = 1F;

        private List<BoxCollider2D> _colliders;
        private BoxCollider2D _targetCollider;
        private Vector2 _targetPosition;
        private Vector2 _originalPos;
        private Vector2 _deltaNavMeshPos;
        private Vector3 _originalRotation;
        private Vector3 _deltaNavMeshRotation;
        private NavMesh _navMesh;
        private float _deltaTimeSincePathUpdate = 0F;
        private bool _hasNavMeshChanged = false;
        private Path _path = new();

        #endregion Fields

        #region Private

        /// <summary>
        /// Znajduje ścieżkę
        /// </summary>
        private void FindPath()
        {
            _navMesh.UpdateMesh();
            _path = _navMesh.FindPath(transform.position, _targetPosition);
        }

        /// <summary>
        /// Tworzy ścieżkę prosto do celu (na razie zdezaktywowane bo robi problemy)
        /// </summary>
        private Path FindStraightPath()
        {
            if (_targetCollider != null)
            {
                var hit = Physics2D.Linecast(
                    transform.position, _targetCollider.transform.position);
                if (hit.collider == _targetCollider)
                {
                    var path = new Path(new Vector2[] {
                    transform.position,
                    hit.point });
                    return path;
                }
            }
            return null;
        }

        /// <summary>
        /// Aktualizuje pozycję ścieżki co klatkę
        /// </summary>
        private void UpdatePath()
        {
            for (int i = 0; i < _path.Count; i++)
            {
                _path[i] = _path[i] + _deltaNavMeshPos;
                _path[i] = ((Vector3)_path[i]).RotateAroundPivot(
                    transform.parent.parent.position,
                    _deltaNavMeshRotation);
            }
        }

        /// <summary>
        /// Zwraca pozycję myszki w świecie
        /// </summary>
        /// <returns></returns>
        private Vector3 GetMousePosition()
        {
            Vector3 v3 = Input.mousePosition;
            v3.z = 10;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            return v3;
        }

        /// <summary>
        /// Ustawia cel na podstawie wskazanego punktu
        /// </summary>
        private void SetTarget()
        {
            _deltaTimeSincePathUpdate = 1F;
            _colliders = FindObjectsOfType<BoxCollider2D>().ToList();
            float closestColliderDistance = _colliders.Min(x => Vector2.Distance(x.transform.position, GetMousePosition()));
            _targetCollider = _colliders.Find(x => Vector2.Distance(x.transform.position, GetMousePosition()) == closestColliderDistance);
        }

        /// <summary>
        /// Porusza agentem
        /// </summary>
        private void MoveAgent()
        {
            if (Vector2.Distance(
                _path[0], (Vector2)transform.position) < _minDistanceToRemoveNode &&
                 _path.Count > 1)
                _path.RemoveAt(0);
            if (Vector2.Distance(transform.position, _targetPosition) > _minDistanceToMove)
                transform.Translate(
                    (_path[0] - (Vector2)transform.position)
                    .normalized * Time.fixedDeltaTime * _movingSpeed);
        }

        /// <summary>
        /// Oblicza różnicę między pozycjami statku po klatce
        /// dla aktualizowania pozycji scieżki
        /// </summary>
        private void CalculateNavMeshDeltas()
        {
            if (!_hasNavMeshChanged)
            {
                _deltaNavMeshPos = (Vector2)transform.parent.parent.position - _originalPos;
                _deltaNavMeshRotation = transform.parent.parent.eulerAngles - _originalRotation;
            }
            else
                _hasNavMeshChanged = false;
            _originalPos = transform.parent.parent.position;
            _originalRotation = transform.parent.parent.eulerAngles;
        }

        /// <summary>
        /// Tworzy ścieżkę do celu
        /// </summary>
        private void CreatePath()
        {
            _targetPosition = _targetCollider.transform.position;
            RaycastHit2D hit = new();
            var closeHit = CheckCloseSurroundings(transform.position);
            if (closeHit.collider == null)
                hit = Physics2D.Linecast(transform.position, _targetPosition);
            if (hit.collider != null)
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
                //_path = FindStraightPath();
                //if (_path == null)
                FindPath();
                _deltaTimeSincePathUpdate -= 1F;
            }
        }

        private RaycastHit2D CheckCloseSurroundings(Vector3 position)
        {
            RaycastHit2D lastHit = new();
            for (int i = 0; i < 360; i += 45)
            {
                var a = Vector3.up * _minDistanceFromCloseShipToChangeShips;
                var hit = Physics2D.Linecast(
                    position,
                    position + (Vector3.up * _minDistanceFromCloseShipToChangeShips)
                    .RotateAroundPivot(Vector3.zero, new(0, 0, i)));
                if (hit.collider != null) return hit;
                lastHit = hit;
            }
            return lastHit;
        }

        #endregion Private

        #region Unity

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.F))
            //{
            //    SetTarget();
            //}

            // Debug
            if (_path.Count > 0)
            {
                DrawEdge(new Edge(0, ((Vector2)transform.position).ToPoint(),
                    _path[0].ToPoint()), Color.green);
                for (int i = 0; i < _path.Count - 1; i++)
                {
                    DrawEdge(new Edge(0, _path[i].ToPoint(),
                        _path[i + 1].ToPoint()), Color.green);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_path != null)
                UpdatePath();
            if (_targetCollider != null)
                CreatePath();
            if (_navMesh != null)
                CalculateNavMeshDeltas();
            if (_path.Count > 0)
                MoveAgent();
            _deltaTimeSincePathUpdate += Time.fixedDeltaTime;
        }

        #endregion Unity

        #region Debug

        private void DrawEdge(IEdge edge, Color color)
        {
            Debug.DrawLine(((Point)edge.P).ToVector2(), ((Point)edge.Q).ToVector2(),
                color);
        }

        #endregion Debug
    }
}