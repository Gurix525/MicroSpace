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
        [field: SerializeField]
        public int Id { get; private set; }

        [field: SerializeField]
        public int ParentId { get; private set; }

        [field: SerializeField]
        public Vector2 LocalPosition { get; private set; }

        public SerializableAstronaut(Astronaut astronaut)
        {
            Id = astronaut.Id;
            ParentId = astronaut.GetParentId();
            LocalPosition = astronaut.transform.localPosition;
        }

        public static implicit operator SerializableAstronaut(Astronaut astronaut)
        {
            return new SerializableAstronaut(astronaut);
        }
    }
}