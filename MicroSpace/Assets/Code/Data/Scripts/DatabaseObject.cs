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
            Position = BigVector2.Zero;
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
            Position.X = Position.X > Database.WorldBorder ?
                Position.X - 2 * Database.WorldBorder :
                Position.X < -Database.WorldBorder ?
                    Position.X + 2 * Database.WorldBorder :
                    Position.X;
            Position.Y = Position.Y > Database.WorldBorder ?
                Position.Y - 2 * Database.WorldBorder :
                Position.Y < -Database.WorldBorder ?
                    Position.Y + 2 * Database.WorldBorder :
                    Position.Y;
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

        public bool IsShipDistant()
        {
            double x = Position.X - Database.FocusedShip.Position.X;
            double y = Position.Y - Database.FocusedShip.Position.Y;

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