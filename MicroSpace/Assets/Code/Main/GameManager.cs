using ExtensionMethods;
using Ships;
using System;
using System.Collections;
using UnityEngine;
using ScriptableObjects;

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

        private static Rigidbody2D _focusedShipRigidbody = null;

        private static bool _isSetupRunnning = false;

        private static DesignManager _designManager;

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

        #endregion Properties

        #region Public

        public static void SwitchSetup() =>
                    _isSetupRunnning ^= true;

        public static void SelectFocusedShip(GameObject ship)
        {
            if (ship != null)
            {
                //Debug.Log(ship);
                _focusedShipRigidbody = ship.GetComponent<Rigidbody2D>();
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

        #endregion Public

        #region Private

        //private void InstantiateCloseShips()
        //{
        //    var shipsToInstantiate = Database.GetShipsToInstantiate();
        //    foreach (var item in shipsToInstantiate)
        //        InstantiateShipFromDB((DatabaseObject)item);
        //}

        private void AlignCameraToFocusedShip()
        {
            Camera.main.transform.rotation =
                _focusedShipRigidbody.transform.rotation;
            var shipPos = _focusedShipRigidbody.position;
            Camera.main.transform.position = new Vector3(shipPos.x, shipPos.y, -10);
        }

        //private void AlignScenePosition()
        //{
        //    Vector3 change = SelectedShipRigidbody.transform.localPosition +
        //        (Vector3)SelectedShipRigidbody.velocity * Time.fixedDeltaTime;
        //    foreach (Transform child in World)
        //        child.localPosition -= change;
        //}

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
            float speed = 5 * _focusedShipRigidbody.mass;
            float rotationSpeed = speed / 5;

            if (Input.GetKey(KeyCode.W))
                _focusedShipRigidbody.AddForce(
                    _focusedShipRigidbody.transform.up * speed);
            if (Input.GetKey(KeyCode.S))
                _focusedShipRigidbody.AddForce(
                    _focusedShipRigidbody.transform.up * -speed);
            if (Input.GetKey(KeyCode.D))
                _focusedShipRigidbody.AddForce(
                    _focusedShipRigidbody.transform.right * speed);
            if (Input.GetKey(KeyCode.A))
                _focusedShipRigidbody.AddForce(
                    _focusedShipRigidbody.transform.right * -speed);
            if (Input.GetKey(KeyCode.E))
                _focusedShipRigidbody.AddTorque(-rotationSpeed);
            if (Input.GetKey(KeyCode.Q))
                _focusedShipRigidbody.AddTorque(rotationSpeed);
            if (Input.GetKey(KeyCode.Space))
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

        private void SwitchPause()
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }

        private void UpdateSpeedometer()
        {
            Speedometer = _target == null ?
                _focusedShipRigidbody.velocity.magnitude :
                Math.Abs((_focusedShipRigidbody.velocity -
                _target.velocity).magnitude);
        }

        #endregion Private

        #region Unity

        private void Awake()
        {
            Instance = this;
            _designManager = GetComponent<DesignManager>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SwitchPause();
                return;
            }

            if (_isSetupRunnning)
                return;

            if (Input.GetKeyDown(KeyCode.C))
            {
                _designManager.StartCancelDesignation();
                return;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                _designManager.StartDesignateMining();
                return;
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                StartCoroutine(BuildShipCoroutine());
                return;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                _designManager.StartDesignateBlock(BlockType.Floor);
                return;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                _designManager.StartDesignateBlock(BlockType.Wall);
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                _uiController.OpenContextualMenu();
                //SelectFocusedShip(null, true);
                return;
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                SaveManager.SaveGame();
                return;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                SaveManager.LoadGame();
                return;
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0 &&
                Camera.main.orthographicSize > 5)
                Camera.main.orthographicSize -= 5;
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 &&
                Camera.main.orthographicSize < 100)
                Camera.main.orthographicSize += 5;
        }

        private void FixedUpdate()
        {
            if (_focusedShipRigidbody != null)
            {
                if (!_isSetupRunnning)
                    SteerTheShip();
                //AlignScenePosition();
                AlignCameraToFocusedShip();
                UpdateSpeedometer();
                ActivateOrDeactivateShips();
            }
        }

        private void ActivateOrDeactivateShips()
        {
            foreach (Transform child in World)
                if (child.TryGetComponent(out Ship ship))
                    ship.ActivateOrDeactivateChildren(
                        _focusedShipRigidbody.transform);
        }

        #endregion Unity
    }
}