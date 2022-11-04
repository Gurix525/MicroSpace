using UnityEngine;
using Attributes;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "IdManager",
        menuName = "ScriptableObjects/IdManagerScriptableObject")]
    public class IdManagerScriptableObject : ScriptableObject
    {
        [SerializeField]
        [ReadonlyInspector]
        private int _nextId = 0;

        public int NextId
        {
            get => _nextId++;
            set => _nextId = value;
        }
    }
}