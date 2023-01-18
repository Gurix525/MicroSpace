using System;
using Maths;
using Miscellaneous;
using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    public class PointerSource : MonoBehaviour
    {
        private GameObject _pointer;
        private Image _image;
        private float _hue = 0;

        private readonly float _greaterBound = 0.99F;
        private readonly float _lesserBound = 0.01F;

        private void Awake()
        {
            _pointer = Instantiate(Prefabs.Pointer, References.Pointers);
            _image = _pointer.GetComponent<Image>();
        }

        private void Update()
        {
            if (Camera.main == null)
                return;
            Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
            if (viewportPosition.x < 0
                || viewportPosition.x > 1
                || viewportPosition.y < 0
                || viewportPosition.y > 1)
            {
                _pointer.SetActive(true);
                SetPointerPosition(viewportPosition);
                ChangePointerColor();
            }
            else
                _pointer.SetActive(false);
        }

        private void OnDestroy()
        {
            Destroy(_pointer);
        }

        private void ChangePointerColor()
        {
            _image.color = Color.HSVToRGB(_hue, 1, 1);
            _hue = _hue >= 1 ? 0 : _hue + 0.001F;
        }

        private void SetPointerPosition(Vector2 viewportPosition)
        {
            Vector2 direction =
                ((Vector2)transform.position
                - (Vector2)Camera.main.transform.position)
                .normalized;
            _pointer.transform.eulerAngles = new(0, 0, Vector2.SignedAngle(Vector2.up, direction) + 45);
            Vector2 intersection = GetCameraIntersection(viewportPosition);
            _pointer.transform.position = intersection;
        }

        private Vector2 GetCameraIntersection(Vector2 viewportPosition)
        {
            Line lineOfSight = new(
                Camera.main.WorldToScreenPoint(transform.position),
                Camera.main.ViewportToScreenPoint(new(0.5F, 0.5F, 0)));
            if (Math.Abs(viewportPosition.x - 0.5F) >= Math.Abs(viewportPosition.y - 0.5F))
            {
                if (viewportPosition.x >= 0.5F)
                {
                    lineOfSight.IsIntersecting(
                        new Line(
                            Camera.main.ViewportToScreenPoint(new(_greaterBound, _lesserBound, 0)),
                            Camera.main.ViewportToScreenPoint(new(_greaterBound, _greaterBound, 0))),
                        out Vector2 intersection);
                    return intersection;
                }
                else
                {
                    lineOfSight.IsIntersecting(
                        new Line(
                            Camera.main.ViewportToScreenPoint(new(_lesserBound, _lesserBound, 0)),
                            Camera.main.ViewportToScreenPoint(new(_lesserBound, _greaterBound, 0))),
                        out Vector2 intersection);
                    return intersection;
                }
            }
            else
            {
                if (viewportPosition.y >= 0.5F)
                {
                    lineOfSight.IsIntersecting(
                        new Line(
                            Camera.main.ViewportToScreenPoint(new(_lesserBound, _greaterBound, 0)),
                            Camera.main.ViewportToScreenPoint(new(_greaterBound, _greaterBound, 0))),
                        out Vector2 intersection);
                    return intersection;
                }
                else
                {
                    lineOfSight.IsIntersecting(
                        new Line(
                            Camera.main.ViewportToScreenPoint(new(_lesserBound, _lesserBound, 0)),
                            Camera.main.ViewportToScreenPoint(new(_greaterBound, _lesserBound, 0))),
                        out Vector2 intersection);
                    return intersection;
                }
            }
        }
    }
}