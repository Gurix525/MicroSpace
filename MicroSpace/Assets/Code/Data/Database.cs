using Assets.Code.Data.Saves;
using Assets.Code.Ships;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Data
{
    public class Database : MonoBehaviour
    {
        /// <summary>
        /// Maximum x, y components of DatabaseObject on map, in meters
        /// </summary>
        public static readonly double WorldBorder = 1000000;

        public static List<DBObject> DBObjects = new();

        public static DBObject FocusedShip = null;

        [SerializeField] private GameObject _world;

        public static Save Save =>
            new(DBObjects, FocusedShip);

        public static void LoadFromSave(Save save)
        {
            DBObjects = save.DBObjects;
            FocusedShip = save.FocusedShip;
        }

        private void CalculateDBObjectsPositions(long physicsFrames = 1)
        {
            foreach (DBObject item in DBObjects)
                item.CalculatePosition(physicsFrames);
        }

        private void DestroyDistantShips()
        {
            foreach (DBObject item in DBObjects)
                if (item.GameObject != null)
                    item.DestroyDistantShip();
        }

        private void FixedUpdate()
        {
            CalculateDBObjectsPositions();
            if (FocusedShip != null)
            {
                UpdateDBObjectVelocity();
                DestroyDistantShips();
                InstantiateCloseShips();
            }
        }

        private Vector3 MapPosition(BigVector2 pos)
        {
            float num = (float)Math.Clamp(pos.x, -WorldBorder, WorldBorder);
            num /= 100000;
            float num2 = (float)Math.Clamp(pos.y, -WorldBorder, WorldBorder);
            num2 /= 100000;

            return new Vector3(num, num2, 0);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            foreach (DBObject item in DBObjects)
                Gizmos.DrawSphere((Vector2)MapPosition(item.Position) +
                    (Vector2)transform.position, 0.2f);
        }

        private void UpdateDBObjectVelocity()
        {
            foreach (Transform item in _world.transform)
                if (item.gameObject.GetComponent<Ship>() != null)
                {
                    var a = DBObjects.Count;
                    DBObjects
                        ?.Find(x => x.GameObject == item.gameObject)
                        ?.UpdateRigidbodyData();
                }
        }

        private void InstantiateCloseShips()
        {
            foreach (DBObject item in DBObjects)
                if (item.GameObject == null)
                    item.InstantiateCloseShip();
        }
    }
}