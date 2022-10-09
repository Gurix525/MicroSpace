using Assets.Code.Data;
using Assets.Code.ExtensionMethods;
using Assets.Code.Ships;
using System;
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
        private GameObject _designationPrefab;

        [SerializeField]
        private GameObject _wallPrefab;

        [SerializeField]
        private GameObject _floorPrefab;

        [SerializeField]
        private GameObject _wallDesignationPrefab;

        [SerializeField]
        private GameObject _floorDesignationPrefab;

        [SerializeField]
        private Transform _worldTransform;

        [SerializeField]
        private int _maxDesignDistance = 10;

        private Cockpit _cockpit;

        #endregion Fields

        #region Properties

        public GameObject WallPrefab => _wallPrefab;

        public GameObject FloorPrefab => _floorPrefab;

        #endregion Properties

        #region Public

        public void StartDesignateBlock(BlockType blockType)
        {
            StartCoroutine(DesignateBlock(blockType));
        }

        public void StartCancelDesignation()
        {
            StartCoroutine(CancelDesignation());
        }

        #endregion Public

        #region Private

        private Vector3 GetMousePosition()
        {
            Vector3 v3 = Input.mousePosition;
            v3.z = 10;
            v3 = Camera.main.ScreenToWorldPoint(v3);
            return v3;
        }

        private IBlock FindClosestBlock(
            GameObject designation, Vector3 v3)
        {
            var num = FindObjectsOfType<MonoBehaviour>()
                .OfType<IBlock>()
#pragma warning disable CS0252
                .Where(x => x != designation.GetComponent<BlockDesignation>())
#pragma warning restore
                .Where(x => x.Parent.GetComponent<Ship>() != null);
            var closestWall = num.Count() > 0 ?
                num.Aggregate((closest, next) =>
                Vector2.Distance(closest.Transform.position, v3) <
                Vector2.Distance(next.Transform.position, v3) ?
                closest : next) :
                null;
            return closestWall;
        }

        private static void UpdateShipData(GameObject ship)
        {
            ship.GetComponent<Ship>().UpdateShipData(ship);
        }

        private void MoveBlockDesignation(
            GameObject designation, IBlock closestBlock, Vector3 v3)
        {
            if (closestBlock != null ?
                    Vector2.Distance(closestBlock.Transform.position, v3) < 1.5F :
                    false)
            {
                var v3relative = closestBlock.Transform.InverseTransformPoint(v3);
                designation.transform.parent = closestBlock.Parent;
                designation.transform.localPosition =
                    closestBlock.Transform.localPosition +
                    v3relative
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

        private void CreateDesignations(IBlock closestBlock,
            Vector3 originalPos, List<GameObject> designations, ref Vector3 localMousePos)
        {
            localMousePos = closestBlock.Parent
                .InverseTransformPoint(GetMousePosition());
            localMousePos = localMousePos.Round();
            ClampDistance(originalPos, ref localMousePos, _maxDesignDistance);
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
                        closestBlock.Parent));
                    designations[^1].transform.localPosition = new Vector3(x, y, 0);
                }
        }

        private void ClampDistance(Vector3 originalPos, ref Vector3 localMousePos, float maxDistance)
        {
            float x = localMousePos.x - originalPos.x > maxDistance ?
                originalPos.x + maxDistance :
                originalPos.x - localMousePos.x > maxDistance ?
                originalPos.x - maxDistance :
                localMousePos.x;
            float y = localMousePos.y - originalPos.y > maxDistance ?
                originalPos.y + maxDistance :
                originalPos.y - localMousePos.y > maxDistance ?
                originalPos.y - maxDistance :
                localMousePos.y;
            localMousePos = new(x, y);
        }

        private IEnumerator DesignateBlock(BlockType blockType)
        {
            var prefab = blockType switch
            {
                BlockType.Floor => _floorDesignationPrefab,
                _ => _wallDesignationPrefab
            };
            _cockpit.SwitchSetup();
            GameObject designation = Instantiate(_designationPrefab, _worldTransform);
            // FindClosestBlock jest potrzebne żeby nie krzyczało że pusty obiekt
            IBlock closestBlock = FindClosestBlock(designation, Vector3.zero);
            while (!Input.GetKeyDown(KeyCode.Mouse0) || IsDesignationObstructed(designation))
            {
                if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    Destroy(designation);
                    _cockpit.SwitchSetup();
                    yield break;
                }
                var mousePos = GetMousePosition();
                closestBlock = FindClosestBlock(designation, mousePos);
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
            Vector3 oldLocalMousePos = Vector3.positiveInfinity;
            List<GameObject> designations = new();
            while (!Input.GetKeyDown(KeyCode.Mouse0) || AreDesignationsObstructed(designations))
            {
                if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    DestroyDesignations(designations);
                    _cockpit.SwitchSetup();
                    yield break;
                }
                localMousePos = closestBlock.Parent
                        .InverseTransformPoint(GetMousePosition());
                localMousePos = localMousePos.Round();
                if (localMousePos != oldLocalMousePos)
                {
                    DestroyDesignations(designations);
                    CreateDesignations(
                        closestBlock, originalPos, designations, ref localMousePos);
                }
                oldLocalMousePos = localMousePos;
                yield return null;
            }
            for (int i = 0; i < designations.Count; i++)
            {
                var block = Instantiate(prefab, closestBlock.Parent);
                block.transform.localPosition = designations[i].transform.localPosition;
            }
            DestroyDesignations(designations);
            UpdateShipData(closestBlock.Parent.gameObject);
            _cockpit.SwitchSetup();
        }

        private IEnumerator CancelDesignation()
        {
            _cockpit.SwitchSetup();
            yield return null;
            _cockpit.SwitchSetup();
        }

        private static bool AreDesignationsObstructed(List<GameObject> designations)
        {
            foreach (var item in designations)
                if (item.GetComponent<BlockDesignation>().IsObstructed)
                    return true;
            return false;
        }

        private static bool IsDesignationObstructed(GameObject designation)
        {
            return designation.GetComponent<BlockDesignation>().IsObstructed;
        }

        private static void DestroyDesignations(List<GameObject> designations)
        {
            for (int i = 0; i < designations.Count; i++)
            {
                Destroy(designations[0]);
                designations.RemoveAt(0);
                i--;
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