using Assets.Code.Data;
using Assets.Code.Data.Saves;
using Assets.Code.ExtensionMethods;
using Assets.Code.Ships;
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
        public GameObject WallDesignationPrefab;
        public GameObject WallPrefab;

        private bool isSetupRunnning = false;

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
            Destroy(designation);
            yield return null;
            SelectFocusedShip(ship);
            SwitchPause();
            SwitchSetup();
        }

        private IEnumerator DesignateWallCoroutine()
        {
            SwitchSetup();
            SwitchPause();
            GameObject designation = Instantiate(WallDesignationPrefab, _world);
            while (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 v3 = MousePosition();
                BoxCollider2D closestWall = FindClosestWall(designation, v3);
                MoveWallDesignation(designation, closestWall, v3);
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

        private BoxCollider2D FindClosestWall(
            GameObject designation, Vector3 v3)
        {
            var num = FindObjectsOfType<BoxCollider2D>()
                .Where(x => x != designation.GetComponent<BoxCollider2D>())
                .Where(x => x.transform.parent.GetComponent<Ship>() != null);
            var closestWall = num.Count() > 0 ?
                num.Aggregate((closest, next) =>
                Vector2.Distance(closest.transform.position, v3) <
                Vector2.Distance(next.transform.position, v3) ?
                closest : next) :
                null;
            return closestWall;
        }

        private void FixedUpdate()
        {
            if (SelectedShipRigidbody != null)
            {
                SteerTheShip();
                AlignScenePosition();
                AlignCamera();
            }
        }

        private Vector3 MousePosition()
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

        private void MoveWallDesignation(
            GameObject designation, BoxCollider2D closestWall, Vector3 v3)
        {
            if (closestWall != null ?
                    Vector2.Distance(closestWall.transform.position, v3) < 2 :
                    false)
            {
                var v3relative = closestWall.transform.InverseTransformPoint(v3);
                designation.transform.parent = closestWall.transform.parent;
                designation.transform.localPosition =
                    closestWall.transform.localPosition +
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

        private void SelectFocusedShip(GameObject ship, bool mouseSelection = false)
        {
            if (mouseSelection)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
                if (hit.collider != null)
                    ship = hit.collider.transform.parent.gameObject;
            }
            if (ship != null)
            {
                Debug.Log(ship);
                SelectedShipRigidbody = ship.GetComponent<Rigidbody2D>();
                Database.FocusedShip = Database.DBObjects
                    .Find(x => x.GameObject == ship);
            }
        }

        private void SteerTheShip()
        {
            if (Input.GetKey(KeyCode.W))
                SelectedShipRigidbody.AddForce(
                    SelectedShipRigidbody.transform.up * 5);
            if (Input.GetKey(KeyCode.S))
                SelectedShipRigidbody.AddForce(
                    SelectedShipRigidbody.transform.up * -5);
            if (Input.GetKey(KeyCode.D))
                SelectedShipRigidbody.AddForce(
                    SelectedShipRigidbody.transform.right * 5);
            if (Input.GetKey(KeyCode.A))
                SelectedShipRigidbody.AddForce(
                    SelectedShipRigidbody.transform.right * -5);
            if (Input.GetKey(KeyCode.E))
                SelectedShipRigidbody.AddTorque(-5);
            if (Input.GetKey(KeyCode.Q))
                SelectedShipRigidbody.AddTorque(5);
            if (Input.GetKey(KeyCode.Space))
            {
                SelectedShipRigidbody.drag = 10;
                SelectedShipRigidbody.angularDrag = 10;
            }
            else
            {
                SelectedShipRigidbody.drag = 0;
                SelectedShipRigidbody.angularDrag = 0;
            }
        }

        private void SwitchPause()
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        }

        private void SwitchSetup() => // Used to switch setup on/off
                    isSetupRunnning ^= true;

        private void Update()
        {
            if (isSetupRunnning)
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
                StartCoroutine(DesignateWallCoroutine());
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectFocusedShip(null, true);
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
            rb.velocity = (Vector3)dbo.Velocity;
            rb.rotation = dbo.Rotation;
            rb.angularVelocity = dbo.AngularVelocity;
            foreach (WallData wallData in dbo.ShipData.Walls)
            {
                if (wallData.Name == "Core")
                    continue;
                GameObject wall = Instantiate(
                    _cockpit.WallPrefab, ship.transform);
                wall.transform.localPosition = new Vector2(
                     wallData.LocalPosition[0], wallData.LocalPosition[1]);
                wall.name = wallData.Name;
                var wallComponent = wall.GetComponent<Wall>();
                wallComponent.Name = wallData.Name;
                wallComponent.Resilience = wallData.Resilience;
                wallComponent.MaxEndurance = wallData.MaxEndurance;
                wallComponent.CurrentEndurance = wallData.CurrentEndurance;
            }
        }
    }
}