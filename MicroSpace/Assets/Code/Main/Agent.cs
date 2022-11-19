using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.InputSystem.InputAction;

namespace Main
{
    public class Agent : MonoBehaviour
    {
        private NavMeshAgent _navMeshAgent;
        private Transform _target;

        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            PlayerController.DefaultSetNavTarget
                .AddListener(ActionType.Performed, SetTarget);
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

        private void Update()
        {
            if (_target != null && _navMeshAgent.isActiveAndEnabled)
                _navMeshAgent.SetDestination(_target.position);
        }

        private void FixedUpdate()
        {
            if (GameManager.FocusedShip != null &&
                _navMeshAgent.isActiveAndEnabled)
            {
                _navMeshAgent.Move(GameManager.FocusedShipVelocity * Time.fixedDeltaTime);
            }
        }
    }
}