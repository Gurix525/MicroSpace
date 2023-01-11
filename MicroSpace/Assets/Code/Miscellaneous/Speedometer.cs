using System;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Miscellaneous
{
    public class Speedometer : MonoBehaviour
    {
        private TextMeshProUGUI _velocityText;

        private void Start()
        {
            _velocityText = GetComponent<TextMeshProUGUI>();
        }

        private void FixedUpdate()
        {
            _velocityText.text = $"{GetVelocity():0.000} m/s";
        }

        private float GetVelocity()
        {
            Rigidbody2D focusedRigidbody = References.FocusedSatellite;
            if (focusedRigidbody == null)
                return 0F;
            return References.Target == null ?
                focusedRigidbody.velocity.magnitude :
                Math.Abs((focusedRigidbody.velocity -
                References.Target.velocity).magnitude);
        }
    }
}