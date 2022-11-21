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
        #region Fields

        private NavMeshAgent _navMeshAgent;
        private Transform _target;
        private Rigidbody2D _obstacleRigidbody;
        private List<Vector3> _path = new();
        private Astronaut _astronaut;
        private bool _isTargetClosestPositionValid;
        private NavMeshHit _targetClosestPosition;

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
            _isTargetClosestPositionValid = NavMesh.SamplePosition(
                _target.position, out _targetClosestPosition, 2F, NavMesh.AllAreas);
            if (_isTargetClosestPositionValid)
            {
                NavMeshPath path = new();
                NavMesh.CalculatePath(
                    transform.position,
                    _targetClosestPosition.position,
                    NavMesh.AllAreas,
                    path);
                if (path.corners.Length != 0)
                {
                    _path = path.corners.ToList();
                    _navMeshAgent.SetPath(path);
                }
            }
        }

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

        #endregion Private

        #region Unity

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            PlayerController.DefaultSetNavTarget
                .AddListener(ActionType.Performed, SetTarget);
            _astronaut = GetComponent<Astronaut>();
        }

        private void Update()
        {
            if (_target != null && _isTargetClosestPositionValid)
                Debug.DrawLine(
                    _target.position,
                    _targetClosestPosition.position,
                    Color.green);
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

        #endregion Unity
    }
}