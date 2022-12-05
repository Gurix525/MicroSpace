using ExtensionMethods;
using Entities;
using System;
using System.Collections;
using UnityEngine;
using ScriptableObjects;
using static UnityEngine.InputSystem.InputAction;
using Attributes;
using System.Collections.Generic;

namespace Main
{
    public class GameManager : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private GameObject _astronautPrefab;

        [SerializeField]
        private GameObject _navMeshPrefab;

        [SerializeField]
        private Rigidbody2D _target;

        [SerializeField]
        private Transform _world;

        [SerializeField]
        private static float _speedometer;

        [SerializeField]
        private float _cameraMovingSpeed = 1F;

        [SerializeField]
        private float _cameraRotationSpeed = 1F;

        [SerializeField]
        [ReadonlyInspector]
        private Rigidbody2D _focusedSatelliteRigidbody = null;

        [SerializeField]
        [ReadonlyInspector]
        private string _currentActionMapName = "";

        private static bool _isSteeringEnabled = false;

        private static bool _isSetupRunnning = false;

        #endregion Fields

        #region Properties

        public static GameManager Instance { get; private set; }

        public static float Speedometer
        {
            get => _speedometer;
            private set => _speedometer = value;
        }

        public static Transform World
        { get => Instance._world; set => Instance._world = value; }

        public static Transform FocusedSatellite =>
            Instance._focusedSatelliteRigidbody != null ?
            Instance._focusedSatelliteRigidbody.transform :
            null;

        public static int FocusedSatelliteId =>
            Instance._focusedSatelliteRigidbody != null ?
            Instance._focusedSatelliteRigidbody.GetComponent<Satellite>().Id :
            0;

        public static Vector2 FocusedSatelliteVelocity =>
            Instance._focusedSatelliteRigidbody != null ?
            Instance._focusedSatelliteRigidbody.velocity :
            Vector2.zero;

        public static Quaternion FocusedSatelliteRotation =>
            Instance._focusedSatelliteRigidbody != null ?
            Instance._focusedSatelliteRigidbody.transform.rotation :
            Quaternion.identity;

        public GameObject AstronautPrefab => _astronautPrefab;

        public GameObject NavMeshPrefab => _navMeshPrefab;

        #endregion Properties

        #region Public

        public static void SimulatePhysics()
        {
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.Simulate(0.000001F);
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        }

        public static void SwitchSetup() =>
                    _isSetupRunnning ^= true;

        public static void SelectFocusedSatellite(GameObject satellite)
        {
            if (satellite != null)
            {
                Instance._focusedSatelliteRigidbody = satellite.GetComponent<Rigidbody2D>();
                AlignCameraToFocusedSatellite(new());
            }
        }

        public static void SelectTarget(GameObject target)
        {
            if (target != null)
                Instance._target = target.GetComponent<Rigidbody2D>();
            else
                Instance._target = null;
        }

        public static void ForEachSatellite(Action<Satellite> action)
        {
            foreach (Satellite satellite in Satellite.Satellites)
                action(satellite);
        }

        #endregion Public

        #region Private

        private void SteerSatellite()
        {
            Vector2 direction = PlayerController.SteeringDirection
                .ReadValue<Vector2>()
                .RotateAroundPivot(
                    Vector2.zero,
                    _focusedSatelliteRigidbody.transform.localEulerAngles.z);
            float speed = 5 * _focusedSatelliteRigidbody.mass;
            float rotationSpeed = _focusedSatelliteRigidbody.inertia;

            _focusedSatelliteRigidbody.AddForce(
                direction * speed);

            _focusedSatelliteRigidbody.AddTorque(PlayerController.SteeringRotation
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
            Vector2 desiredVelocity = _target != null ?
                _target.velocity : Vector2.zero;
            var currentVelocity = _focusedSatelliteRigidbody.velocity;
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

            _focusedSatelliteRigidbody.velocity += new Vector2(x, y);
        }

        private void UpdateSpeedometer()
        {
            Speedometer = _target == null ?
                _focusedSatelliteRigidbody.velocity.magnitude :
                Math.Abs((_focusedSatelliteRigidbody.velocity -
                _target.velocity).magnitude);
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

        private void ActivateOrDeactivateSatellites()
        {
            ForEachSatellite(satellite => satellite.ActivateOrDeactivateChildren(
                _focusedSatelliteRigidbody.transform));
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
            if (Instance._focusedSatelliteRigidbody == null)
                return;
            Camera.main.transform.parent = Instance._focusedSatelliteRigidbody.transform;
            Camera.main.transform.rotation =
                Instance._focusedSatelliteRigidbody.transform.rotation;
            var satellitePos = Instance._focusedSatelliteRigidbody.position;
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
            Instance = this;
        }

        private void Update()
        {
            _currentActionMapName = PlayerController.PlayerInput.currentActionMap.name;

            if (_isSetupRunnning)
                return;

            if (!_isSteeringEnabled)
                SteerCamera();
        }

        private void FixedUpdate()
        {
            if (_focusedSatelliteRigidbody != null)
            {
                if (_isSteeringEnabled)
                    SteerSatellite();
                UpdateSpeedometer();
                ActivateOrDeactivateSatellites();
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
            _focusedSatelliteRigidbody ??= satellite.GetComponent<Rigidbody2D>();
            Satellite.FirstSatelliteCreated.RemoveAllListeners();
        }

        #endregion Callbacks
    }
}