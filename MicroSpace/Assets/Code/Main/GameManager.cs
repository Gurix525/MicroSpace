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

        private static Rigidbody2D SelectedShipRigidbody = null;
        public UIController UIController;

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
                SelectedShipRigidbody = ship.GetComponent<Rigidbody2D>();
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

        private void AlignCamera()
        {
            Camera.main.transform.rotation =
                SelectedShipRigidbody.transform.rotation;
        }

        private void AlignScenePosition()
        {
            Vector3 change = SelectedShipRigidbody.transform.localPosition +
                (Vector3)SelectedShipRigidbody.velocity * Time.fixedDeltaTime;
            foreach (Transform child in World)
                child.localPosition -= change;
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
                SelectedShipRigidbody != null ?
                SelectedShipRigidbody.velocity : Vector2.zero;
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
            float speed = 5 * SelectedShipRigidbody.mass;
            float rotationSpeed = speed / 5;

            if (Input.GetKey(KeyCode.W))
                SelectedShipRigidbody.AddForce(
                    SelectedShipRigidbody.transform.up * speed);
            if (Input.GetKey(KeyCode.S))
                SelectedShipRigidbody.AddForce(
                    SelectedShipRigidbody.transform.up * -speed);
            if (Input.GetKey(KeyCode.D))
                SelectedShipRigidbody.AddForce(
                    SelectedShipRigidbody.transform.right * speed);
            if (Input.GetKey(KeyCode.A))
                SelectedShipRigidbody.AddForce(
                    SelectedShipRigidbody.transform.right * -speed);
            if (Input.GetKey(KeyCode.E))
                SelectedShipRigidbody.AddTorque(-rotationSpeed);
            if (Input.GetKey(KeyCode.Q))
                SelectedShipRigidbody.AddTorque(rotationSpeed);
            if (Input.GetKey(KeyCode.Space))
                AdjustSpeed(speed * Time.fixedDeltaTime);
        }

        private void AdjustSpeed(float speed)
        {
            Vector2 desiredVelocity = _target != null ?
                _target.velocity : Vector2.zero;
            var currentVelocity = SelectedShipRigidbody.velocity;
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

            SelectedShipRigidbody.velocity += new Vector2(x, y);
        }

        private void SwitchPause()
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }

        private void UpdateSpeedometer()
        {
            Speedometer = _target == null ?
                SelectedShipRigidbody.velocity.magnitude :
                Math.Abs((SelectedShipRigidbody.velocity -
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
                UIController.OpenContextualMenu();
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
            if (SelectedShipRigidbody != null)
            {
                if (!_isSetupRunnning)
                    SteerTheShip();
                AlignScenePosition();
                AlignCamera();
                UpdateSpeedometer();
                //InstantiateCloseShips();
            }
        }

        #endregion Unity
    }
}