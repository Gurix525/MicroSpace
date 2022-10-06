using UnityEngine;
using System.Collections.Generic;
using Assets.Code.Data;

namespace Assets.Code.Ships
{
    public class Ship : MonoBehaviour
    {
        public DBObject DBObject;

        private void Start()
        {
            AssignDBObject();
        }

        private void AssignDBObject()
        {
            var dbObject = Database.DBObjects
                .Find(x => x.GameObject == gameObject);
            if (dbObject == null)
                dbObject = CreateDBObject();
            DBObject = dbObject;
            DBObject.UpdateShipData();
        }

        private DBObject CreateDBObject()
        {
            var db = Database.DBObjects;
            db.Add(new());
            var shipData = db[^1];
            shipData.GameObject = gameObject;
            shipData.Name = $"Ship No {Database.DBObjects.Count}";
            shipData.Position = db.Count == 1 ?
                BigVector2.Zero :
                (db[^2].Position +
                (BigVector2)transform.localPosition);
            return db[^1];
        }
    }
}