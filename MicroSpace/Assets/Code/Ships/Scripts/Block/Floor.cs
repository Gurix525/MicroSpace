using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Code.Data;

namespace Assets.Code.Ships
{
    public class Floor : Block
    {
        //public string Name;
        //public float Resilience; // Minimum energy to do damage
        //public float MaxEndurance; // Maximum taken damage
        //public float CurrentEndurance;

        [SerializeField]
        private FloorData _floorData = null;

        public FloorData FloorData { get => _floorData; set => _floorData = value; }

        private void Update()
        {
            // Poniższego ifa trzeba przenieść do ShipData.UpdateRooms()
            // i uogólnić, żeby się wszystkie sprawdzały od razu po sprawdzaniu
            // czy są exposed
            if (_floorData != null)
            {
                if (FloorData.IsExposed)
                    GetComponent<SpriteRenderer>().color = new Color32(127, 63, 63, 255);
                else
                    GetComponent<SpriteRenderer>().color = new Color32(63, 63, 127, 255);
            }
        }

        public static implicit operator FloorData(Floor floor)
        {
            var floorData = floor.FloorData ?? new();
            //wallData.Name = wall.Name;
            //wallData.Resilience = wall.Resilience;
            //wallData.MaxEndurance = wall.MaxEndurance;
            //wallData.CurrentEndurance = wall.CurrentEndurance;
            floorData.LocalPosition = new int[]
            {
                (int) Math.Round(floor.gameObject.transform.localPosition.x),
                (int) Math.Round(floor.gameObject.transform.localPosition.y)
            };
            floorData.LocalRotation = floor.gameObject.transform.localEulerAngles.z;
            floor.FloorData = floorData;
            return floorData;
        }
    }
}