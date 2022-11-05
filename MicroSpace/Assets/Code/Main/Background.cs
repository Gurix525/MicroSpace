using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class Background : MonoBehaviour
    {
        private Vector3 _originalScale = new();
        private float _originalCameraSize = 10;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _originalCameraSize = Camera.main.orthographicSize;
        }

        private void LateUpdate()
        {
            transform.localScale = _originalScale *
                (Camera.main.orthographicSize / _originalCameraSize);
            transform.eulerAngles = Vector3.zero;
        }
    }
}