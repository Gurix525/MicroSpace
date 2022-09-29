using Assets.Code.Data;
using Assets.Code.Data.PartDataImplementations;
using Assets.Code.Data.Saves;
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
        public Rigidbody2D SelectedShipRigidbody = null;
        public GameObject ShipDesignationPrefab;
        public GameObject ShipPrefab;
        public GameObject BlockDesignationPrefab;
        public GameObject BlockPrefab;
        public GameObject WallPrefab;
        public UIController UIController;

        private bool _isSetupRunnning = false;
        public float Speedometer; // For UI speedometer purposes

        [SerializeField] private Rigidbody2D _target;
        [SerializeField] private Transform _world;
        [SerializeField] private static Cockpit _cockpit; // singleton

        private void Awake()
        {
            _cockpit = GameObject.Find("Cockpit")
                .GetComponent<Cockpit>();
        }

        private void UpdateShipData(GameObject ship)
        {
            var db = Database.DBObjects;
            db.Find(x => x.GameObject == ship)
                .UpdateShipData();
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
            SwitchPause();
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
            SwitchPause();
            SwitchSetup();
        }

        private IEnumerator DesignateBlockCoroutine()
        {
            SwitchSetup();
            SwitchPause();
            GameObject designation = Instantiate(BlockDesignationPrefab, _world);
            while (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 v3 = GetMousePosition();
                BoxCollider2D closestBlock = FindClosestBlock(designation, v3);
                MoveBlockDesignation(designation, closestBlock, v3);
                yield return null;
            }
            if (designation.transform.parent != null)
            {
                Instantiate(
                    BlockPrefab,
                    designation.transform.position,
                    designation.transform.rotation,
                    designation.transform.parent);
                UpdateShipData(designation.transform.parent.gameObject);
            }
            Destroy(designation);
            SwitchPause();
            SwitchSetup();
        }

        private IEnumerator DesignateWallCoroutine()
        {
            SwitchSetup();
            SwitchPause();
            GameObject designation = Instantiate(BlockDesignationPrefab, _world);
            while (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 v3 = GetMousePosition();
                BoxCollider2D closestBlock = FindClosestBlock(designation, v3);
                MoveBlockDesignation(designation, closestBlock, v3);
                yield return null;
            }
            if (designation.transform.parent != null)
            {
                Instantiate(
                    WallPrefab,
                    designation.transform.position,
                    designation.transform.rotation,
                    designation.transform.parent);
                UpdateShipData(designation.transform.parent.gameObject);
            }
            Destroy(designation);
            SwitchPause();
            SwitchSetup();
        }

        private BoxCollider2D FindClosestBlock(
            GameObject designation, Vector3 v3)
        {
            var num = FindObjectsOfType<BoxCollider2D>()
                .Where(x => x != designation.GetComponent<BoxCollider2D>())
                .Where(x => x.transform.parent.GetComponent<Ship>() != null);
            var closestBlock = num.Count() > 0 ?
                num.Aggregate((closest, next) =>
                Vector2.Distance(closest.transform.position, v3) <
                Vector2.Distance(next.transform.position, v3) ?
                closest : next) :
                null;
            return closestBlock;
        }

        private void FixedUpdate()
        {
            if (SelectedShipRigidbody != null)
            {
                SteerTheShip();
                AlignScenePosition();
                AlignCamera();
                UpdateSpeedometer();
            }
        }

        private Vector3 GetMousePosition()
        {
            Vector3 v3 = Input.mousePosition;
            v3.z = 10;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            return v3;
        }

        private void MoveShipDesignation(GameObject designation)
        {
            var v3 = Input.mousePosition;
            v3.z = 10;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            designation.transform.position = v3;
        }

        private void MoveBlockDesignation(
            GameObject designation, BoxCollider2D closestBlock, Vector3 v3)
        {
            if (closestBlock != null ?
                    Vector2.Distance(closestBlock.transform.position, v3) < 2 :
                    false)
            {
                var v3relative = closestBlock.transform.InverseTransformPoint(v3);
                designation.transform.parent = closestBlock.transform.parent;
                designation.transform.localPosition =
                    closestBlock.transform.localPosition +
                    v3relative
                    .normalized
                    .Round();
                designation.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                designation.transform.parent = null;
                designation.transform.position = v3;
                designation.transform.rotation = Quaternion.identity;
            }
        }

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

        private void SteerTheShip()
        {
            int speed = 5 * Database.FocusedShip.ShipData.ElementsCount;

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
                SelectedShipRigidbody.AddTorque(-speed);
            if (Input.GetKey(KeyCode.Q))
                SelectedShipRigidbody.AddTorque(speed);
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

        private void SwitchSetup() => // Used to switch setup on/off
                    _isSetupRunnning ^= true;

        private void UpdateSpeedometer()
        {
            Speedometer = _target == null ?
                SelectedShipRigidbody.velocity.magnitude :
                Math.Abs(SelectedShipRigidbody.velocity.magnitude -
                _target.velocity.magnitude);
        }

        private void Update()
        {
            if (_isSetupRunnning)
                return;

            if (Input.GetKeyDown(KeyCode.N))
            {
                StartCoroutine(BuildShipCoroutine());
                return;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                SwitchPause();
                return;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                StartCoroutine(DesignateBlockCoroutine());
                return;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                StartCoroutine(DesignateWallCoroutine());
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

        public static void InstantiateShipFromDB(DBObject dbo)
        {
            GameObject ship = GameObject.Instantiate(
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

            foreach (BlockData blockData in dbo.ShipData.Blocks)
            {
                if (blockData.Name == "Core")
                    continue;
                GameObject block = Instantiate(
                    _cockpit.BlockPrefab, ship.transform);
                block.transform.localPosition = new Vector2(
                     blockData.LocalPosition[0], blockData.LocalPosition[1]);
                block.name = blockData.Name;
                var blockComponent = block.GetComponent<Block>();
                blockComponent.BlockData = blockData;
            }

            foreach (WallData wallData in dbo.ShipData.Walls)
            {
                GameObject wall = Instantiate(
                    _cockpit.WallPrefab, ship.transform);
                wall.transform.localPosition = new Vector2(
                    wallData.LocalPosition[0], wallData.LocalPosition[1]);
                wall.name = wallData.Name;
                var wallComponent = wall.GetComponent<Wall>();
                wallComponent.WallData = wallData;
                wallData.Room = dbo.ShipData.Rooms
                    .Find(x => x.Id == wallData.Room.Id);
            }
        }
    }
}