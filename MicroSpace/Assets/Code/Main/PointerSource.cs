using System;
using Maths;
using Miscellaneous;
using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    public class PointerSource : MonoBehaviour
    {
        #region Fields

        private GameObject _pointer;
        private Image _image;

        #endregion Fields

        #region Unity

        private void Awake()
        {
            _pointer = Instantiate(Prefabs.Pointer, References.Pointers);
            _image = _pointer.GetComponent<Image>();
        }

        private void Update()
        {
            if (Camera.main == null)
                return;
            ControlPointer();
        }

        private void OnDestroy()
        {
            Destroy(_pointer);
        }

        #endregion Unity

        #region Private

        private void ControlPointer()
        {
            Vector2 viewportPosition = Camera.main
                .WorldToViewportPoint(transform.position);
            float distanceFromCamera = Vector2
                .Distance(transform.position, Camera.main.transform.position);
            if (distanceFromCamera < 500)
            {
                if (viewportPosition.x < 0
                    || viewportPosition.x > 1
                    || viewportPosition.y < 0
                    || viewportPosition.y > 1)
                {
                    _pointer.SetActive(true);
                    SetPointerPosition(viewportPosition);
                    ChangePointerColor(distanceFromCamera);
                    return;
                }
            }
            _pointer.SetActive(false);
        }

        private void ChangePointerColor(float distanceFromCamera)
        {
            _image.color = new(
                1, 1, 1,
                Prefabs.PointerCurve.Evaluate(distanceFromCamera / 500));
        }

        private void SetPointerPosition(Vector2 viewportPosition)
        {
            Vector2 direction =
                ((Vector2)transform.position
                - (Vector2)Camera.main.transform.position)
                .normalized;
            _pointer.transform.eulerAngles = new(
                0, 0, Vector2.SignedAngle(Vector2.up, direction) + 45);
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
                            new(
                                Screen.width - 16F,
                                0),
                            new(
                                Screen.width - 16F,
                                Screen.height)),
                        out Vector2 intersection);
                    return intersection;
                }
                else
                {
                    lineOfSight.IsIntersecting(
                        new Line(
                            new(
                                16F,
                                00),
                            new(
                                16F,
                                Screen.height)),
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
                            new(
                                0,
                                Screen.height - 16F),
                            new(
                                Screen.width,
                                Screen.height - 16F)),
                        out Vector2 intersection);
                    return intersection;
                }
                else
                {
                    lineOfSight.IsIntersecting(
                        new Line(
                            new(
                                0,
                                16F),
                            new(
                                Screen.width,
                                16F)),
                        out Vector2 intersection);
                    return intersection;
                }
            }
        }

        #endregion Private
    }
}