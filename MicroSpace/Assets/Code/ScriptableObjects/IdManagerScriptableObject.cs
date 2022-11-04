using UnityEngine;
using Attributes;

namespace ScriptableObjects
{
    [CreateAssetMenu(
        fileName = "IdManager",
        menuName = "ScriptableObjects/IdManager")]
    public class IdManagerScriptableObject : ScriptableObject
    {
        [SerializeField]
        [ReadonlyInspector]
        private int _nextId = 1;

        public int NextId
        {
            get => _nextId++;
            set => _nextId = value;
        }

        private void OnEnable()
        {
            NextId = 1;
        }
    }
}