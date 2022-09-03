using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Pathfinding
{
    public class NavMesh : MonoBehaviour
    {
        [SerializeField] private List<Agent> _agents;

        private void FixedUpdate()
        {
            if (_agents.Count > 0)
                UpdateMesh();
        }

        private void UpdateMesh()
        {
        }
    }
}