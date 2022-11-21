using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.InputSystem.InputAction;
using ExtensionMethods;
using System.Linq;
using Ships;

namespace Main
{
    public class Agent : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Transform _target;
        private Rigidbody2D _obstacleRigidbody;
        private List<Vector3> _path = new();
        private Astronaut _astronaut;

        public void SetObstacleRigidbody(Rigidbody2D rigidbody)
        {
            _obstacleRigidbody = rigidbody;
        }

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            PlayerController.DefaultSetNavTarget
                .AddListener(ActionType.Performed, SetTarget);
            _astronaut = GetComponent<Astronaut>();
        }

        private void SetTarget(CallbackContext context)
        {
            var ray = Camera.main.ScreenPointToRay(
                PlayerController.DefaultPoint.ReadValue<Vector2>());
            var hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null)
                _target = hit.collider.transform;
            Debug.Log(_target);
        }

        private void SetDestination()
        {
            NavMeshPath path = new();
            NavMesh.CalculatePath(transform.position,
                new Vector3(_target.position.x, _target.position.y, 0.3F),
                NavMesh.AllAreas, path);
            if (path.corners.Length != 0)
            {
                _path = path.corners.ToList();
                _navMeshAgent.SetPath(path);
            }
            //_navMeshAgent.SetPath(_path);
        }

        private void FixedUpdate()
        {
            if (_target != null
                && _navMeshAgent.isActiveAndEnabled)
                SetDestination();

            if (_obstacleRigidbody != null &&
                _navMeshAgent.isActiveAndEnabled)
            {
                AddRelativeDisplacement();
                //Move();
            }

            if (_target != null)
                DetectObstacle();
            if (_obstacleRigidbody != null)
                if (!transform.IsChildOf(_obstacleRigidbody.transform))
                {
                    transform.parent = _obstacleRigidbody.transform;
                    _astronaut.SetParentId(_obstacleRigidbody.GetComponent<Ship>().Id);
                }
        }

        //private void Move()
        //{
        //    if (_path.Count > 0)
        //    {
        //        var direction = ((Vector2)_path[0] - (Vector2)transform.position);
        //        var normalizedDirection = direction.normalized;
        //        direction = Vector2.ClampMagnitude(direction, normalizedDirection.magnitude);
        //        _navMeshAgent.Move(direction * _speed * Time.fixedDeltaTime);
        //        if (Vector3.Distance(transform.position, _path[0]) < 1)
        //            _path.RemoveAt(0);
        //    }
        //}

        private void AddRelativeDisplacement()
        {
            _navMeshAgent.Move(
                _obstacleRigidbody.velocity * Time.fixedDeltaTime);
        }

        private void DetectObstacle()
        {
            RaycastHit2D hit = Physics2D.Linecast(
                transform.position, _target.position);
            if (hit.collider == null)
                _target.TryGetComponentUpInHierarchy(out _obstacleRigidbody);
            else
            {
                if (hit.collider
                    .TryGetComponentUpInHierarchy(out Rigidbody2D rigidbody))
                {
                    _obstacleRigidbody = rigidbody;
                }
            }
        }
    }
}