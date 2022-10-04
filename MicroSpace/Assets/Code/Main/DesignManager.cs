using Assets.Code.Data;
using Assets.Code.ExtensionMethods;
using Assets.Code.Ships;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code.Main
{
    public class DesignManager : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private GameObject _wallPrefab;

        [SerializeField]
        private GameObject _floorPrefab;

        [SerializeField]
        private GameObject _designationPrefab;

        [SerializeField]
        private Transform _worldTransform;

        private Cockpit _cockpit;

        #endregion Fields

        #region Properties

        public GameObject WallPrefab => _wallPrefab;

        public GameObject FloorPrefab => _floorPrefab;

        #endregion Properties

        #region Public

        public IEnumerator DesignateBlock(BlockType blockType)
        {
            var prefab = blockType switch
            {
                BlockType.Floor => _floorPrefab,
                _ => _wallPrefab
            };
            _cockpit.SwitchSetup();
            GameObject designation = Instantiate(_designationPrefab, _worldTransform);
            BoxCollider2D closestBlock = new();
            while (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                var mousePos = GetMousePosition();
                closestBlock = FindClosestWall(designation, mousePos);
                MoveBlockDesignation(designation, closestBlock, mousePos);
                yield return null;
            }
            if (designation.transform.parent == null)
            {
                Destroy(designation);
                _cockpit.SwitchSetup();
                yield break;
            }
            Vector3 originalPos = designation.transform.localPosition;
            originalPos = originalPos.Round();
            Destroy(designation);
            yield return null;
            Vector3 localMousePos = new();
            List<GameObject> designations = new();
            while (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    _cockpit.SwitchSetup();
                    yield break;
                }
                DestroyDesignations(designations);
                CreateDesignations(
                    closestBlock, originalPos, designations, ref localMousePos);
                yield return null;
            }
            for (int i = 0; i < designations.Count; i++)
            {
                var block = Instantiate(prefab, closestBlock.transform.parent);
                block.transform.localPosition = designations[i].transform.localPosition;
            }
            for (int i = 0; i < designations.Count; i++)
                Destroy(designations[i]);
            UpdateShipData(closestBlock.transform.parent.gameObject);
            _cockpit.SwitchSetup();
        }

        #endregion Public

        #region Private

        private static void DestroyDesignations(List<GameObject> designations)
        {
            for (int i = 0; i < designations.Count; i++)
            {
                Destroy(designations[0]);
                designations.RemoveAt(0);
                i--;
            }
        }

        private Vector3 GetMousePosition()
        {
            Vector3 v3 = Input.mousePosition;
            v3.z = 10;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            return v3;
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

        private void UpdateShipData(GameObject ship)
        {
            var db = Database.DBObjects;
            db.Find(x => x.GameObject == ship)
                .UpdateShipData();
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

        private void CreateDesignations(BoxCollider2D closestBlock,
            Vector3 originalPos, List<GameObject> designations, ref Vector3 localMousePos)
        {
            localMousePos = closestBlock.transform.parent
                .InverseTransformPoint(GetMousePosition());
            localMousePos = localMousePos.Round();
            int lesserX = 0;
            int lesserY = 0;
            int greaterX = 0;
            int greaterY = 0;
            if (localMousePos.x < originalPos.x)
            {
                lesserX = (int)localMousePos.x;
                greaterX = (int)originalPos.x;
            }
            else
            {
                greaterX = (int)localMousePos.x;
                lesserX = (int)originalPos.x;
            }
            if (localMousePos.y < originalPos.y)
            {
                lesserY = (int)localMousePos.y;
                greaterY = (int)originalPos.y;
            }
            else
            {
                greaterY = (int)localMousePos.y;
                lesserY = (int)originalPos.y;
            }
            for (int x = lesserX; x <= greaterX; x++)
                for (int y = lesserY; y <= greaterY; y++)
                {
                    designations.Add(Instantiate(
                        _designationPrefab,
                        closestBlock.transform.parent));
                    designations[^1].transform.localPosition = new Vector3(x, y, 0);
                }
        }

        #endregion Private

        #region Unity

        private void Awake()
        {
            _cockpit = GetComponent<Cockpit>();
        }

        #endregion Unity
    }
}