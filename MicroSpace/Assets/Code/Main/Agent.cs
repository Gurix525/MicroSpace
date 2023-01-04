using Entities;
using ExtensionMethods;
using static UnityEngine.InputSystem.InputAction;
using System.Collections.Generic;
using System.Linq;
using System;
using Tasks;
using UnityEngine.AI;
using UnityEngine;
using Miscellaneous;

namespace Main
{
    public class Agent : MonoBehaviour
    {
        #region Fields

        private Astronaut _astronaut;
        private TaskExecutor _taskExecutor;
        private Rigidbody2D _rigidbody;
        private CircleCollider2D _collider;
        private Transform _target;
        private Rigidbody2D _obstacleRigidbody;
        private List<Vector3> _path = new();
        private float _speed = 5F;
        private Vector2 _deltaPosition;
        private float _pathObstructedTime = 0F;
        private float _pathObstructedTimeLimit = 3F;

        #endregion Fields

        #region Private Properties

        private Astronaut Astronaut =>
            _astronaut ??= GetComponent<Astronaut>();

        private TaskExecutor TaskExecutor =>
            _taskExecutor ??= GetComponent<TaskExecutor>();

        private Rigidbody2D Rigidbody =>
            _rigidbody ??= GetComponent<Rigidbody2D>();

        private CircleCollider2D Collider =>
            _collider ??= GetComponent<CircleCollider2D>();

        private bool IsPathValid => _path.Count > 0;

        private float MaxMoveDisplacement => _speed * Time.fixedDeltaTime;

        #endregion Private Properties

        #region Public

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetObstacleRigidbody(Rigidbody2D rigidbody)
        {
            _obstacleRigidbody = rigidbody;
        }

        #endregion Public

        #region Unity

        private void Start()
        {
            AddEventListeners();
        }

        private void FixedUpdate()
        {
            AssignTargetFromTask();
            GetCurrentPath();
            ChangeRigidbodyVelocity();
            FindClosestObstacleIfNull();
            ChangeParentToObstacle();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            SolveCollision(collision);
        }

        #endregion Unity

        #region Private

        private void FindClosestObstacleIfNull()
        {
            if (_obstacleRigidbody == null)
            {
                var closestSatellite = Satellite.Satellites.Aggregate((a, b) =>
                {
                    if (Vector2.Distance(
                        a.transform.position, transform.position)
                        < Vector2.Distance(
                        b.transform.position, transform.position))
                        return a;
                    else
                        return b;
                });
                _obstacleRigidbody = closestSatellite.GetComponent<Rigidbody2D>();
            }
        }

        private void AssignTargetFromTask()
        {
            if (TaskExecutor.AssignedTask != null)
                _target = TaskExecutor.AssignedTask.Target;
        }

        private void GetCurrentPath()
        {
            if (_target == null)
                return;

            if (
            PathProvider.TryGetPath(
                transform.position,
                _target.position,
                out List<Vector3> newPath,
                _target))
            {
                _path = newPath;
                _pathObstructedTime = 0F;
            }
            else
            {
                _path = new();
                _pathObstructedTime += Time.fixedDeltaTime;
                if (_pathObstructedTime >= _pathObstructedTimeLimit)
                    TaskExecutor.UnassignTask();
            }
            DetectObstacle();
        }

        private void DetectObstacle()
        {
            RaycastHit2D hit = CastRayToDetectObstacle();
            if (hit.collider == null)
                _target.TryGetComponentUpInHierarchy(out _obstacleRigidbody);
            else
            {
                hit.collider
                    .TryGetComponentUpInHierarchy(out _obstacleRigidbody);
            }
        }

        private void AddMoveDisplacement()
        {
            ShortenPathIfPossible();
            if (!IsPathValid)
                return;
            float remainingDisplacement = MaxMoveDisplacement;
            while (remainingDisplacement > 0.001F && _path.Count > 0)
            {
                remainingDisplacement = CalculateMoveDisplacement(ref remainingDisplacement);
            }
        }

        private void AddDisplacementFromObstacle()
        {
            _deltaPosition += _obstacleRigidbody.velocity * Time.fixedDeltaTime;
        }

        private void ChangeRigidbodyVelocity()
        {
            if (_obstacleRigidbody != null)
            {
                if (_target != null)
                    if (_path.Count() > 1
                        && Vector2.Distance(transform.position, _target.position) > 1.5F)
                    {
                        AddMoveDisplacement();
                    }
                AddDisplacementFromObstacle();
                Rigidbody.velocity = _deltaPosition / Time.fixedDeltaTime;
                _deltaPosition = Vector2.zero;
            }
        }

        private float CalculateMoveDisplacement(ref float remainingDisplacement)
        {
            Vector2 startDeltaPosition = _deltaPosition;
            Vector2 clampedVelocity = CalculateDeltaVelocity(remainingDisplacement);
            _deltaPosition += clampedVelocity;
            remainingDisplacement -= Vector2.Distance(startDeltaPosition, _deltaPosition);
            if (Vector2.Distance(Rigidbody.position + _deltaPosition, _path[0]) < 0.2F)
                _path.RemoveAt(0);
            return remainingDisplacement;
        }

        private Vector2 CalculateDeltaVelocity(float remainingDisplacement)
        {
            Vector2 direction = ((Vector2)_path[0] - (Rigidbody.position + _deltaPosition));
            Vector2 normalizedDirection = direction.normalized;
            Vector2 velocity = normalizedDirection * remainingDisplacement;
            Vector2 clampedVelocity = Vector2.ClampMagnitude(
                velocity,
                Vector2.Distance(Rigidbody.position + _deltaPosition, _path[0]));
            return clampedVelocity;
        }

        private void ShortenPathIfPossible()
        {
            while (_path.Count() > 0)
            {
                if (Vector2.Distance(Rigidbody.position, _path[0]) < 0.2F)
                    _path.RemoveAt(0);
                else
                    break;
            }
        }

        private RaycastHit2D CastRayToDetectObstacle()
        {
            bool originalQueriesGitTriggers = Physics2D.queriesHitTriggers;
            bool originalQueriesStartInColliders = Physics2D.queriesStartInColliders;
            Physics2D.queriesHitTriggers = false;
            Physics2D.queriesStartInColliders = true;
            RaycastHit2D hit = Physics2D.CircleCast(
                transform.position, 0.4F, _target.position - transform.position,
                (_target.position - transform.position).magnitude);
            Physics2D.queriesHitTriggers = originalQueriesGitTriggers;
            Physics2D.queriesStartInColliders = originalQueriesStartInColliders;
            return hit;
        }

        private void AddEventListeners()
        {
            //PlayerController.DefaultSetNavTarget
            //                .AddListener(ActionType.Performed, SetTargetFromClick);
            Astronaut.GettingParentId.AddListener(SetAstronautParentId);
        }

        private void ChangeParentToObstacle()
        {
            if (_obstacleRigidbody != null)
            {
                if (!transform.IsChildOf(_obstacleRigidbody.transform))
                {
                    transform.parent = _obstacleRigidbody.transform;
                }
            }
        }

        private void SolveCollision(Collider2D collision)
        {
            if (collision.TryGetComponent<Wall>(out _))
            {
                var colliderDistance = Physics2D.Distance(Collider, collision);
                Rigidbody.position += colliderDistance.pointB - colliderDistance.pointA;
            }
        }

        #endregion Private

        #region Callbacks

        private void SetTargetFromClick(CallbackContext context)
        {
            var ray = Camera.main.ScreenPointToRay(
                PlayerController.DefaultPoint.ReadValue<Vector2>());
            var hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null)
                _target = hit.collider.transform;
        }

        private void SetAstronautParentId()
        {
            Astronaut.SetParentId(_obstacleRigidbody.GetComponent<Satellite>().Id);
        }

        #endregion Callbacks
    }
}