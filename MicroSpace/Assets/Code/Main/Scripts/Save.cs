using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Code.Ships;
using UnityEngine;

namespace Assets.Code.Main
{
    [Serializable]
    public class Save
    {
        [SerializeField]
        private List<Ship> _ships;

        public List<Ship> Ships => _ships;

        public Save(List<Ship> ships)
        {
            _ships = ships;
        }
    }
}