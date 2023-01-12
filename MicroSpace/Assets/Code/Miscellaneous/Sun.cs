using System;
using UnityEngine;

namespace Miscellaneous
{
    public class Sun : MonoBehaviour
    {
        private int _timer;

        private void FixedUpdate()
        {
            _timer++;
            if (_timer < 10)
                return;
            _timer = 0;

            float cameraSize = Camera.main != null ?
                Camera.main.orthographicSize * Screen.width / Screen.height * 1.2F :
                0;
            int rayCount = (int)Math.Ceiling(50 * cameraSize / 10);
            Vector2 cameraPosition = Camera.main != null ?
                Camera.main.transform.position :
                Vector2.zero;
            Vector2 perpendicular = Vector2.Perpendicular(cameraPosition).normalized;
            Vector2[] bounds = new Vector2[2]
            {
                cameraPosition + perpendicular * cameraSize,
                cameraPosition - perpendicular * cameraSize
            };
            float cameraAngle = Vector2.SignedAngle(Vector2.up, cameraPosition);
            float boundsAngle = Vector3.Angle(bounds[0], bounds[1]);
            float difference = boundsAngle / rayCount;
            float currentAngle = cameraAngle - boundsAngle / 2F + difference;
            //for (int i = 0; i < rayCount; i++)
            //{
            //    Debug.DrawRay(Vector2.zero, (Quaternion.Euler(0, 0, currentAngle) * Vector2.up) * 20000, Color.red);
            //    currentAngle += difference;
            //}
            for (int i = 0; i < rayCount; i++)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(
                    Vector2.zero,
                    Quaternion.Euler(0, 0, currentAngle) * Vector2.up,
                    20000F,
                    LayerMask.GetMask("Walls", "Floors", "RigidEntities"));
                bool isLight = true;
                for (int j = 0; j < hits.Length; j++)
                {
                    hits[j].collider.transform.GetChild(0).gameObject.SetActive(isLight);
                    if (hits[j].collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
                        isLight = false;
                }
                currentAngle += difference;
            }
        }
    }
}