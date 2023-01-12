using System.Collections.Generic;
using System.Linq;
using Entities;
using Maths;
using UnityEngine;

namespace Main
{
    public class Sun : MonoBehaviour
    {
        private int _timer;
        private List<Range> _ranges = new();

        private void FixedUpdate()
        {
            //_timer++;
            //if (_timer < 10)
            //    return;
            //_timer = 0;

            _ranges.Clear();
            IlluminateWalls();
            foreach (var range in _ranges)
            {
                Debug.DrawRay(Vector3.zero, Quaternion.Euler(0, 0, range.Start) * Vector2.up * 20000, Color.magenta);
                Debug.DrawRay(Vector3.zero, Quaternion.Euler(0, 0, range.End) * Vector2.up * 20000, Color.magenta);
            }
        }

        private void IlluminateWalls()
        {
            var sortedWalls = Wall.EnabledWalls
                .OrderBy(wall => wall.transform.position.magnitude);
            foreach (var wall in sortedWalls)
                IlluminateWall(wall);
        }

        private void IlluminateWall(Wall wall)
        {
            Range wallRange = wall.ShadowRange;
            foreach (var range in _ranges)
            {
                if (wallRange.IsCovered(range))
                {
                    wall.SetLightActive(false);
                    return;
                }
            }
            wall.SetLightActive(true);
            _ranges.Add(wallRange);
            OrganiseRanges();
        }

        private void OrganiseRanges()
        {
            _ranges = _ranges.OrderBy(range => range.Start).ToList();
            for (int i = 0; i < _ranges.Count - 1; i++)
            {
                if (_ranges[i].IsOverlapping(_ranges[i + 1], out Range combinedRange))
                {
                    _ranges[i] = combinedRange;
                    _ranges.RemoveAt(i + 1);
                }
            }
        }

        //    float cameraSize = Camera.main != null ?
        //        Camera.main.orthographicSize * Screen.width / Screen.height * 1.2F :
        //        0;
        //    int rayCount = (int)Math.Ceiling(50 * cameraSize / 10);
        //    Vector2 cameraPosition = Camera.main != null ?
        //        Camera.main.transform.position :
        //        Vector2.zero;
        //    Vector2 perpendicular = Vector2.Perpendicular(cameraPosition).normalized;
        //    Vector2[] bounds = new Vector2[2]
        //    {
        //        cameraPosition + perpendicular * cameraSize,
        //        cameraPosition - perpendicular * cameraSize
        //    };
        //    float cameraAngle = Vector2.SignedAngle(Vector2.up, cameraPosition);
        //    float boundsAngle = Vector3.Angle(bounds[0], bounds[1]);
        //    float difference = boundsAngle / rayCount;
        //    float currentAngle = cameraAngle - boundsAngle / 2F + difference;
        //    //for (int i = 0; i < rayCount; i++)
        //    //{
        //    //    Debug.DrawRay(Vector2.zero, (Quaternion.Euler(0, 0, currentAngle) * Vector2.up) * 20000, Color.red);
        //    //    currentAngle += difference;
        //    //}
        //    Dictionary<Collider2D, int> allColliders = new();
        //    for (int i = 0; i < rayCount; i++)
        //    {
        //        RaycastHit2D[] hits = Physics2D.RaycastAll(
        //            Vector2.zero,
        //            Quaternion.Euler(0, 0, currentAngle) * Vector2.up,
        //            20000F,
        //            LayerMask.GetMask("Walls", "Floors", "RigidEntities"));
        //        bool isLight = true;
        //        for (int j = 0; j < hits.Length; j++)
        //        {
        //            if (!allColliders.ContainsKey(hits[j].collider))
        //                allColliders.Add(hits[j].collider, 0);
        //            allColliders[hits[j].collider] += isLight ? 1 : 0;
        //            if (hits[j].collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
        //                isLight = false;
        //            //hits[j].collider.transform.GetChild(0).gameObject.SetActive(isLight);
        //        }
        //        currentAngle += difference;
        //    }
        //    foreach (var collider in allColliders)
        //    {
        //        collider.Key.transform.GetChild(0).gameObject.SetActive(
        //            collider.Value > 0 ? true : false);
        //    }
        //}
    }
}