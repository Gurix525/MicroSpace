using Miscellaneous;
using UnityEngine;

namespace Entities
{
    public abstract class Entity : MonoBehaviour, IIdentifiable
    {
        public abstract int Id { get; set; }
    }
}