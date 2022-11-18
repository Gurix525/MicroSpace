using ExtensionMethods;
using Ships;
using System;
using System.Collections;
using UnityEngine;
using ScriptableObjects;
using static UnityEngine.InputSystem.InputAction;
using Attributes;

namespace Main
{
    public class GameManager : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private IdManagerScriptableObject _idManager;

        [SerializeField]
        private GameObject _shipDesignationPrefab;

        [SerializeField]
        private GameObject _wallPrefab;

        [SerializeField]
        private GameObject _shipPrefab;

        [SerializeField]
        private Rigidbody2D _target;

        [SerializeField]
        private Transform _world;

        [SerializeField]
        private static float _speedometer;

        [SerializeField]
        private BuildingManager _buildingManager;

        [SerializeField]
        private float _cameraMovingSpeed = 1F;

        [SerializeField]
        private float _cameraRotationSpeed = 1F;

        [SerializeField]
        [ReadonlyInspector]
        private Rigidbody2D _focusedShipRigidbody = null;

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

        public GameObject ShipPrefab => _shipPrefab;

        public Transform World { get => _world; set => _world = value; }

        public IdManagerScriptableObject IdManager => _idManager;

        public static int FocusedShipId =>
            Instance._focusedShipRigidbody.GetComponent<Ship>().Id;

        public static Vector2 FocusedShipVelocity =>
            Instance._focusedShipRigidbody != null ?
            Instance._focusedShipRigidbody.velocity :
            Vector2.zero;

        public static Quaternion FocusedShipRotation =>
            Instance._focusedShipRigidbody != null ?
            Instance._focusedShipRigidbody.transform.rotation :
            Quaternion.identity;

        #endregion Properties

        #region Public

        public static void SwitchSetup() =>
                    _isSetupRunnning ^= true;

        public static void SelectFocusedShip(GameObject ship)
        {
            if (ship != null)
            {
                Instance._focusedShipRigidbody = ship.GetComponent<Rigidbody2D>();
                AlignCameraToFocusedShip(new());
            }
        }

        public static void SelectTarget(GameObject target)
        {
            if (target != null)
                Instance._target = target.GetComponent<Rigidbody2D>();
            else
                Instance._target = null;
        }

        public static void ForEachShip(Action<Ship> action)
        {
            foreach (Transform child in Instance.World)
                if (child.TryGetComponent(out Ship ship))
                    action(ship);
        }

        #endregion Public

        #region Private

        private void SteerShip()
        {
            Vector3 direction = ((Vector3)PlayerController.SteeringDirection
                .ReadValue<Vector2>())
                .RotateAroundPivot(
                    Vector3.zero,
                    _focusedShipRigidbody.transform.localEulerAngles);
            float speed = 5 * _focusedShipRigidbody.mass;
            float rotationSpeed = _focusedShipRigidbody.mass;

            _focusedShipRigidbody.AddForce(
                direction * speed);

            _focusedShipRigidbody.AddTorque(PlayerController.SteeringRotation
                .ReadValue<float>());

            if (PlayerController.SteeringAdjustSpeed.IsPressed())
                AdjustFocusedShipSpeed(speed * Time.fixedDeltaTime);
        }

        private void SetCameraFree()
        {
            Camera.main.transform.parent = null;
        }

        private void SteerCamera()
        {
            if ((Vector2)Camera.main.transform.localPosition != Vector2.zero &&
                Camera.main.transform.parent != null)
                Camera.main.transform.parent = null;

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

        private void AdjustFocusedShipSpeed(float speed)
        {
            Vector2 desiredVelocity = _target != null ?
                _target.velocity : Vector2.zero;
            var currentVelocity = _focusedShipRigidbody.velocity;
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

            _focusedShipRigidbody.velocity += new Vector2(x, y);
        }

        private void UpdateSpeedometer()
        {
            Speedometer = _target == null ?
                _focusedShipRigidbody.velocity.magnitude :
                Math.Abs((_focusedShipRigidbody.velocity -
                _target.velocity).magnitude);
        }

        private void SwitchPause(CallbackContext context)
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }

        private void Zoom(CallbackContext context)
        {
            Vector2 scrollValue = context.ReadValue<Vector2>();
            if (scrollValue.y > 0 &&
                Camera.main.orthographicSize > 5)
                Camera.main.orthographicSize -= 5;
            else if (scrollValue.y < 0 &&
                Camera.main.orthographicSize < 100)
                Camera.main.orthographicSize += 5;
        }

        private void QuickSave(CallbackContext context)
        {
            SaveManager.SaveGame();
        }

        private void QuickLoad(CallbackContext context)
        {
            SaveManager.LoadGame();
        }

        private void ActivateOrDeactivateShips()
        {
            ForEachShip(ship => ship.ActivateOrDeactivateChildren(
                _focusedShipRigidbody.transform));
        }

        private void EnableSteering(CallbackContext context)
        {
            PlayerController.PlayerInput.SwitchCurrentActionMap("Steering");
            _isSteeringEnabled = true;
            AlignCameraToFocusedShip(new());
        }

        private void DisableSteering(CallbackContext context)
        {
            PlayerController.PlayerInput.SwitchCurrentActionMap("Default");
            _isSteeringEnabled = false;
            SetCameraFree();
        }

        private void EnableBuilding(CallbackContext context)
        {
            PlayerController.PlayerInput.SwitchCurrentActionMap("Building");
            _buildingManager.enabled = true;
        }

        private void DisableBuilding(CallbackContext context)
        {
            _buildingManager.enabled = false;
            PlayerController.PlayerInput.SwitchCurrentActionMap("Default");
        }

        private void SwitchSteeringToBuilding(CallbackContext context)
        {
            DisableSteering(context);
            EnableBuilding(context);
        }

        public static void AlignCameraToFocusedShip(CallbackContext context)
        {
            if (Instance._focusedShipRigidbody == null)
                return;
            Camera.main.transform.parent = Instance._focusedShipRigidbody.transform;
            Camera.main.transform.rotation =
                Instance._focusedShipRigidbody.transform.rotation;
            var shipPos = Instance._focusedShipRigidbody.position;
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
                .AddListener(ActionType.Performed, AlignCameraToFocusedShip);

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
                .AddListener(ActionType.Performed, AlignCameraToFocusedShip);

            PlayerController.BuildingDisableBuilding
                .AddListener(ActionType.Performed, DisableBuilding);
            PlayerController.BuildingZoom
                .AddListener(ActionType.Performed, Zoom);
            PlayerController.BuildingPause
                .AddListener(ActionType.Performed, SwitchPause);
            PlayerController.BuildingAlignCamera
                .AddListener(ActionType.Performed, AlignCameraToFocusedShip);
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
            Instance = this;
            //_designManager = GetComponent<DesignManager>();
        }

        private void Update()
        {
            _currentActionMapName = PlayerController.PlayerInput.currentActionMap.name;

            if (_isSetupRunnning)
                return;

            if (!_isSteeringEnabled)
                SteerCamera();

            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    _designManager.StartCancelDesignation();
            //    return;
            //}

            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    _designManager.StartDesignateMining();
            //    return;
            //}

            //if (Input.GetKeyDown(KeyCode.N))
            //{
            //    StartCoroutine(BuildShipCoroutine());
            //    return;
            //}

            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    _designManager.StartDesignateBlock(BlockType.Floor);
            //    return;
            //}

            //if (Input.GetKeyDown(KeyCode.B))
            //{
            //    _designManager.StartDesignateBlock(BlockType.Wall);
            //    return;
            //}
        }

        private void FixedUpdate()
        {
            if (_focusedShipRigidbody != null)
            {
                if (_isSteeringEnabled)
                    SteerShip();
                UpdateSpeedometer();
                ActivateOrDeactivateShips();
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
    }
}