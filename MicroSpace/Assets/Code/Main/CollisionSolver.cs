using Entities;
using UnityEngine;
using UnityEngine.AI;

namespace Main
{
    public class CollisionSolver : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;

        private Rigidbody2D Rigidbody =>
            _rigidbody ??= GetComponent<Rigidbody2D>();

        private void OnTriggerStay2D(Collider2D collision)
        {
            SolveCollision(collision);
        }

        private void SolveCollision(Collider2D collision)
        {
            if (collision.TryGetComponent<Wall>(out _))
            {
                NavMesh.SamplePosition(
                    transform.position,
                    out NavMeshHit closestHit,
                    float.PositiveInfinity,
                    NavMesh.AllAreas);
                Rigidbody.position = closestHit.position;
            }
        }
    }
}