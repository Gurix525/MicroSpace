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
        private List<SerializableShip> _ships = new();

        public List<SerializableShip> Ships => _ships;

        public Save(List<Ship> ships)
        {
            foreach (var ship in ships)
                _ships.Add(new(ship));
        }
    }
}