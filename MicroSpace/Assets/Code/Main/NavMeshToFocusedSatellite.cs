using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class NavMeshToFocusedSatellite : MonoBehaviour
    {
        private void Update()
        {
            if (GameManager.FocusedSatellite != null)
                if (transform.parent != GameManager.FocusedSatellite)
                {
                    transform.parent = GameManager.FocusedSatellite;
                    transform.localPosition = Vector3.zero;
                }
        }
    }
}