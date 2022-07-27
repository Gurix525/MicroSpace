using Assets.Code.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.Data
{
    [Serializable]
    public class DBObject
    {
        public string Name;
        public BigVector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float AngularVelocity;
        public ShipData ShipData = new();
        [NonSerialized] public GameObject GameObject;

        public DBObject(string name = "", string desc = "")
        {
            Position = BigVector2.zero;
            Velocity = Vector2.zero;
            Name = name;
        }

        public DBObject(BigVector2 pos, string name = "", string desc = "")
        {
            Position = pos;
            Velocity = Vector2.zero;
            Name = name;
        }

        public DBObject(BigVector2 pos, Vector2 vel, string name = "", string desc = "")
        {
            Position = pos;
            Velocity = vel;
            Name = name;
        }

        /// <summary>
        /// Calculate new DatabaseObject position based on Velocity
        /// and given amount of physics frames (default = 1)
        /// </summary>
        /// <param name="physicsFrames"></param>
        public void CalculatePosition(long physicsFrames = 1)
        {
            Position += Velocity * physicsFrames * Time.fixedDeltaTime;
            Position.x = Position.x > Database.WorldBorder ?
                Position.x - 2 * Database.WorldBorder :
                Position.x < -Database.WorldBorder ?
                    Position.x + 2 * Database.WorldBorder :
                    Position.x;
            Position.y = Position.y > Database.WorldBorder ?
                Position.y - 2 * Database.WorldBorder :
                Position.y < -Database.WorldBorder ?
                    Position.y + 2 * Database.WorldBorder :
                    Position.y;
        }

        public void UpdateRigidbodyData()
        {
            if (GameObject != null)
                if (GameObject.GetComponent<Rigidbody2D>())
                {
                    var rb = GameObject.GetComponent<Rigidbody2D>();
                    Velocity = rb.velocity;
                    Rotation = rb.rotation;
                    AngularVelocity = rb.angularVelocity;
                }
        }

        public void UpdateShipData() =>
            ShipData.Update(GameObject);

        public void DestroyDistantShip()
        {
            if (IsShipDistant())
                UnityEngine.Object.Destroy(GameObject);
        }

        public void InstantiateCloseShip()
        {
            if (!IsShipDistant())
                Cockpit.InstantiateShipFromDB(this);
        }

        private bool IsShipDistant()
        {
            double x = Position.x - Database.FocusedShip.Position.x;
            double y = Position.y - Database.FocusedShip.Position.y;

            bool isXDistant = x > 60 ?
                true :
                x < -60 ?
                    true :
                    false;

            bool isYDistant = y > 60 ?
                true :
                y < -60 ?
                    true :
                    false;

            return isXDistant || isYDistant;
        }
    }
}