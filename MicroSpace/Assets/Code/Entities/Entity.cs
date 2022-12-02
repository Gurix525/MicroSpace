using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public abstract class Entity : MonoBehaviour, IEntity
    {
        public abstract int Id { get; set; }
    }
}