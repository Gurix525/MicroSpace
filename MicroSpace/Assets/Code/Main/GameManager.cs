using ExtensionMethods;
using Entities;
using System;
using System.Collections;
using UnityEngine;
using ScriptableObjects;
using static UnityEngine.InputSystem.InputAction;
using Attributes;
using System.Collections.Generic;
using Miscellaneous;

namespace Main
{
    public class GameManager : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private float _cameraMovingSpeed = 1F;

        [SerializeField]
        private float _cameraRotationSpeed = 1F;

        private static bool _isSteeringEnabled = false;

        #endregion Fields

        #region Public

        public static void SelectFocusedSatellite(GameObject satellite)
        {
            if (satellite != null)
            {
                References.FocusedSatellite = satellite.GetComponent<Rigidbody2D>();
                AlignCameraToFocusedSatellite(new());
            }
        }

        public static void SelectTarget(GameObject target)
        {
            if (target != null)
                References.Target = target.GetComponent<Rigidbody2D>();
            else
                References.Target = null;
        }

        #endregion Public

        #region Private

        private void SteerSatellite()
        {
            var rigidbody = References.FocusedSatellite;
            Vector2 direction = PlayerController.SteeringDirection
                .ReadValue<Vector2>()
                .RotateAroundPivot(
                    Vector2.zero,
                    rigidbody.transform.localEulerAngles.z);
            float speed = 5 * rigidbody.mass;
            float rotationSpeed = rigidbody.inertia;

            rigidbody.AddForce(
                direction * speed);

            rigidbody.AddTorque(PlayerController.SteeringRotation
                .ReadValue<float>() * rotationSpeed);

            if (PlayerController.SteeringAdjustSpeed.IsPressed())
                AdjustFocusedSatelliteSpeed(speed * Time.fixedDeltaTime);
        }

        private void SteerCamera()
        {
            Vector3 direction = (Vector3)PlayerController.DefaultDirection
                .ReadValue<Vector2>() +
                (Vector3)PlayerController.BuildingDirection
                .ReadValue<Vector2>();
            float rotation = PlayerController.DefaultRotation
                .ReadValue<float>() +
                PlayerController.BuildingRotation
                .ReadValue<float>();
            Camera.main.transform.Translate(
                direction * Time.unscaledDeltaTime *
                _cameraMovingSpeed * Camera.main.orthographicSize);
            Camera.main.transform.Rotate(new Vector3(
                0, 0, rotation * Time.unscaledDeltaTime * _cameraRotationSpeed));
        }

        private void AdjustFocusedSatelliteSpeed(float speed)
        {
            Vector2 desiredVelocity = References.Target != null ?
                References.Target.velocity : Vector2.zero;
            var currentVelocity = References.FocusedSatellite.velocity;
            float x = 0;
            float y = 0;

            int xSign = Math.Sign(desiredVelocity.x - currentVelocity.x);
            int ySign = Math.Sign(desiredVelocity.y - currentVelocity.y);

            if (Math.Abs(desiredVelocity.x - currentVelocity.x) >= speed)
                x = speed * xSign;
            else
                x = desiredVelocity.x - currentVelocity.x;

            if (Math.Abs(desiredVelocity.y - currentVelocity.y) >= speed)
                y = speed * ySign;
            else
                y = desiredVelocity.y - currentVelocity.y;

            References.FocusedSatellite.velocity += new Vector2(x, y);
        }

        private void SwitchPause(CallbackContext context)
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }

        private void Zoom(CallbackContext context)
        {
            float newSize = Camera.main.orthographicSize;
            Vector2 scrollValue = context.ReadValue<Vector2>();
            if (scrollValue.y > 0)
                newSize -= 5;
            else if (scrollValue.y < 0)
                newSize += 5;
            newSize = Math.Clamp(newSize, 5, 70);
            Camera.main.orthographicSize = newSize;
        }

        private void QuickSave(CallbackContext context)
        {
            SaveManager.SaveGame();
        }

        private void QuickLoad(CallbackContext context)
        {
            SaveManager.LoadGame();
        }

        private void EnableSteering(CallbackContext context)
        {
            PlayerController.PlayerInput.SwitchCurrentActionMap("Steering");
            _isSteeringEnabled = true;
            AlignCameraToFocusedSatellite(new());
        }

        private void DisableSteering(CallbackContext context)
        {
            PlayerController.PlayerInput.SwitchCurrentActionMap("Default");
            _isSteeringEnabled = false;
        }

        private void EnableBuilding(CallbackContext context)
        {
            PlayerController.PlayerInput.SwitchCurrentActionMap("Building");
            BuildingManager.Instance.enabled = true;
        }

        private void DisableBuilding(CallbackContext context)
        {
            BuildingManager.Instance.enabled = false;
            PlayerController.PlayerInput.SwitchCurrentActionMap("Default");
        }

        private void SwitchSteeringToBuilding(CallbackContext context)
        {
            DisableSteering(context);
            EnableBuilding(context);
        }

        public static void AlignCameraToFocusedSatellite(CallbackContext context)
        {
            if (References.FocusedSatellite == null)
                return;
            Transform focusedSatellite = References.FocusedSatellite.transform;
            Camera.main.transform.parent = focusedSatellite;
            Camera.main.transform.rotation =
                focusedSatellite.rotation;
            var satellitePos = focusedSatellite.position;
            Camera.main.transform.localPosition = new Vector3(0, 0, -10);
        }

        private void SubscribeToInputEvents()
        {
            PlayerController.DefaultPause
                .AddListener(ActionType.Performed, SwitchPause);
            PlayerController.DefaultZoom
                .AddListener(ActionType.Performed, Zoom);
            PlayerController.DefaultQuickLoad
                .AddListener(ActionType.Performed, QuickLoad);
            PlayerController.DefaultQuickSave
                .AddListener(ActionType.Performed, QuickSave);
            PlayerController.DefaultEnableSteering
                .AddListener(ActionType.Performed, EnableSteering);
            PlayerController.DefaultEnableBuilding
                .AddListener(ActionType.Performed, EnableBuilding);
            PlayerController.DefaultAlignCamera
                .AddListener(ActionType.Performed, AlignCameraToFocusedSatellite);

            PlayerController.SteeringPause
                .AddListener(ActionType.Performed, SwitchPause);
            PlayerController.SteeringZoom
                .AddListener(ActionType.Performed, Zoom);
            PlayerController.SteeringQuickSave
                .AddListener(ActionType.Performed, QuickSave);
            PlayerController.SteeringQuickLoad
                .AddListener(ActionType.Performed, QuickLoad);
            PlayerController.SteeringDisableSteering
                .AddListener(ActionType.Performed, DisableSteering);
            PlayerController.SteeringSwitchToBuilding
                .AddListener(ActionType.Performed, SwitchSteeringToBuilding);
            PlayerController.SteeringAlignCamera
                .AddListener(ActionType.Performed, AlignCameraToFocusedSatellite);

            PlayerController.BuildingDisableBuilding
                .AddListener(ActionType.Performed, DisableBuilding);
            PlayerController.BuildingZoom
                .AddListener(ActionType.Performed, Zoom);
            PlayerController.BuildingPause
                .AddListener(ActionType.Performed, SwitchPause);
            PlayerController.BuildingAlignCamera
                .AddListener(ActionType.Performed, AlignCameraToFocusedSatellite);
        }

        //private void UnsubscribeFromInputEvents()
        //{
        //    PlayerController.DefaultPause.performed -= SwitchPause;
        //    PlayerController.DefaultZoom.started -= Zoom;
        //    PlayerController.DefaultQuickLoad.performed -= QuickLoad;
        //    PlayerController.DefaultQuickSave.performed -= QuickSave;
        //    PlayerController.DefaultEnableSteering.performed -= EnableSteering;

        //    PlayerController.SteeringPause.performed -= SwitchPause;
        //    PlayerController.SteeringZoom.performed -= Zoom;
        //    PlayerController.SteeringQuickSave.performed -= QuickSave;
        //    PlayerController.SteeringQuickLoad.performed -= QuickLoad;
        //    PlayerController.SteeringDisableSteering.performed -= DisableSteering;
        //}

        #endregion Private

        #region Unity

        private void Awake()
        {
            Satellite.FirstSatelliteCreated.AddListener(SetFirstFocusedSatellite);
        }

        private void Update()
        {
            if (!_isSteeringEnabled)
                SteerCamera();
        }

        private void FixedUpdate()
        {
            if (References.FocusedSatellite != null)
            {
                if (_isSteeringEnabled)
                    SteerSatellite();
            }
        }

        //private void OnEnable()
        //{
        //    SubscribeToInputEvents();
        //}

        private void Start()
        {
            SubscribeToInputEvents();
        }

        //private void OnDisable()
        //{
        //    UnsubscribeFromInputEvents();
        //}

        #endregion Unity

        #region Callbacks

        private void SetFirstFocusedSatellite(Satellite satellite)
        {
            References.FocusedSatellite ??= satellite.GetComponent<Rigidbody2D>();
            Satellite.FirstSatelliteCreated.RemoveAllListeners();
        }

        #endregion Callbacks
    }
}