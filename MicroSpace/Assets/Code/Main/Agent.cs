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

        private Transform _target;
        private Rigidbody2D _obstacleRigidbody;
        private List<Vector3> _path = new();
        private Astronaut _astronaut;
        private bool _isTargetClosestPositionValid;
        private bool _isCurrentClosestPositionValid;
        private NavMeshHit _targetClosestPosition;
        private NavMeshHit _currentClosestPosition;
        private float _speed = 5F;
        private Rigidbody2D _rigidbody;
        private CircleCollider2D _collider;
        private Vector2 _deltaPosition;

        #endregion Fields

        #region Public

        public void SetObstacleRigidbody(Rigidbody2D rigidbody)
        {
            _obstacleRigidbody = rigidbody;
        }

        #endregion Public

        #region Private

        private void SetTarget(CallbackContext context)
        {
            var ray = Camera.main.ScreenPointToRay(
                PlayerController.DefaultPoint.ReadValue<Vector2>());
            var hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null)
                _target = hit.collider.transform;
        }

        private void SetDestination()
        {
            Vector2 correction = (transform.position - _target.position).normalized * 0.01F;
            if (Math.Abs(correction.x) > Math.Abs(correction.y))
                correction = new(0.01F * Math.Sign(correction.x), 0F);
            else
                correction = new(0F, 0.01F * Math.Sign(correction.y));
            Vector2 correctedTargetPosition = _target.position + _target.InverseTransformDirection(correction);
            _isCurrentClosestPositionValid = NavMesh.SamplePosition(
                transform.position, out _currentClosestPosition, 2F, NavMesh.AllAreas);
            _isTargetClosestPositionValid = NavMesh.SamplePosition(
                correctedTargetPosition, out _targetClosestPosition, 2F, NavMesh.AllAreas);
            if (_isCurrentClosestPositionValid && _isTargetClosestPositionValid)
            {
                NavMeshPath path = new();
                NavMesh.CalculatePath(
                    _currentClosestPosition.position,
                    _targetClosestPosition.position,
                    NavMesh.AllAreas,
                    path);
                _path = path.corners.ToList();
            }
        }

        private void Move()
        {
            while (_path.Count() > 0)
            {
                if (Vector2.Distance(_rigidbody.position, _path[0]) < 0.2F)
                    _path.RemoveAt(0);
                else
                    break;
            }

            if (_path.Count() == 0)
                return;

            float remainingDisplacement = _speed * Time.fixedDeltaTime;
            while (remainingDisplacement > 0.001F && _path.Count > 0)
            {
                Vector2 startDeltaPosition = _deltaPosition;
                Vector2 direction = ((Vector2)_path[0] - (_rigidbody.position + _deltaPosition));
                Vector2 normalizedDirection = direction.normalized;
                Vector2 velocity = normalizedDirection * remainingDisplacement;
                Vector2 clampedVelocity = Vector2.ClampMagnitude(
                    velocity,
                    Vector2.Distance(_rigidbody.position + _deltaPosition, _path[0]));
                _deltaPosition += clampedVelocity;
                remainingDisplacement -= Vector2.Distance(startDeltaPosition, _deltaPosition);
                if (Vector2.Distance(_rigidbody.position + _deltaPosition, _path[0]) < 0.2F)
                    _path.RemoveAt(0);
            }
        }

        private void AddRelativeDisplacement()
        {
            _deltaPosition += _obstacleRigidbody.velocity * Time.fixedDeltaTime;
        }

        private void DetectObstacle()
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
            if (hit.collider == null)
                _target.TryGetComponentUpInHierarchy(out _obstacleRigidbody);
            else
            {
                hit.collider
                    .TryGetComponentUpInHierarchy(out _obstacleRigidbody);
            }
        }

        #endregion Private

        #region Unity

        private void Start()
        {
            PlayerController.DefaultSetNavTarget
                .AddListener(ActionType.Performed, SetTarget);
            _astronaut = GetComponent<Astronaut>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CircleCollider2D>();
        }

        private void Update()
        {
            if (_target != null && _isTargetClosestPositionValid)
            {
                Debug.DrawLine(
                    _target.position,
                    _targetClosestPosition.position,
                    Color.green);
                for (int i = 0; i < _path.Count - 1; i++)
                {
                    Debug.DrawLine(_path[i], _path[i + 1], Color.yellow);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_target != null)
            {
                SetDestination();
                DetectObstacle();
            }

            if (_obstacleRigidbody != null)
            {
                if (_path.Count() > 1
                    && Vector2.Distance(transform.position, _target.position) > 1.5F)
                {
                    Move();
                }
                AddRelativeDisplacement();
                _rigidbody.velocity = _deltaPosition / Time.fixedDeltaTime;
                _deltaPosition = Vector2.zero;
            }

            if (_obstacleRigidbody != null)
            {
                if (!transform.IsChildOf(_obstacleRigidbody.transform))
                {
                    transform.parent = _obstacleRigidbody.transform;
                    _astronaut.SetParentId(_obstacleRigidbody.GetComponent<Satellite>().Id);
                }
            }
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    OnTriggerStay2D(collision);
        //}

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Wall>(out _))
            {
                var colliderDistance = Physics2D.Distance(_collider, collision);
                _rigidbody.position += colliderDistance.pointB - colliderDistance.pointA;
            }
        }

        #endregion Unity
    }
}