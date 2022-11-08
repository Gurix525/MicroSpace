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
        private UIController _uiController;

        [SerializeField]
        private DesignManager _designManager;

        [SerializeField]
        [ReadonlyInspector]
        private Rigidbody2D _focusedShipRigidbody = null;

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

        #endregion Properties

        #region Public

        public static void SwitchSetup() =>
                    _isSetupRunnning ^= true;

        public static void SelectFocusedShip(GameObject ship)
        {
            if (ship != null)
            {
                //Debug.Log(ship);
                Instance._focusedShipRigidbody = ship.GetComponent<Rigidbody2D>();
                //Database.FocusedShip = Database.DBObjects
                //    .Find(x => x.GameObject == ship);
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

        private void AlignCameraToFocusedShip()
        {
            Camera.main.transform.rotation =
                _focusedShipRigidbody.transform.rotation;
            var shipPos = _focusedShipRigidbody.position;
            Camera.main.transform.position = new Vector3(shipPos.x, shipPos.y, -10);
        }

        private IEnumerator BuildShipCoroutine()
        {
            SwitchSetup();
            GameObject designation = Instantiate(_shipDesignationPrefab, World);
            while (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                MoveShipDesignation(designation);
                yield return null;
            }
            GameObject ship = Instantiate(ShipPrefab, World);
            ship.transform.localPosition = designation.transform.localPosition;
            ship.GetComponent<Rigidbody2D>().velocity =
                _focusedShipRigidbody != null ?
                _focusedShipRigidbody.velocity : Vector2.zero;
            Destroy(designation);
            yield return null;
            SelectFocusedShip(ship);
            Instantiate(_wallPrefab, ship.transform);
            ship.GetComponent<Ship>().UpdateShip();
            SwitchSetup();
        }

        private void MoveShipDesignation(GameObject designation)
        {
            var v3 = Input.mousePosition;
            v3.z = 10;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            designation.transform.position = v3;
        }

        private void SteerTheShip()
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
        }

        private void DisableSteering(CallbackContext context)
        {
            PlayerController.PlayerInput.SwitchCurrentActionMap("Default");
            _isSteeringEnabled = false;
        }

        private void EnableBuilding(CallbackContext context)
        {
            PlayerController.PlayerInput.SwitchCurrentActionMap("Building");
            _designManager.enabled = true;
        }

        private void DisableBuilding(CallbackContext context)
        {
            _designManager.enabled = false;
            PlayerController.PlayerInput.SwitchCurrentActionMap("Default");
        }

        private void SubscribeToInputEvents()
        {
            PlayerController.DefaultPause.performed += SwitchPause;
            PlayerController.DefaultZoom.performed += Zoom;
            PlayerController.DefaultQuickLoad.performed += QuickLoad;
            PlayerController.DefaultQuickSave.performed += QuickSave;
            PlayerController.DefaultEnableSteering.performed += EnableSteering;
            PlayerController.DefaultEnableBuilding.performed += EnableBuilding;

            PlayerController.SteeringPause.performed += SwitchPause;
            PlayerController.SteeringZoom.performed += Zoom;
            PlayerController.SteeringQuickSave.performed += QuickSave;
            PlayerController.SteeringQuickLoad.performed += QuickLoad;
            PlayerController.SteeringDisableSteering.performed += DisableSteering;

            PlayerController.BuildingDisableBuilding.performed += DisableBuilding;
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
            if (_isSetupRunnning)
                return;

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

            //if (Input.GetMouseButtonDown(1))
            //{
            //    _uiController.OpenContextualMenu();
            //    //SelectFocusedShip(null, true);
            //    return;
            //}
        }

        private void FixedUpdate()
        {
            if (_focusedShipRigidbody != null)
            {
                if (_isSteeringEnabled)
                    SteerTheShip();
                AlignCameraToFocusedShip();
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