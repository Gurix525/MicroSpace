using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Code.Data;
using Assets.Code.Data.PartDataImplementations;

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
    }
}