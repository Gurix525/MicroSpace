using Assets.Code.Data;
using Assets.Code.ExtensionMethods;
using Assets.Code.Ships;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Main
{
    public class Cockpit : MonoBehaviour
    {
        #region Fields

        private Rigidbody2D SelectedShipRigidbody = null;
        public GameObject ShipDesignationPrefab;
        public GameObject ShipPrefab;
        public UIController UIController;
        public float Speedometer; // For UI speedometer purposes

        [SerializeField] private Rigidbody2D _target;
        [SerializeField] private Transform _world;

        private bool _isSetupRunnning = false;

        // Singletons
        private static Cockpit _cockpit;

        private static DesignManager _designManager;

        #endregion Fields

        #region Public

        public void SwitchSetup() => // Used to switch setup on/off
                    _isSetupRunnning ^= true;

        public void SelectFocusedShip(GameObject ship)
        {
            if (ship != null)
            {
                //Debug.Log(ship);
                SelectedShipRigidbody = ship.GetComponent<Rigidbody2D>();
                Database.FocusedShip = Database.DBObjects
                    .Find(x => x.GameObject == ship);
            }
        }

        public void SelectTarget(GameObject target)
        {
            if (target != null)
                _target = target.GetComponent<Rigidbody2D>();
            else
                _target = null;
        }

        public static void InstantiateShipFromDB(DBObject dbo)
        {
            GameObject ship = Instantiate(
                _cockpit.ShipPrefab, GameObject.Find("World").transform);
            dbo.GameObject = ship;
            ship.transform.position = (Vector3)(
                dbo.Position - Database.FocusedShip.Position);
            ship.name = dbo.Name;
            ship.GetComponent<Ship>().DBObject = dbo;
            var rb = ship.GetComponent<Rigidbody2D>();
            rb.velocity = dbo.Velocity;
            rb.rotation = dbo.Rotation;
            rb.angularVelocity = dbo.AngularVelocity;

            foreach (WallData wallData in dbo.ShipData.Walls)
            {
                if (wallData.Name == "Core")
                    continue;
                GameObject wall = Instantiate(
                    _designManager.WallPrefab, ship.transform);
                wall.transform.localPosition = new Vector2(
                     wallData.LocalPosition[0], wallData.LocalPosition[1]);
                wall.name = wallData.Name;
                var wallComponent = wall.GetComponent<Wall>();
                wallComponent.WallData = wallData;
            }

            foreach (FloorData floorData in dbo.ShipData.Floors)
            {
                GameObject floor = Instantiate(
                    _designManager.FloorPrefab, ship.transform);
                floor.transform.localPosition = new Vector2(
                    floorData.LocalPosition[0], floorData.LocalPosition[1]);
                floor.name = floorData.Name;
                var floorComponent = floor.GetComponent<Floor>();
                floorComponent.FloorData = floorData;
                floorData.Room = dbo.ShipData.Rooms
                    .Find(x => x.Id == floorData.Room.Id);
            }
        }

        #endregion Public

        #region Private

        private void InstantiateCloseShips()
        {
            var shipsToInstantiate = Database.GetShipsToInstantiate();
            foreach (var item in shipsToInstantiate)
                InstantiateShipFromDB((DBObject)item);
        }

        private void AlignCamera()
        {
            Camera.main.transform.rotation =
                SelectedShipRigidbody.transform.rotation;
        }

        private void AlignScenePosition()
        {
            Vector3 change = SelectedShipRigidbody.transform.localPosition +
                (Vector3)SelectedShipRigidbody.velocity * Time.fixedDeltaTime;
            foreach (Transform child in _world)
                child.localPosition -= change;
        }

        private IEnumerator BuildShipCoroutine()
        {
            SwitchSetup();
            //SwitchPause();
            GameObject designation = Instantiate(ShipDesignationPrefab, _world);
            while (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                MoveShipDesignation(designation);
                yield return null;
            }
            GameObject ship = Instantiate(ShipPrefab, _world);
            ship.transform.localPosition = designation.transform.localPosition;
            ship.GetComponent<Rigidbody2D>().velocity =
                SelectedShipRigidbody != null ?
                SelectedShipRigidbody.velocity : Vector2.zero;
            Destroy(designation);
            yield return null;
            SelectFocusedShip(ship);
            //SwitchPause();
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
            int speed = 5 * Database.FocusedShip.ShipData.ElementsCount;
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
                Math.Abs(SelectedShipRigidbody.velocity.magnitude -
                _target.velocity.magnitude);
        }

        #endregion Private

        #region Unity

        private void Awake()
        {
            _cockpit = this;
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
                InstantiateCloseShips();
            }
        }

        #endregion Unity
    }
}