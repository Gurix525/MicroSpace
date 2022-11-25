using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public class SerializableAstronaut
    {
        // Pola nie mogą być readonly bo serializacja nie zadziała

        [SerializeField]
        private int _id;

        [SerializeField]
        private int _parentId;

        [SerializeField]
        private Vector2 _localPosition;

        public int Id => _id;

        public int ParentId => _parentId;

        public Vector2 LocalPosition => _localPosition;

        public SerializableAstronaut(Astronaut astronaut)
        {
            _id = astronaut.Id;
            _parentId = astronaut.GetParentId();
            _localPosition = astronaut.transform.localPosition;
        }

        public static implicit operator SerializableAstronaut(Astronaut astronaut)
        {
            return new SerializableAstronaut(astronaut);
        }
    }
}