using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.InputSystem.InputAction;
using ExtensionMethods;
using System.Linq;
using Entities;

namespace Main
{
    public class Agent : MonoBehaviour
    {
        #region Fields

        private Astronaut _astronaut;
        private Rigidbody2D _rigidbody;
        private CircleCollider2D _collider;
        private Transform _target;
        private Rigidbody2D _obstacleRigidbody;
        private List<Vector3> _path = new();
        private NavMeshHit _targetClosestPosition;
        private NavMeshHit _currentClosestPosition;
        private float _speed = 5F;
        private Vector2 _deltaPosition;

        #endregion Fields

        #region Private Properties

        private Astronaut Astronaut =>
            _astronaut ??= GetComponent<Astronaut>();

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
            GetCurrentPath();
            ChangeRigidbodyVelocity();
            ChangeParentToObstacle();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            SolveCollision(collision);
        }

        #endregion Unity

        #region Private

        private void GetCurrentPath()
        {
            if (_target != null)
            {
                CreatePathIfPossible();
                DetectObstacle();
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

        private void CreatePath()
        {
            NavMeshPath path = new();
            NavMesh.CalculatePath(
                _currentClosestPosition.position,
                _targetClosestPosition.position,
                NavMesh.AllAreas,
                path);
            _path = path.corners.ToList();
        }

        private void ChangeRigidbodyVelocity()
        {
            if (_obstacleRigidbody != null)
            {
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

        private bool IsPathPossibleToCreate(Vector2 correctedTargetPosition)
        {
            bool isCurrentClosestPositionValid = CheckIfPositionIsValid(
                transform.position,
                out _currentClosestPosition);
            bool isTargetClosestPositionValid = CheckIfPositionIsValid(
                correctedTargetPosition,
                out _targetClosestPosition);
            return isCurrentClosestPositionValid && isTargetClosestPositionValid;
        }

        private bool CheckIfPositionIsValid(Vector2 position, out NavMeshHit closestPosition)
        {
            return NavMesh.SamplePosition(
                            position, out closestPosition, 2F, NavMesh.AllAreas);
        }

        private Vector2 CorrectTargetPosition(ref Vector2 correction)
        {
            return _target.position
                + _target.InverseTransformDirection(correction);
        }

        private Vector2 CreateCorrection()
        {
            Vector2 correction = (transform.position - _target.position).normalized * 0.01F;
            if (Math.Abs(correction.x) > Math.Abs(correction.y))
                correction = new(0.01F * Math.Sign(correction.x), 0F);
            else
                correction = new(0F, 0.01F * Math.Sign(correction.y));
            return correction;
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
            PlayerController.DefaultSetNavTarget
                            .AddListener(ActionType.Performed, SetTargetFromClick);
            Astronaut.GettingParentId.AddListener(OnAstronautGettingParentId);
        }

        private void CreatePathIfPossible()
        {
            Vector2 targetPositionCorrection = CreateCorrection();
            Vector2 correctedTargetPosition = CorrectTargetPosition(ref targetPositionCorrection);
            if (IsPathPossibleToCreate(correctedTargetPosition))
                CreatePath();
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

        private void OnAstronautGettingParentId()
        {
            Astronaut.SetParentId(_obstacleRigidbody.GetComponent<Satellite>().Id);
        }

        #endregion Callbacks
    }
}