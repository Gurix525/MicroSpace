using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Code.Temporary
{
    public class Agent : MonoBehaviour
    {
        public Transform navigationTarget;

        [SerializeField] private Transform navMesh;
        private NavMeshAgent agent;

        private Vector3 previousPosition;

        // Start is called before the first frame update
        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;

            StartCoroutine(SetDestinationIE());
        }

        private void Update()
        {
            if (navigationTarget == null)
                try { navigationTarget = FindObjectOfType<BoxCollider2D>().transform; }
                catch { }

            if (previousPosition != null)
                transform.position += navMesh.position - previousPosition;
            previousPosition = navMesh.position;
        }

        private void FixedUpdate()
        {
        }

        private IEnumerator SetDestinationIE()
        {
            while (true)
            {
                yield return null;
                if (navigationTarget != null)
                    agent.SetDestination(navigationTarget.position);
            }
        }
    }
}