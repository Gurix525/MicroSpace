using Assets.Code.Data.PartDataImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Ships
{
    /// <summary>
    /// Komponent umieszczany w konkretnych prefabach Block,
    /// ma swój odpowiednik w Data.BlockData.
    /// </summary>
    public class Wall : MonoBehaviour
    {
        //public string Name;
        //public float Resilience; // Minimum energy to do damage
        //public float MaxEndurance; // Maximum taken damage
        //public float CurrentEndurance;

        public WallData WallData;
    }
}