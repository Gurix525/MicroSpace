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
    public class Wall : MonoBehaviour
    {
        //public string Name;
        //public float Resilience; // Minimum energy to do damage
        //public float MaxEndurance; // Maximum taken damage
        //public float CurrentEndurance;

        public WallData WallData = null;

        private void Update()
        {
            if (WallData.IsExposed)
                GetComponent<SpriteRenderer>().color = new Color32(127, 63, 63, 255);
            else
                GetComponent<SpriteRenderer>().color = new Color32(63, 63, 127, 255);
        }
    }
}