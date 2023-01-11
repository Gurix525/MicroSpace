using System.Collections;
using System.Collections.Generic;
using Miscellaneous;
using UnityEngine;

namespace Main
{
    public class NavMeshToFocusedSatellite : MonoBehaviour
    {
        private void Update()
        {
            Transform focusedSatellite = References.FocusedSatellite.transform;
            if (focusedSatellite != null)
                if (transform.parent != focusedSatellite)
                {
                    transform.parent = focusedSatellite;
                    transform.localPosition = Vector3.zero;
                }
        }
    }
}