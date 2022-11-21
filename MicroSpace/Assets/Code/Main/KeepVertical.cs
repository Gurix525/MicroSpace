using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class KeepVertical : MonoBehaviour
    {
        private void Update()
        {
            transform.eulerAngles = Camera.main.transform.eulerAngles;
        }
    }
}