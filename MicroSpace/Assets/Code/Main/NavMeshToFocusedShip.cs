using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class NavMeshToFocusedShip : MonoBehaviour
    {
        private void Update()
        {
            if (GameManager.FocusedShip != null)
                if (transform.parent != GameManager.FocusedShip)
                {
                    transform.parent = GameManager.FocusedShip;
                    transform.localPosition = Vector3.zero;
                }
        }
    }
}