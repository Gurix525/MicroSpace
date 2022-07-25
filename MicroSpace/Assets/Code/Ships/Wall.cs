using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Code.Data;

namespace Assets.Code.Ships
{
    public class Wall : MonoBehaviour
    {
        public string Name;
        public float Resilience; // Minimum energy to do damage
        public float MaxEndurance; // Maximum taken damage
        public float CurrentEndurance;
        [NonSerialized] public RoomData Room = null;
    }
}