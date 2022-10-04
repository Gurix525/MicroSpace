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
        private GameObject _blockPrefab;

        [SerializeField]
        private GameObject _wallPrefab;

        [SerializeField]
        private GameObject _designationPrefab;

        [SerializeField]
        private Transform _worldTransform;

        private Cockpit _cockpit;

        #endregion Fields

        #region Public

        public IEnumerator DesignateBlockCoroutine(GameObject prefab)
        {
            _cockpit.SwitchSetup();
            GameObject designation = Instantiate(_designationPrefab, _worldTransform);
            while (!Input.GetKeyDown(KeyCode.Mouse0))
            {
                Vector3 mousePos = GetMousePosition();
                BoxCollider2D closestBlock = FindClosestBlock(designation, mousePos);
                MoveBlockDesignation(designation, closestBlock, mousePos);
                yield return null;
            }
            if (designation.transform.parent != null)
            {
                Instantiate(
                    prefab,
                    designation.transform.position,
                    designation.transform.rotation,
                    designation.transform.parent);
                UpdateShipData(designation.transform.parent.gameObject);
            }
            Destroy(designation);
            _cockpit.SwitchSetup();
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

        #endregion Private

        #region Unity

        private void Awake()
        {
            _cockpit = GetComponent<Cockpit>();
        }

        #endregion Unity
    }
}